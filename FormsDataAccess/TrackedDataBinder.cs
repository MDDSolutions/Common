using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MDDDataAccess;
using MDDFoundation; // TrackedEntity<T>, TrackedList<T>


namespace FormsDataAccess
{
    /// <summary>
    /// TrackedDataBinder<T>
    /// ---------------------
    /// Runtime-only binder that:
    ///  - Discovers designer-authored bindings for a specific BindingSource (constructor)
    ///    and builds a control↔property map limited to that BindingSource.
    ///  - Optionally removes those bindings at runtime so nothing implicit fires.
    ///  - Wires minimal control events; parses user input; distinguishes EditPending vs Dirty (without spamming the model).
    ///  - Keeps controls in sync with model notifications (NotifierObject preferred, INPC supported).
    ///  - Integrates tightly with TrackedList<T>: it listens to CurrentChanged and owns Save/Next/Prev/New buttons.
    ///
    /// Key behaviors
    ///  - Default commit policy is OnCommit (commit when control leaves). During typing, we DO NOT push to the model.
    ///  - Per-control visual state: optional DirtyBackColor and PendingBackColor.
    ///  - External model changes while editing: default is QueueWhileEditing (apply on Leave).
    ///  - BindingSource is kept in sync (DataSource = current entity) to support other controls bound to the same source.
    ///
    /// Notes
    ///  - Only bindings targeting the supplied BindingSource instance are mapped.
    ///  - Requires TrackedEntity<T>.Initialize to have run for T (your tracker handles this).
    ///  - C# 7.3 compatible.
    /// </summary>
    public sealed class TrackedDataBinder<T> : IDisposable where T : class, new()
    {
        // Construction and Initialization
        private readonly ContainerControl _root;
        private BindingSource _bindingSource;
        private readonly ErrorProvider _errors; // optional
        private readonly bool _clearDesignerBindings;        
        // Required navigation integration (recommended always)
        private readonly TrackedList<T> _list;
        /// <summary>
        /// Build the control↔property map at startup and subscribe to TrackedList.
        /// </summary>
        public TrackedDataBinder(ContainerControl root, BindingSource bindingSource, TrackedList<T> list, ErrorProvider errors = null, bool clearDesignerBindings = true)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (bindingSource == null) throw new ArgumentNullException(nameof(bindingSource));

            _root = root;

            _bindingSource = bindingSource;
            _list = list; // may be null, but recommended non-null
            _errors = errors;
            _clearDesignerBindings = clearDesignerBindings;

            BuildMapFromDesignerBindings();

            if (_list != null)
            {
                _list.CurrentChanged += OnListCurrentChanged;
                _list.PropertyChanged += OnListPropertyChanged;
                _list.PreparingForNavigation += OnPreparingForNavigation;
                // Initialize to current
                if (_list.CurrentEntity != null)
                    LoadFromList(_list.CurrentEntity, _list.Current);
            }
        }
        /// <summary>
        /// Attach UI buttons. You can pass null for any you don't use.
        /// </summary>
        private Button _btnSave, _btnNext, _btnPrev, _btnNew;
        public void AttachButtons(Button btnSave = null, Button btnNext = null, Button btnPrev = null, Button btnNew = null)
        {
            _btnSave = btnSave; _btnNext = btnNext; _btnPrev = btnPrev; _btnNew = btnNew;
            WireButtons();
            RefreshButtonsEnabled();
        }
        // Map: one per control-binding that targets _bindingSource
        private sealed class Map : PropertyDelegateInfo<T> // reuse Getter/Setter/flags from your APD
        {
            public Control Control;
            public string ControlProp;     // Text / Checked / Value / SelectedValue
            public string EntityProp;      // property name on T
            public Type EntityPropType;    // set from APD.PropertyType if available, else reflection fallback
            public bool ProgrammaticSet;

            public Color OriginalBackColor;
            public bool IsObjectBinding;               // binding to the whole entity
            public PropertyInfo ControlPropertyInfo;   // reflected once: property to set on the control

            // editing state
            public bool InEditSession;     // true while user is actively editing
            public bool HasPendingParsed;  // a valid value typed but not yet committed
            public object PendingParsedValue;
            public bool HasStagedValidValue;   // set during Validating if parse succeeded & differs
            public object StagedValidValue;      // the parsed candidate

            // external updates while editing
            public bool HasQueuedExternal;
            public object QueuedExternalValue;

            // cached event handlers so we can unwind reliably
            public EventHandler EnterHandler;
            public CancelEventHandler ValidatingHandler;
            public EventHandler ValidatedHandler;
            public EventHandler TextChangedHandler;
            public EventHandler CheckedChangedHandler;
            public EventHandler SelectedValueChangedHandler;
            public EventHandler ValueChangedHandler;
            public EventHandler ComboTextChangedHandler;
        }
        private readonly List<Map> _maps = new List<Map>(32);
        private readonly Dictionary<string, Map> _byProp = new Dictionary<string, Map>(StringComparer.Ordinal);
        private readonly Dictionary<Control, Map> _byControl = new Dictionary<Control, Map>();
        // =============== Map discovery ===============
        private void BuildMapFromDesignerBindings()
        {
            var toRemove = new List<Tuple<Control, Binding>>();
            var keepbs = false;
            var allpropertydelegates = TrackedEntity<T>.GetAllPropertyDelegates();

            foreach (var ctl in EnumerateControls(_root))
            {
                if (ctl.DataBindings.Count == 0) continue;
                foreach (Binding b in ctl.DataBindings)
                {
                    // Only bindings wired to our BindingSource instance
                    if (!ReferenceEquals(b.DataSource, _bindingSource))
                        continue;

                    var propName = b.BindingMemberInfo.BindingField;
                    Map map;
                    if (string.IsNullOrEmpty(propName))
                    {
                        var cpi = ctl.GetType().GetProperty(b.PropertyName, BindingFlags.Instance | BindingFlags.Public);
                        if (cpi != null && cpi.CanWrite && cpi.PropertyType.IsAssignableFrom(typeof(T)))
                        {
                            map = new Map
                            {
                                Control = ctl,
                                ControlProp = b.PropertyName,
                                EntityProp = null,
                                EntityPropType = typeof(T),
                                IsObjectBinding = true,
                                ControlPropertyInfo = cpi,
                                OriginalBackColor = ctl.BackColor
                            };
                        }
                        else
                        {
                            // I believe this is the only branch where we are not removing the binding
                            // if it is not hit, the bindingsource will have no bindings
                            keepbs = true;
                            continue;
                        }
                    }
                    else
                    {
                        PropertyDelegateInfo<T> baseInfo;
                        if (!allpropertydelegates.TryGetValue(propName, out baseInfo))
                            continue; // not a tracked column

                        map = new Map
                        {
                            Control = ctl,
                            ControlProp = b.PropertyName, // Text / Checked / Value / SelectedValue
                            EntityProp = propName,
                            EntityPropType = baseInfo.PropertyType,
                            Optional = baseInfo.Optional,
                            Ignore = baseInfo.Ignore,
                            Concurrency = baseInfo.Concurrency,
                            DirtyAwareEnabled = baseInfo.DirtyAwareEnabled,
                            Getter = baseInfo.Getter,
                            PublicSetter = baseInfo.PublicSetter,
                            Setter = baseInfo.Setter,
                            OriginalBackColor = ctl.BackColor
                        };

                        _byProp[propName] = map;      // one map per property
                    }

                    _maps.Add(map);
                    _byControl[ctl] = map;
                    // Stop runtime auto-push; we'll own it
                    b.DataSourceUpdateMode = DataSourceUpdateMode.Never;
                    if (_clearDesignerBindings)
                        toRemove.Add(Tuple.Create(ctl, b));
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (var pair in toRemove)
                    pair.Item1.DataBindings.Remove(pair.Item2);
                if (!keepbs)
                    _bindingSource = null;
            }
            WireControls();
        }
        private static IEnumerable<Control> EnumerateControls(Control root)
        {
            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (var child in EnumerateControls(c)) yield return child;
            }
        }
        // Commit policy
        public enum CommitMode { OnCommit /*Leave/Validated*/, Debounced /*future*/, Immediate }
        public CommitMode UpdateCommitMode { get; set; } = CommitMode.OnCommit;
        public string DateTimeDisplayFormat { get; set; } = "yyyy-MM-dd HH:mm";
        public string DateTimeEditFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fffffff";

        // External model update policy
        public ExternalUpdatePolicy ModelUpdatePolicy { get; set; } = ExternalUpdatePolicy.QueueWhileEditing;
        // Optional visual cues
        public Color DirtyBackColor { get; set; } = Color.Empty;    // applied when DirtyPending or DirtyCommitted
        public Color PendingBackColor { get; set; } = Color.Empty;  // applied when EditPending

        // ===================================================================================================================
        // Current entity + tracker
        private T _entity;
        private TrackedEntity<T> _tracked;
        private readonly HashSet<Control> _pendingControls = new HashSet<Control>(); // EditPending controls
        public bool HasEditPending => _pendingControls.Count > 0;
        public bool IsDirtyCommitted => _tracked != null && _tracked.State == TrackedState.Modified; 
        private void CalculateDirtyPending(out bool dirty, out bool pending)
        {
            dirty = true;
            pending = false;

            var dirtyprops = _tracked.DirtyProperties;
            if (dirtyprops.Count == 0) dirty = false;

            var pendingmaps = _maps.Where(x => x.HasPendingParsed).ToList();
            if (pendingmaps.Count > 0) pending = true;

            if (!dirty) return;
            if (dirtyprops.Count != pendingmaps.Count) return;

            //special case:  if the model is dirty, but the pending parsed updates would undo the dirtiness
            //then they cancel each other out and we can show as clean
            foreach (var m in pendingmaps)
            {
                if (dirtyprops.TryGetValue(m.EntityProp, out var dirtyval))
                {
                    if (!DBEngine.ValueEquals(dirtyval.OldValue, m.PendingParsedValue, m.EntityPropType))
                        return;
                }
                else
                    return;
            }
            dirty = false;
            pending = false;
        }
        public event EventHandler<DirtyStateChangedEventArgs> DirtyStateChanged;
        private void RaiseDirtyStateChanged()
        {
            CalculateDirtyPending(out var dirty, out var pending);
            int count = _tracked?.DirtyCount ?? 0;
            int keysHash = _tracked?.DirtyVersion ?? 0; // exact change token
            var snap = new DirtySnapshot(dirty, pending, count, keysHash);

            if (snap.Equals(_lastDirty)) return;          // << no-op, don’t fire

            _lastDirty = snap;

            var args = new DirtyStateChangedEventArgs(dirty, pending, count);
            DirtyStateChanged?.Invoke(this, args);
        }
        private struct DirtySnapshot
        {
            public readonly bool Committed;
            public readonly bool Pending;
            public readonly int DirtyCount;
            public readonly int KeysHash; // coarse membership signal

            public DirtySnapshot(bool committed, bool pending, int count, int keysHash)
            { Committed = committed; Pending = pending; DirtyCount = count; KeysHash = keysHash; }

            public override bool Equals(object obj)
            {
                if (!(obj is DirtySnapshot d)) return false;
                return Committed == d.Committed && Pending == d.Pending &&
                       DirtyCount == d.DirtyCount && KeysHash == d.KeysHash;
            }
            public override int GetHashCode() =>
                ((Committed ? 1 : 0) * 397) ^ ((Pending ? 1 : 0) * 131) ^ (DirtyCount * 17) ^ KeysHash;
        }
        private DirtySnapshot _lastDirty = new DirtySnapshot(false, false, 0, 0);



        /// <summary>
        /// Reset all controls from the current entity (model → UI). Useful for POCOs.
        /// </summary>
        public void ResetBindings()
        {
            if (_entity == null) return;
            foreach (var m in _maps)
            {
                if (m.IsObjectBinding)
                    SetControlValue(m, _entity);
                else
                {
                    SetControlValue(m, m.Getter(_entity));
                    UpdateVisual(m, false);
                }
            }
            RaiseDirtyStateChanged();
        }

        /// <summary>
        /// Commit all pending valid edits into the model (e.g., before Save/Navigate if code bypasses our buttons).
        /// </summary>
        public void CommitAll()
        {
            if (_entity == null) return;
            foreach (var m in _maps)
            {
                CommitIfPending(m);
            }
        }

        /// <summary>Dispose/unwire everything.</summary>
        public void Dispose()
        {
            UnwireButtons();
            UnsubscribeEntity();
            UnwireControls();
        }



        // =============== Control wiring ===============
        private void WireControls()
        {
            foreach (var m in _maps)
            {
                var c = m.Control;

                if (c is TextBoxBase tb1 && IsDateTimeType(m.EntityPropType))
                {
                    m.EnterHandler = (s, e) => { m.InEditSession = true; EnterDateInputMode(m); };
                    c.Enter += m.EnterHandler;
                    m.TextChangedHandler = (s, e) => OnTextLikeChanged(m);
                    c.TextChanged += m.TextChangedHandler;
                    tb1.ReadOnly = !m.PublicSetter;
                }
                else if (c is TextBoxBase tb2)
                {
                    m.TextChangedHandler = (s, e) => OnTextLikeChanged(m);
                    c.TextChanged += m.TextChangedHandler;
                    tb2.ReadOnly = !m.PublicSetter;
                }
                else if (c is ComboBox cbx)
                {
                    var cb = (ComboBox)c;
                    m.SelectedValueChangedHandler = (s, e) => OnComboChanged(m, cb);
                    m.ComboTextChangedHandler = (s, e) => OnComboTextChanged(m, cb);
                    cb.SelectedValueChanged += m.SelectedValueChangedHandler;
                    cb.TextChanged += m.ComboTextChangedHandler;
                    cbx.Enabled = !m.PublicSetter;
                }
                else if (c is CheckBox chk)
                {
                    m.CheckedChangedHandler = (s, e) => OnValueLikeChanged(m, chk.Checked);
                    chk.CheckedChanged += m.CheckedChangedHandler;
                    chk.Enabled = !m.PublicSetter;
                }
                else if (c is RadioButton rb)
                {
                    m.CheckedChangedHandler = (s, e) => OnValueLikeChanged(m, rb.Checked);
                    rb.CheckedChanged += m.CheckedChangedHandler;
                    rb.Enabled = !m.PublicSetter;
                }
                else if (c is DateTimePicker dtp)
                {
                    m.ValueChangedHandler = (s, e) => OnValueLikeChanged(m, dtp.Value);
                    dtp.ValueChanged += m.ValueChangedHandler;
                    dtp.Enabled = !m.PublicSetter;
                }
                else if (c is NumericUpDown nud)
                {
                    m.ValueChangedHandler = (s, e) => OnValueLikeChanged(m, nud.Value);
                    nud.ValueChanged += m.ValueChangedHandler;
                    nud.Enabled = !m.PublicSetter;
                }
                else
                {
                    m.TextChangedHandler = (s, e) => OnTextLikeChanged(m);
                    c.TextChanged += m.TextChangedHandler;
                    c.Enabled = !m.PublicSetter;
                }

                if (m.EnterHandler == null)
                {
                    m.EnterHandler = (s, e) => { m.InEditSession = true; };
                    c.Enter += m.EnterHandler;
                }
                m.ValidatingHandler = (s, e) => OnValidating(m, e);
                c.Validating += m.ValidatingHandler;

                m.ValidatedHandler = (s, e) => OnValidated(m);
                c.Validated += m.ValidatedHandler;
            }
        }
        private void OnValidating(Map m, CancelEventArgs e)
        {
            if (m.Setter == null) return;

            // Source value
            string text = null; object raw = null;
            if (m.Control is TextBoxBase) text = m.Control.Text;
            else if (m.Control is ComboBox cb) raw = (!string.IsNullOrEmpty(cb.ValueMember) && cb.SelectedValue != null) ? cb.SelectedValue : (object)cb.Text;
            else if (m.Control is CheckBox chk) raw = chk.ThreeState && chk.CheckState == CheckState.Indeterminate ? (bool?)null : (object)chk.Checked;
            else if (m.Control is RadioButton rb) raw = (object)rb.Checked;
            else if (m.Control is DateTimePicker dtp) raw = dtp.ShowCheckBox && !dtp.Checked ? null : (object)dtp.Value;
            else if (m.Control is NumericUpDown nud) raw = (object)nud.Value;
            else text = m.Control.Text;

            // Parse
            string err; object parsed;
            if (text != null)
            {
                if (!TryConvert(text, m.EntityPropType, out parsed, out err))
                {
                    SetPending(m, err ?? "Invalid value");
                    e.Cancel = true;            // block leaving
                    return;
                }
            }
            else
            {
                parsed = Coerce(raw, m.EntityPropType);
            }

            // It’s valid. Stage for commit if different. Don’t mutate UI here.
            var current = m.Getter(_entity);
            m.HasStagedValidValue = !DBEngine.ValueEquals(parsed, current, m.EntityPropType);
            m.StagedValidValue = m.HasStagedValidValue ? parsed : null;
        }
        private void OnValidated(Map m)
        {
            if (m.Setter == null) return;
            // successful exit from the field
            m.InEditSession = false;

            // If Validating staged a change, commit once
            if (m.HasStagedValidValue)
            {
                m.Setter(_entity, m.StagedValidValue);
                m.HasStagedValidValue = false;
                m.StagedValidValue = null;
            }

            // apply queued external updates now that edit session ended
            ApplyQueuedExternalIfAny(m);

            // reformat (e.g., DateTime display) AFTER commit
            if (IsDateTimeType(m.EntityPropType))
                FormatDateForDisplay(m);

            //// visuals + dirty signal
            //UpdateVisual(m, false);
            //RaiseDirtyStateChanged();
        }


        private void UnwireControls()
        {
            foreach (var m in _maps)
            {
                var c = m.Control;
                try
                {
                    if (m.EnterHandler != null) c.Enter -= m.EnterHandler;
                    if (m.ValidatedHandler != null) c.Validated -= m.ValidatedHandler;
                    if (m.ValidatingHandler != null) c.Validating -= m.ValidatingHandler;
                    if (m.TextChangedHandler != null) c.TextChanged -= m.TextChangedHandler;
                    if (m.CheckedChangedHandler != null)
                    {
                        var chk = c as CheckBox; if (chk != null) chk.CheckedChanged -= m.CheckedChangedHandler;
                        var rb = c as RadioButton; if (rb != null) rb.CheckedChanged -= m.CheckedChangedHandler;
                    }
                    if (m.SelectedValueChangedHandler != null)
                    {
                        var cb = c as ComboBox; if (cb != null) cb.SelectedValueChanged -= m.SelectedValueChangedHandler;
                    }
                    if (m.ComboTextChangedHandler != null)
                    {
                        var cb = c as ComboBox; if (cb != null) cb.TextChanged -= m.ComboTextChangedHandler;
                    }
                    if (m.ValueChangedHandler != null)
                    {
                        var dtp = c as DateTimePicker; if (dtp != null) dtp.ValueChanged -= m.ValueChangedHandler;
                        var nud = c as NumericUpDown; if (nud != null) nud.ValueChanged -= m.ValueChangedHandler;
                    }
                }
                catch { }
            }
        }

        // =============== TrackedList integration ===============
        private void OnListCurrentChanged(object sender, TrackedEntityChangedEventArgs<T> e)
        {
            LoadFromList(e.CurrentEntity, e.CurrentTracked);
        }
        private bool OnPreparingForNavigation(TrackedEntity<T> entity)
        {
            if (!_root.ValidateChildren(ValidationConstraints.Enabled)) return false;
            return true;
        }

        private void OnListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TrackedList<T>.SaveCommand) || e.PropertyName == nameof(TrackedList<T>.CurrentState))
                RefreshButtonsEnabled();
        }

        private void LoadFromList(T entity, TrackedEntity<T> tracked)
        {
            // Unsubscribe previous
            UnsubscribeEntity();

            _entity = entity;
            _tracked = tracked;

            // Keep BindingSource synced so other controls bound to it also refresh
            // the only way _bindingSource should be null is if we've removed all of it's bindings
            // so there would be nothing to do here
            if (_bindingSource != null)
                _bindingSource.DataSource = _entity;

            if (_entity == null || _tracked == null)
            {
                ResetControlsToDefault();
                RefreshButtonsEnabled();
                return;
            }

            SubscribeEntity();
            ResetBindings(); // model → UI
            RefreshButtonsEnabled();
        }

        // =============== Entity subscription ===============
        private void SubscribeEntity()
        {
            if (_entity == null) return;

            var nobj = _entity as NotifierObject;
            if (nobj != null)
                NotifierObject.PropertyUpdated += OnNotifierObjectPropertyUpdated; // static event; filter by sender
            else
            {
                var inpc = _entity as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged += OnInpcPropertyChanged;
            }

            if (_tracked != null)
            {
                _tracked.TrackedStateChanged += OnTrackedStateChanged;
                _tracked.DirtySetChanged += OnDirtySetChanged;
            }
        }
        private void UnsubscribeEntity()
        {
            if (_entity == null) return;

            var nobj = _entity as NotifierObject;
            if (nobj != null)
                NotifierObject.PropertyUpdated -= OnNotifierObjectPropertyUpdated;
            else
            {
                var inpc = _entity as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged -= OnInpcPropertyChanged;
            }

            if (_tracked != null)
            {
                _tracked.TrackedStateChanged -= OnTrackedStateChanged;
                _tracked.DirtySetChanged -= OnDirtySetChanged;
            }

            _entity = null;
            _tracked = null;
        }

        private void OnTrackedStateChanged(object sender, TrackedState e)
        {
            RefreshButtonsEnabled();
            UpdateAllVisuals();
        }
        private void OnDirtySetChanged(object sender, DirtySetChangedEventArgs e)
        {
            RaiseDirtyStateChanged();
        }

        private void OnNotifierObjectPropertyUpdated(object sender, PropertyChangedWithValuesEventArgs e)
        {
            if (!ReferenceEquals(sender, _entity)) return;

            Map m;
            if (!_byProp.TryGetValue(e.PropertyName, out m)) return;

            OnModelValueChanged(m, e.NewValue);
        }

        private void OnInpcPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, _entity)) return;

            Map m;
            if (!_byProp.TryGetValue(e.PropertyName, out m)) return;

            var val = m.Getter(_entity);
            OnModelValueChanged(m, val);
        }

        private void OnModelValueChanged(Map m, object newValue)
        {
            if (m.InEditSession)
            {
                if (ModelUpdatePolicy == ExternalUpdatePolicy.OverwriteAlways)
                {
                    SetControlValue(m, newValue);
                    m.HasPendingParsed = false; m.PendingParsedValue = null;
                }
                else if (ModelUpdatePolicy == ExternalUpdatePolicy.QueueWhileEditing)
                {
                    m.HasQueuedExternal = true;
                    m.QueuedExternalValue = newValue;
                    SetPending(m, "Updated externally; finish edit to refresh");
                }
                // else Ignore
            }
            else
            {
                SetControlValue(m, newValue);
                m.HasPendingParsed = false; m.PendingParsedValue = null;
            }

            UpdateVisual(m);
        }

        // =============== Control → Model (deferred by default) ===============
        private void OnTextLikeChanged(Map m)
        {
            if (m.ProgrammaticSet) return;
            if (_entity == null) return;
            var text = m.Control.Text;

            string err; object parsed;
            if (!TryConvert(text, m.EntityPropType, out parsed, out err))
            {
                // Invalid/incomplete → EditPending; don’t touch model
                SetPending(m, err ?? "Invalid value");
                m.HasPendingParsed = false; m.PendingParsedValue = null;
                UpdateVisual(m);
                return;
            }

            // Valid value: if equal to current model, clear states; else mark DirtyPending
            ClearPending(m);

            var current = m.Getter(_entity);
            if (DBEngine.ValueEquals(parsed, current, m.EntityPropType))
            {
                m.HasPendingParsed = false; m.PendingParsedValue = null;
            }
            else
            {
                m.HasPendingParsed = true; m.PendingParsedValue = parsed;
                if (UpdateCommitMode == CommitMode.Immediate)
                    CommitIfPending(m);
            }

            UpdateVisual(m);
        }

        private void OnComboChanged(Map m, ComboBox cb)
        {
            if (m.ProgrammaticSet) return;
            object candidate = (!string.IsNullOrEmpty(cb.ValueMember) && cb.SelectedValue != null) ? cb.SelectedValue : (object)cb.Text;
            if (candidate is string)
            {
                OnTextLikeChanged(m);
                return;
            }
            ClearPending(m);

            var coerced = Coerce(candidate, m.EntityPropType);
            var current = m.Getter(_entity);
            if (!DBEngine.ValueEquals(coerced, current, m.EntityPropType))
            {
                m.HasPendingParsed = true; m.PendingParsedValue = coerced;
                if (UpdateCommitMode == CommitMode.Immediate)
                    CommitIfPending(m);
            }
            else
            {
                m.HasPendingParsed = false; m.PendingParsedValue = null;
            }

            UpdateVisual(m);
        }

        private void OnComboTextChanged(Map m, ComboBox cb)
        {
            OnTextLikeChanged(m);
        }

        private void OnValueLikeChanged(Map m, object candidate)
        {
            if (m.ProgrammaticSet) return;
            if (_entity == null) return;
            ClearPending(m);

            var coerced = Coerce(candidate, m.EntityPropType);
            var current = m.Getter(_entity);
            if (!DBEngine.ValueEquals(coerced, current, m.EntityPropType))
            {
                m.HasPendingParsed = true; m.PendingParsedValue = coerced;
                if (UpdateCommitMode == CommitMode.Immediate)
                    CommitIfPending(m);
            }
            else
            {
                m.HasPendingParsed = false; m.PendingParsedValue = null;
            }

            UpdateVisual(m);
        }

        private void CommitIfPending(Map m)
        {
            if (!m.HasPendingParsed) return;
            var current = m.Getter(_entity);
            if (DBEngine.ValueEquals(m.PendingParsedValue, current, m.EntityPropType))
            {
                m.HasPendingParsed = false; m.PendingParsedValue = null;
                UpdateVisual(m);
                return;
            }
            m.Setter(_entity, m.PendingParsedValue);
            m.HasPendingParsed = false; m.PendingParsedValue = null;
            UpdateVisual(m);
        }

        private void ApplyQueuedExternalIfAny(Map m)
        {
            if (!m.HasQueuedExternal) return;
            SetControlValue(m, m.QueuedExternalValue);
            m.HasQueuedExternal = false; m.QueuedExternalValue = null;
            UpdateVisual(m);
        }

        // =============== Model → Control ===============
        private void SetControlValue(Map m, object value)
        {
            Action setAction;
            var c = m.Control;

            if (m.IsObjectBinding)
            {
                // here, "value" is expected to be the entity (we pass _entity at call sites)
                setAction = new Action(() =>
                {
                    m.ControlPropertyInfo.SetValue(c, value, null);
                    ClearPending(m);
                });
            }
            else
            {
                setAction = () =>
                {
                    m.ProgrammaticSet = true;
                    try
                    {
                        if (c is TextBoxBase)
                            c.BackColor = c.BackColor; // preserve color; value set below

                        if (c is TextBoxBase)
                            c.Text = ToControlString(value, m.EntityPropType);
                        else if (c is CheckBox chk)
                        {
                            if (Under(m.EntityPropType) == typeof(bool) && chk.ThreeState)
                            {
                                bool? nb = value == null ? default : Convert.ToBoolean(value);
                                chk.CheckState = nb == null ? CheckState.Indeterminate : (nb.Value ? CheckState.Checked : CheckState.Unchecked);
                            }
                            else
                            {
                                chk.Checked = Convert.ToBoolean(Coerce(value, typeof(bool)));
                            }
                        }
                        else if (c is RadioButton)
                            ((RadioButton)c).Checked = Convert.ToBoolean(Coerce(value, typeof(bool)));
                        else if (c is DateTimePicker dtp)
                        {
                            if (dtp.ShowCheckBox)
                                dtp.Checked = value != null;
                            if (value != null)
                                dtp.Value = (DateTime)Coerce(value, typeof(DateTime));
                            else
                                dtp.Value = default;
                        }                        
                        else if (c is NumericUpDown)
                            ((NumericUpDown)c).Value = Convert.ToDecimal(Coerce(value, typeof(decimal)));
                        else if (c is ComboBox)
                        {
                            var cb = (ComboBox)c;
                            if (!string.IsNullOrEmpty(cb.ValueMember))
                                cb.SelectedValue = value;
                            else
                                cb.Text = ToControlString(value, m.EntityPropType);
                        }
                        else
                        {
                            c.Text = ToControlString(value, m.EntityPropType);
                        }
                    }
                    finally
                    {
                        m.ProgrammaticSet = false;
                        ClearPending(m); // model is authoritative now
                    }
                };
            }

            if (c.IsHandleCreated && c.InvokeRequired)
                c.BeginInvoke(setAction);
            else
                setAction();
        }

        private string ToControlString(object value, Type t)
        {
            if (value == null) return string.Empty;
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            if (nt == typeof(DateTime))
                return ((DateTime)value).ToString(DateTimeDisplayFormat, CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        // =============== Pending/Validation UI ===============
        private void SetPending(Map m, string error)
        {
            _pendingControls.Add(m.Control);
            if (_errors != null) _errors.SetError(m.Control, error);
            UpdateVisual(m);
            RefreshButtonsEnabled();
        }

        private void ClearPending(Map m)
        {
            if (_pendingControls.Remove(m.Control) && _errors != null)
                _errors.SetError(m.Control, "");
            if (_pendingControls.Count == 0 && _errors != null)
                _errors.Clear();
            RefreshButtonsEnabled();
            RaiseDirtyStateChanged();
        }

        private void ResetControlsToDefault()
        {
            foreach (var m in _maps)
            {
                SetControlValue(m, null);
                m.HasPendingParsed = false; m.PendingParsedValue = null;
                m.HasQueuedExternal = false; m.QueuedExternalValue = null;
                m.Control.BackColor = m.OriginalBackColor;
            }
        }

        private void UpdateAllVisuals()
        {
            foreach (var m in _maps.Where(x => x.EntityProp != null)) UpdateVisual(m, false);
            RaiseDirtyStateChanged();
        }

        private void UpdateVisual(Map m, bool withdirty = true)
        {
            var c = m.Control;
            if (c == null) return;

            bool isDirtyCommitted = false;
            if (_tracked != null)
                isDirtyCommitted = _tracked.DirtyProperties.ContainsKey(m.EntityProp);

            if (c is TextBoxBase)
            {
                bool isDirtyPending = m.HasPendingParsed;
                bool isEditPending = _pendingControls.Contains(m.Control);

                if (isEditPending)
                {
                    if (PendingBackColor != Color.Empty)
                        c.BackColor = PendingBackColor;
                    else if (DirtyBackColor != Color.Empty)
                        c.BackColor = DirtyBackColor;
                }
                else if (isDirtyPending || isDirtyCommitted)
                {
                    if (DirtyBackColor != Color.Empty)
                        c.BackColor = DirtyBackColor;
                }
                else
                {
                    c.BackColor = m.OriginalBackColor;
                }
            }
            if (withdirty) RaiseDirtyStateChanged();
        }

        private static Type Under(Type t) { return Nullable.GetUnderlyingType(t) ?? t; }
        // =============== Conversion helpers ===============
        private static object Coerce(object value, Type target)
        {
            if (value == null) return null;
            var t = Under(target);

            if (t.IsEnum)
                return Enum.ToObject(t, Convert.ChangeType(value, Enum.GetUnderlyingType(t), CultureInfo.InvariantCulture));

            if (t == typeof(Guid)) return (value is Guid) ? value : (object)Guid.Parse(Convert.ToString(value, CultureInfo.InvariantCulture));
            if (t == typeof(DateTime)) return (value is DateTime) ? value : (object)DateTime.Parse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            if (t == typeof(TimeSpan)) return (value is TimeSpan) ? value : (object)TimeSpan.Parse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
        }

        private static bool TryConvert(string text, Type target, out object value, out string error)
        {
            value = null; error = null;
            var t = Under(target);

            if (string.IsNullOrWhiteSpace(text) && (t == typeof(string) || Nullable.GetUnderlyingType(target) != null))
            {
                value = null;
                return true;
            }

            if (t == typeof(string))  { value = text; return true; }

            if (t == typeof(DateTime))
            {
                DateTime dt;

                if (Foundation.TryParseDateTime(text, out dt))
                { value = dt; return true; }
                error = "Enter valid datetime"; return false;
            }

            int i; long l; float f; double d; decimal m;
            bool b;
            if (t == typeof(int)) { if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) { value = i; return true; } error = "Invalid integer"; return false; }
            if (t == typeof(long)) { if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out l)) { value = l; return true; } error = "Invalid integer"; return false; }
            if (t == typeof(float)) { if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out f)) { value = f; return true; } error = "Invalid number"; return false; }
            if (t == typeof(double)) { if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) { value = d; return true; } error = "Invalid number"; return false; }
            if (t == typeof(decimal)) { if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out m)) { value = m; return true; } error = "Invalid number"; return false; }
            if (t == typeof(bool)) { if (bool.TryParse(text, out b)) { value = b; return true; } error = "True/False"; return false; }

            if (t.IsEnum)
            {
                try { value = Enum.Parse(t, text, true); return true; }
                catch { error = "Invalid option"; return false; }
            }

            try { value = Convert.ChangeType(text, t, CultureInfo.InvariantCulture); return true; }
            catch { error = "Invalid value"; return false; }
        }
        private static bool IsDateTimeType(Type t) => (Nullable.GetUnderlyingType(t) ?? t) == typeof(DateTime);

        private void EnterDateInputMode(Map m)
        {
            var cur = m.Getter(_entity);
            if (cur == null) return;

            var dt = (DateTime)Coerce(cur, typeof(DateTime));
            m.ProgrammaticSet = true;
            try
            {
                m.Control.Text = dt.ToString(DateTimeEditFormat ?? "yyyy-MM-dd HH:mm:ss.fffffff",
                                             CultureInfo.InvariantCulture);
                if (m.Control is TextBoxBase tb) tb.SelectAll();
            }
            finally
            {
                m.ProgrammaticSet = false;
            }
        }

        // Re-apply display format after committing/refreshing
        private void FormatDateForDisplay(Map m)
        {
            if (!IsDateTimeType(m.EntityPropType)) return;
            SetControlValue(m, m.Getter(_entity)); // uses ToControlString -> display format
        }

        // =============== Buttons / TrackedList integration ===============
        private void WireButtons()
        {
            if (_btnSave != null) _btnSave.Click += BtnSave_Click;
            if (_btnNext != null) _btnNext.Click += BtnNext_Click;
            if (_btnPrev != null) _btnPrev.Click += BtnPrev_Click;
            if (_btnNew != null) _btnNew.Click += BtnNew_Click;

            if (_btnSave != null) _btnSave.CausesValidation = true;
            if (_btnNext != null) _btnNext.CausesValidation = true;
            if (_btnPrev != null) _btnPrev.CausesValidation = true;
            if (_btnNew != null) _btnNew.CausesValidation = true; // or false if 'New' should not block
        }
        private void UnwireButtons()
        {
            if (_btnSave != null) _btnSave.Click -= BtnSave_Click;
            if (_btnNext != null) _btnNext.Click -= BtnNext_Click;
            if (_btnPrev != null) _btnPrev.Click -= BtnPrev_Click;
            if (_btnNew != null) _btnNew.Click -= BtnNew_Click;

            if (_list != null)
            {
                _list.CurrentChanged -= OnListCurrentChanged;
                _list.PropertyChanged -= OnListPropertyChanged;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            CommitAll();
            if (_list != null && _list.SaveCommand != null && _list.SaveCommand.CanExecute(null))
            {
                try { await _list.SaveCommand.ExecuteAsync(null); }
                catch { /* surfaced via TrackedListError */ }
            }
            else if (_tracked != null && _tracked.SaveCommand != null && _tracked.SaveCommand.CanExecute(null))
            {
                try { await ((AsyncDbCommand)_tracked.SaveCommand).ExecuteAsync(null); }
                catch { }
            }
            RefreshButtonsEnabled();
        }
        private void BtnNext_Click(object sender, EventArgs e)
        { CommitAll(); if (_list != null && _list.NextCommand != null && _list.NextCommand.CanExecute(null)) _list.NextCommand.Execute(null); }
        private void BtnPrev_Click(object sender, EventArgs e)
        { CommitAll(); if (_list != null && _list.PreviousCommand != null && _list.PreviousCommand.CanExecute(null)) _list.PreviousCommand.Execute(null); }
        private void BtnNew_Click(object sender, EventArgs e)
        { CommitAll(); OnNewRequested(); }

        public event EventHandler NewRequested; // host can handle creating/adding a new entity
        private void OnNewRequested() { var h = NewRequested; if (h != null) h(this, EventArgs.Empty); }

        private void RefreshButtonsEnabled()
        {
            bool canSave = false;
            if (_list != null && _list.SaveCommand != null)
                canSave = _list.SaveCommand.CanExecute(null);
            else if (_tracked != null && _tracked.SaveCommand != null)
                canSave = _tracked.SaveCommand.CanExecute(null);

            if (_btnSave != null) _btnSave.SynchronizedInvoke(() => _btnSave.Enabled = canSave && !HasEditPending);
            if (_btnNext != null) _btnNext.SynchronizedInvoke(() => _btnNext.Enabled = _list != null && _list.NextCommand != null && _list.NextCommand.CanExecute(null));
            if (_btnPrev != null) _btnPrev.SynchronizedInvoke(() => _btnPrev.Enabled = _list != null && _list.PreviousCommand != null && _list.PreviousCommand.CanExecute(null));
            if (_btnNew != null) _btnNew.SynchronizedInvoke(() => _btnNew.Enabled = true);
        }
    }
    public sealed class DirtyStateChangedEventArgs : EventArgs
    {
        public bool IsDirtyCommitted { get; }
        public bool HasEditPending { get; }
        public int DirtyPropertyCount { get; }
        public DirtyStateChangedEventArgs(bool committed, bool pending, int count)
        { IsDirtyCommitted = committed; HasEditPending = pending; DirtyPropertyCount = count; }
    }
    public enum ExternalUpdatePolicy
    {
        OverwriteAlways,
        QueueWhileEditing, // default: respect user edits; refresh on Leave
        Ignore
    }
}
