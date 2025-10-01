using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MDDDataAccess; // TrackedEntity<T>

namespace FormsDataAccess
{
    /// <summary>
    /// TrackedDataBinder<T>
    /// ---------------------
    /// Runtime-only binder that:
    ///  - Discovers designer-authored bindings for a specific BindingSource
    ///    and builds a control↔property map (constructor).
    ///  - Optionally removes those bindings at runtime so nothing implicit fires.
    ///  - Wires minimal control events to push valid values to the model as the user edits.
    ///  - Keeps controls in sync with model notifications (NotifierObject or INPC).
    ///  - Distinguishes Clean vs EditPending vs DirtyCommitted via the TrackedEntity<T>.
    ///  - Integrates with TrackedList<T> for Next/Prev/New/Save navigation.
    ///
    /// Usage:
    ///   var binder = new TrackedDataBinder<Customer>(this, customerBindingSource, errorProvider1,
    ///                   clearDesignerBindings: true);
    ///   binder.AttachListAndButtons(customerTrackedList, btnSave, btnNext, btnPrev, btnNew);
    ///   // When you have a current entity:
    ///   binder.Load(customerTrackedList.CurrentEntity, customerTrackedList.Current);
    ///
    /// Notes:
    ///  - Only bindings targeting the supplied BindingSource are mapped.
    ///  - Requires TrackedEntity<T>.Initialize to have run for T (your tracker does this).
    ///  - C# 7.3 compatible.
    /// </summary>
    public sealed class TrackedDataBinder<T> : IDisposable where T : class, new()
    {
        private readonly Control _root;
        private readonly BindingSource _bindingSource;
        private readonly ErrorProvider _errors; // optional
        private readonly bool _clearDesignerBindings;

        // Optional navigation integration
        private TrackedList<T> _list;
        private Button _btnSave, _btnNext, _btnPrev, _btnNew;

        // Current entity + tracker
        private T _entity;
        private TrackedEntity<T> _tracked;

        // Map: one per control-binding that targets _bindingSource
        private sealed class Map
        {
            public Control Control;
            public string ControlProp;     // Text / Checked / Value / SelectedValue
            public string EntityProp;      // property name on T
            public Type EntityPropType;

            // State for editing + programmatic updates
            public bool ProgrammaticSet;   // reentrancy guard
            public bool InEditSession;     // true while user is actively editing
            public object QueuedExternalValue; // for conflict handling
            public bool HasQueuedExternal;
        }

        private readonly List<Map> _maps = new List<Map>(32);
        private readonly Dictionary<string, Map> _byProp = new Dictionary<string, Map>(StringComparer.Ordinal);
        private readonly Dictionary<Control, Map> _byControl = new Dictionary<Control, Map>();

        private readonly HashSet<Control> _pending = new HashSet<Control>(); // EditPending controls

        public enum ExternalUpdatePolicy
        {
            /// <summary>Always overwrite control from model, even if user is editing.</summary>
            OverwriteAlways,
            /// <summary>Overwrite only if control is not in edit session; otherwise queue until edit ends (default).</summary>
            QueueWhileEditing,
            /// <summary>Never overwrite automatically (manual ResetBindings required).</summary>
            Ignore
        }

        public ExternalUpdatePolicy ModelUpdatePolicy { get; set; } = ExternalUpdatePolicy.QueueWhileEditing;

        public bool HasEditPending { get { return _pending.Count > 0; } }
        public bool IsDirtyCommitted { get { return _tracked != null && _tracked.State == TrackedState.Modified; } }

        /// <summary>
        /// Build the control↔property map at startup. No entity instance is required yet.
        /// </summary>
        public TrackedDataBinder(Control root, BindingSource bindingSource, ErrorProvider errors = null, bool clearDesignerBindings = true)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (bindingSource == null) throw new ArgumentNullException(nameof(bindingSource));

            _root = root;
            _bindingSource = bindingSource;
            _errors = errors;
            _clearDesignerBindings = clearDesignerBindings;

            BuildMapFromDesignerBindings();
        }

        /// <summary>Optional: connect navigation buttons and TrackedList.</summary>
        public void AttachListAndButtons(TrackedList<T> list, Button btnSave = null, Button btnNext = null, Button btnPrev = null, Button btnNew = null)
        {
            _list = list;
            _btnSave = btnSave; _btnNext = btnNext; _btnPrev = btnPrev; _btnNew = btnNew;

            if (_list != null)
            {
                _list.CurrentChanged += OnListCurrentChanged;
                _list.PropertyChanged += OnListPropertyChanged;
            }

            WireButtons();
            RefreshButtonsEnabled();
        }

        /// <summary>
        /// Load a new current entity + tracker. Unsubscribes from previous instance.
        /// Call with entity == null to unload.
        /// </summary>
        public void Load(T entity, TrackedEntity<T> tracked)
        {
            UnsubscribeEntity();
            _entity = entity;
            _tracked = tracked;
            if (_bindingSource != null) _bindingSource.DataSource = entity;

            if (_entity == null || _tracked == null)
            {
                ResetControlsToDefault();
                RefreshButtonsEnabled();
                return;
            }

            SubscribeEntity();
            ResetBindings(); // push model → controls
            RefreshButtonsEnabled();
        }

        /// <summary>
        /// For POCOs that only implement INPC: force a model→UI refresh.
        /// Safe to call anytime.
        /// </summary>
        public void ResetBindings()
        {
            if (_entity == null)
                return;

            foreach (var m in _maps)
            {
                var val = GetModelValue(m.EntityProp);
                SetControlValue(m, val);
            }
        }

        /// <summary>Dispose/unwire everything.</summary>
        public void Dispose()
        {
            UnwireButtons();
            UnsubscribeEntity();
            UnwireControls();
        }

        // =============== Map discovery ===============
        private void BuildMapFromDesignerBindings()
        {
            // Gather bindings to remove (only those that target _bindingSource)
            var toRemove = new List<Tuple<Control, Binding>>();

            foreach (var ctl in EnumerateControls(_root))
            {
                if (ctl.DataBindings.Count == 0) continue;
                foreach (Binding b in ctl.DataBindings)
                {
                    // Match only bindings wired to our BindingSource instance
                    if (!ReferenceEquals(b.DataSource, _bindingSource))
                        continue;

                    var propName = b.BindingMemberInfo.BindingField;
                    if (string.IsNullOrEmpty(propName)) continue;

                    var pi = typeof(T).GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (pi == null) continue;

                    var map = new Map
                    {
                        Control = ctl,
                        ControlProp = b.PropertyName, // Text / Checked / Value / SelectedValue
                        EntityProp = propName,
                        EntityPropType = pi.PropertyType,
                        ProgrammaticSet = false,
                        InEditSession = false,
                        QueuedExternalValue = null,
                        HasQueuedExternal = false
                    };

                    _maps.Add(map);
                    _byProp[propName] = map;
                    _byControl[ctl] = map; // one prop per control is typical; if multiple, last wins

                    // Stop runtime auto-push; we'll own it
                    b.DataSourceUpdateMode = DataSourceUpdateMode.Never;
                    if (_clearDesignerBindings)
                        toRemove.Add(Tuple.Create(ctl, b));
                }
            }

            // Remove after enumeration
            foreach (var pair in toRemove)
                pair.Item1.DataBindings.Remove(pair.Item2);

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

        // =============== Control wiring ===============
        private void WireControls()
        {
            foreach (var m in _maps)
            {
                var c = m.Control;

                c.Enter += Control_Enter;
                c.Leave += Control_Leave;

                if (c is TextBoxBase)
                    c.TextChanged += (s, e) => OnTextLikeChanged(m);
                else if (c is ComboBox)
                {
                    var cb = (ComboBox)c;
                    cb.SelectedValueChanged += (s, e) => OnComboChanged(m, cb);
                    cb.TextChanged += (s, e) => OnComboTextChanged(m, cb);
                }
                else if (c is CheckBox)
                    ((CheckBox)c).CheckedChanged += (s, e) => OnValueLikeChanged(m, ((CheckBox)c).Checked);
                else if (c is RadioButton)
                    ((RadioButton)c).CheckedChanged += (s, e) => OnValueLikeChanged(m, ((RadioButton)c).Checked);
                else if (c is DateTimePicker)
                    ((DateTimePicker)c).ValueChanged += (s, e) => OnValueLikeChanged(m, ((DateTimePicker)c).Value);
                else if (c is NumericUpDown)
                    ((NumericUpDown)c).ValueChanged += (s, e) => OnValueLikeChanged(m, ((NumericUpDown)c).Value);
                else
                    c.TextChanged += (s, e) => OnTextLikeChanged(m);
            }
        }

        private void UnwireControls()
        {
            foreach (var m in _maps)
            {
                var c = m.Control;
                try
                {
                    c.Enter -= Control_Enter;
                    c.Leave -= Control_Leave;
                }
                catch { }
            }
        }

        private void Control_Enter(object sender, EventArgs e)
        {
            Map m;
            if (_byControl.TryGetValue((Control)sender, out m))
                m.InEditSession = true;
        }

        private void Control_Leave(object sender, EventArgs e)
        {
            Map m;
            if (_byControl.TryGetValue((Control)sender, out m))
            {
                m.InEditSession = false;
                // If an external value was queued while editing, apply now
                if (m.HasQueuedExternal)
                {
                    var queued = m.QueuedExternalValue;
                    m.HasQueuedExternal = false;
                    m.QueuedExternalValue = null;
                    SetControlValue(m, queued);
                }
            }
        }

        // =============== Entity subscription ===============
        private void SubscribeEntity()
        {
            if (_entity == null) return;

            // Prefer NotifierObject with old/new values
            var nobj = _entity as NotifierObject;
            if (nobj != null)
                NotifierObject.PropertyUpdated += OnNotifierObjectPropertyUpdated; // static event; we'll filter by sender
            else
            {
                var inpc = _entity as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged += OnInpcPropertyChanged;
            }

            if (_tracked != null)
                _tracked.TrackedStateChanged += OnTrackedStateChanged;
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
                _tracked.TrackedStateChanged -= OnTrackedStateChanged;

            _entity = null;
            _tracked = null;
        }

        private void OnTrackedStateChanged(object sender, TrackedState e)
        {
            RefreshButtonsEnabled();
        }

        private void OnNotifierObjectPropertyUpdated(object sender, PropertyChangedWithValuesEventArgs e)
        {
            if (!ReferenceEquals(sender, _entity))
                return; // ignore updates for other entities

            Map m;
            if (!_byProp.TryGetValue(e.PropertyName, out m))
                return; // control not bound through this binder

            // External model update arrives
            OnModelValueChanged(m, e.NewValue);
        }

        private void OnInpcPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, _entity))
                return;

            Map m;
            if (!_byProp.TryGetValue(e.PropertyName, out m))
                return;

            var val = GetModelValue(e.PropertyName);
            OnModelValueChanged(m, val);
        }

        private void OnModelValueChanged(Map m, object newValue)
        {
            if (m.InEditSession)
            {
                if (ModelUpdatePolicy == ExternalUpdatePolicy.OverwriteAlways)
                    SetControlValue(m, newValue);
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
            }
        }

        // =============== Control → Model ===============
        private void OnTextLikeChanged(Map m)
        {
            if (_entity == null) return;
            var text = m.Control.Text;

            string err;
            object parsed;
            if (!TryConvert(text, m.EntityPropType, out parsed, out err))
            {
                SetPending(m, err ?? "Invalid value");
                return; // EditPending; do not push
            }

            ClearPending(m);

            var current = GetModelValue(m.EntityProp);
            if (ValueEquals(parsed, current, m.EntityPropType))
                return; // no change

            SetModelValue(m.EntityProp, parsed);
        }

        private void OnComboChanged(Map m, ComboBox cb)
        {
            object candidate;
            if (!string.IsNullOrEmpty(cb.ValueMember) && cb.SelectedValue != null)
                candidate = cb.SelectedValue;
            else
                candidate = cb.Text; // fall back to text

            if (candidate is string)
            {
                OnTextLikeChanged(m); // parse path
                return;
            }

            ClearPending(m);

            var coerced = Coerce(candidate, m.EntityPropType);
            var current = GetModelValue(m.EntityProp);
            if (!ValueEquals(coerced, current, m.EntityPropType))
                SetModelValue(m.EntityProp, coerced);
        }

        private void OnComboTextChanged(Map m, ComboBox cb)
        {
            OnTextLikeChanged(m);
        }

        private void OnValueLikeChanged(Map m, object candidate)
        {
            if (_entity == null) return;
            ClearPending(m);

            var coerced = Coerce(candidate, m.EntityPropType);
            var current = GetModelValue(m.EntityProp);
            if (!ValueEquals(coerced, current, m.EntityPropType))
                SetModelValue(m.EntityProp, coerced);
        }

        // =============== Model → Control ===============
        private void SetControlValue(Map m, object value)
        {
            var c = m.Control;

            Action setAction = () =>
            {
                m.ProgrammaticSet = true;
                try
                {
                    if (c is TextBoxBase)
                        c.Text = ToControlString(value, m.EntityPropType);
                    else if (c is CheckBox)
                        ((CheckBox)c).Checked = Convert.ToBoolean(Coerce(value, typeof(bool)));
                    else if (c is RadioButton)
                        ((RadioButton)c).Checked = Convert.ToBoolean(Coerce(value, typeof(bool)));
                    else if (c is DateTimePicker)
                        ((DateTimePicker)c).Value = (DateTime)Coerce(value, typeof(DateTime));
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
                        // Fallback to Text
                        c.Text = ToControlString(value, m.EntityPropType);
                    }
                }
                finally
                {
                    m.ProgrammaticSet = false;
                    ClearPending(m); // model is authoritative now
                }
            };

            if (c.IsHandleCreated && c.InvokeRequired)
                c.BeginInvoke(setAction);
            else
                setAction();
        }

        private static string ToControlString(object value, Type t)
        {
            if (value == null) return string.Empty;
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            if (nt == typeof(DateTime))
                return ((DateTime)value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        // =============== Pending/Validation UI ===============
        private void SetPending(Map m, string error)
        {
            _pending.Add(m.Control);
            if (_errors != null) _errors.SetError(m.Control, error);
            RefreshButtonsEnabled();
        }

        private void ClearPending(Map m)
        {
            if (_pending.Remove(m.Control) && _errors != null)
                _errors.SetError(m.Control, "");
            if (_pending.Count == 0 && _errors != null)
                _errors.Clear();
            RefreshButtonsEnabled();
        }

        private void ResetControlsToDefault()
        {
            foreach (var m in _maps)
            {
                SetControlValue(m, null);
            }
        }

        // =============== Model value access via compiled delegates ===============
        private object GetModelValue(string prop)
        {
            var pd = TrackedEntity<T>.AllPropertyDelegates[prop];
            T ent = _entity;
            return pd.Getter(ent);
        }

        private void SetModelValue(string prop, object value)
        {
            var pd = TrackedEntity<T>.AllPropertyDelegates[prop];
            T ent = _entity;
            pd.Setter(ent, value);
        }

        // =============== Conversion helpers ===============
        private static object Coerce(object value, Type target)
        {
            if (value == null) return null;
            var t = Nullable.GetUnderlyingType(target) ?? target;

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
            var t = Nullable.GetUnderlyingType(target) ?? target;

            if (t == typeof(string)) { value = text; return true; }

            if (t == typeof(DateTime))
            {
                DateTime dt;
                if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                { value = dt; return true; }
                error = "Enter date as yyyy-MM-dd"; return false;
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

        private static bool ValueEquals(object a, object b, Type t)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            if (nt == typeof(string)) return string.Equals((string)a, (string)b, StringComparison.Ordinal);
            if (nt == typeof(float)) return Math.Abs((float)a - (float)b) < 1e-6f;
            if (nt == typeof(double)) return Math.Abs((double)a - (double)b) < 1e-9;
            if (nt == typeof(decimal)) return (decimal)a == (decimal)b;
            if (nt == typeof(DateTime)) return (DateTime)a == (DateTime)b;
            return Equals(a, b);
        }

        // =============== Buttons / TrackedList integration ===============
        private void WireButtons()
        {
            if (_btnSave != null) _btnSave.Click += BtnSave_Click;
            if (_btnNext != null) _btnNext.Click += BtnNext_Click;
            if (_btnPrev != null) _btnPrev.Click += BtnPrev_Click;
            if (_btnNew != null) _btnNew.Click += BtnNew_Click;
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
            if (_list != null && _list.SaveCommand != null && _list.SaveCommand.CanExecute(null))
            {
                try { await _list.SaveCommand.ExecuteAsync(null); }
                catch { /* surface via TrackedListError */ }
            }
            else if (_tracked != null && _tracked.SaveCommand != null && _tracked.SaveCommand.CanExecute(null))
            {
                try { await ((AsyncDbCommand)_tracked.SaveCommand).ExecuteAsync(null); }
                catch { }
            }
            RefreshButtonsEnabled();
        }
        private void BtnNext_Click(object sender, EventArgs e)
        { if (_list != null && _list.NextCommand != null && _list.NextCommand.CanExecute(null)) _list.NextCommand.Execute(null); }
        private void BtnPrev_Click(object sender, EventArgs e)
        { if (_list != null && _list.PreviousCommand != null && _list.PreviousCommand.CanExecute(null)) _list.PreviousCommand.Execute(null); }
        private void BtnNew_Click(object sender, EventArgs e)
        { OnNewRequested(); }

        public event EventHandler NewRequested; // host can handle creating/adding a new entity
        private void OnNewRequested() { var h = NewRequested; if (h != null) h(this, EventArgs.Empty); }

        private void OnListCurrentChanged(object sender, TrackedEntityChangedEventArgs<T> e)
        {
            Load(e.CurrentEntity, e.CurrentTracked);
        }
        private void OnListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TrackedList<T>.SaveCommand) || e.PropertyName == nameof(TrackedList<T>.CurrentState))
                RefreshButtonsEnabled();
        }

        private void RefreshButtonsEnabled()
        {
            bool canSave = false;
            if (_list != null && _list.SaveCommand != null)
                canSave = _list.SaveCommand.CanExecute(null);
            else if (_tracked != null && _tracked.SaveCommand != null)
                canSave = _tracked.SaveCommand.CanExecute(null);

            if (_btnSave != null) _btnSave.Enabled = canSave && !HasEditPending;
            if (_btnNext != null) _btnNext.Enabled = _list != null && _list.NextCommand != null && _list.NextCommand.CanExecute(null);
            if (_btnPrev != null) _btnPrev.Enabled = _list != null && _list.PreviousCommand != null && _list.PreviousCommand.CanExecute(null);
            if (_btnNew != null) _btnNew.Enabled = true;
        }
    }
}
