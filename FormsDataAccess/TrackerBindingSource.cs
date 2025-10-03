using MDDDataAccess;
using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace FormsDataAccess
{
    public class TrackerBindingSource<T> where T : class, new()
    {
        private readonly BindingSource bindingSource;
        private TrackedEntity<T> trackedEntity;
        private int previousPosition = -1;
        private Tracker<T> tracker = null;
        private readonly BindingList<T> bindinglist = new BindingList<T>();
        public TrackerBindingSource(BindingSource baseSource, DBEngine engine)
        {
            bindingSource = baseSource ?? throw new ArgumentNullException(nameof(baseSource));
            bindingSource.DataSource = bindinglist;
            bindingSource.CurrentChanged += OnCurrentChanged;
            bindingSource.PositionChanged += OnPositionChanged;
            tracker = engine.GetTracker<T>();
            NextCommand = new RelayCommand(Next, CanMoveNext);
            PreviousCommand = new RelayCommand(Previous, CanMovePrevious);

        }
        private void Next() { if (bindingSource.Position < bindinglist.Count - 1) UpdateCurrentRow(bindingSource.Position + 1); }
        private bool CanMoveNext() => bindingSource.Position < bindinglist.Count - 1;
        private void Previous() { if (bindingSource.Position > 0) UpdateCurrentRow(bindingSource.Position - 1); }
        private bool CanMovePrevious() => bindingSource.Position > 0;
        private void OnPositionChanged(object sender, EventArgs e)
        {
            if (bindingSource.Position != previousPosition)
            {
                bool cancel = false;
                if (PositionChanging != null)
                {
                    T entityfrom = null;
                    TrackedState statefrom = TrackedState.Invalid;
                    if (previousPosition != -1)
                    {
                        entityfrom = bindingSource[previousPosition] as T;
                        statefrom = trackedEntity.State;
                    }

                    var args = new PositionChangingEventArgs<T>(entityfrom, statefrom, previousPosition, bindingSource.Current as T, bindingSource.Position);
                    PositionChanging(this, args);
                    cancel = args.Cancel;
                    if (cancel)
                    {
                        bindingSource.Position = previousPosition;
                    }
                }
                if (!cancel)
                {
                    if (trackedEntity != null) trackedEntity.TrackedStateChanged -= TrackedEntity_TrackedStateChanged;
                    var newvp = bindingSource.Current as T;
                    trackedEntity = tracker.GetOrAdd(ref newvp);
                    if (!ReferenceEquals(newvp, bindingSource.Current))
                        throw new InvalidOperationException("Mismatch");
                    trackedEntity.TrackedStateChanged += TrackedEntity_TrackedStateChanged;
                }
            }
            previousPosition = bindingSource.Position;
        }

        private void TrackedEntity_TrackedStateChanged(object sender, TrackedState e)
        {
            TrackedStateChanged?.Invoke(this, e);
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            CurrentChanged?.Invoke(this, bindingSource.Current as T);
        }
        public event EventHandler<T> CurrentChanged;
        public event EventHandler<PositionChangingEventArgs<T>> PositionChanging;
        public event EventHandler<TrackedState> TrackedStateChanged;
        public TrackedState EntityState => trackedEntity != null ? trackedEntity.State : TrackedState.Invalid;
        //public object DataSource { get => bindingSource.DataSource; set => bindingSource.DataSource = value; }
        public T Current => bindingSource.Current as T;
        public int Position => bindingSource.Position;
        private bool UpdateCurrentRow(int newindex)
        {
            throw new NotImplementedException();
        }
        public bool SetCurrent(T entity)
        {
            int currentIndex = bindingSource.Position;

            bool done = false;

            var incomingid = TrackedEntity<T>.GetKeyValue(entity);

            // If the next entry is the same performer, just advance
            if (currentIndex + 1 < bindinglist.Count &&
                Foundation.ValueEquals(TrackedEntity<T>.GetKeyValue(bindinglist[currentIndex + 1]), incomingid))
            {
                bindingSource.Position = currentIndex + 1;
                done = true;
            }

            if (!done && currentIndex >= 1 &&
                    Foundation.ValueEquals(TrackedEntity<T>.GetKeyValue(bindinglist[currentIndex - 1]), incomingid))
            {
                bindingSource.Position = currentIndex - 1;
                done = true;
            }


            // Otherwise, truncate future and add new performer
            if (!done)
            {
                if (currentIndex < bindinglist.Count - 1)
                {
                    for (int i = bindinglist.Count - 1; i >= currentIndex + 1; i--)
                    {
                        bindinglist.RemoveAt(i);
                    }
                }
                bindinglist.Add(entity);
                //if (bsVideoPerformer.Position == -1) bsVideoPerformer.ResetBindings(false);
                bindingSource.Position = bindinglist.Count - 1;
            }
            //btnBack.Enabled = bindingSource.Position > 0;
            //btnForward.Enabled = bindingSource.Position < bindinglist.Count - 1;
            (NextCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PreviousCommand as RelayCommand)?.RaiseCanExecuteChanged();
            return true;
        }
        public ICommand NextCommand { get; private set; }
        public ICommand PreviousCommand { get; private set; }
    }
    public class PositionChangingEventArgs<T> : EventArgs
    {
        public T EntityFrom { get; }
        public TrackedState StateFrom { get; }
        public int PositionFrom { get; }
        public T EntityTo { get; }
        public int PositionTo { get; set; }
        public bool Cancel { get; set; }
        public PositionChangingEventArgs(T entityfrom, TrackedState statefrom, int positionfrom, T entityto, int positionto)
        {
            EntityFrom = entityfrom;
            StateFrom = statefrom;
            PositionFrom = positionfrom;
            EntityTo = entityto;
            PositionTo = positionto;
            Cancel = false;
        }
    }
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;
        private event EventHandler canExecuteChanged;
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => canExecute == null || canExecute();
        public void Execute(object parameter) => execute();
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    canExecuteChanged += value;
                }
            }
            remove
            {
                if (canExecute != null)
                {
                    canExecuteChanged -= value;
                }
            }
        }
        public void RaiseCanExecuteChanged()
        {
            canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
