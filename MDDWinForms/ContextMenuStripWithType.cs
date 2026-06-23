using MDDFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public class ContextMenuStripWithType<T> : ContextMenuStrip
    {
        public Form OwnerForm { get; set; }

        public static Type LoaderType { get; set; } = null;
        
        private T currentobject;
        public T CurrentObject 
        {
            get
            {
                if (currentobject == null && GetCurrentObject != null)
                    currentobject = GetCurrentObject();
                return currentobject;
            }
            set
            {
                currentobject = value;
            }
        }
        public Func<T> GetCurrentObject;

        public Action<ILoader<T>> DetailsClicked;
        public Action<HandlerBase<T>> HandlerDetailsClicked;
        public Func<T, object, Task<HandlerBase<T>>> NewDetailsHandlerAsync { get; set; }

        public bool Initialized { get; set; } = false;

        private List<ToolStripMenuItemWithContext<ILoader<T>>> detailsitems = new List<ToolStripMenuItemWithContext<ILoader<T>>>();
        private List<ToolStripMenuItemWithContext<HandlerBase<T>>> handlerdetailsitems = new List<ToolStripMenuItemWithContext<HandlerBase<T>>>();
        private ToolStripMenuItem detailsnew = null;
        /// <summary>
        /// Set OwnerForm and CurrentObject if you want to Initialize before showing - Initialize if you want to populate all menu items so you can 
        /// add or insert custom menu items after all the automatic ones are done
        /// </summary>
        public virtual void Initialize()
        {
            if (NewDetailsHandlerAsync != null || LoaderType != null)
            {
                if (!Items.OfType<ToolStripMenuItem>().Any(x => x.Text == "Details <new>"))
                {
                    detailsnew = new ToolStripMenuItem("Details <new>", null, Details_Click);
                    Items.Add(detailsnew);
                }
            }
            int detailsindex = detailsnew == null ? 0 : Items.IndexOf(detailsnew);
            foreach (var item in Items.OfType<ToolStripMenuItemWithContext<HandlerBase<T>>>().Where(x => IsDetailsHandler(x.ContextObject) && !handlerdetailsitems.Contains(x)).ToList())
            {
                Items.Remove(item);
            }
            foreach (var item in handlerdetailsitems.ToList())
            {
                if (!IsAvailableDetailsHandler(item.ContextObject))
                {
                    handlerdetailsitems.Remove(item);
                    Items.Remove(item);
                }
            }
            foreach (var handler in HandlerBase<T>.Instances.Where(IsAvailableDetailsHandler))
            {
                var item = handlerdetailsitems.FirstOrDefault(x => x.ContextObject == handler);
                if (item != null)
                    item.Text = DetailsMenuText(handler.HandlerCaption);
                else
                {
                    item = new ToolStripMenuItemWithContext<HandlerBase<T>>(DetailsMenuText(handler.HandlerCaption), HandlerDetails_Click, handler);
                    handlerdetailsitems.Add(item);
                    Items.Insert(detailsindex, item);
                }
            }
            foreach (var item in detailsitems.ToList())
            {
                if (!Application.OpenForms.OfType<ILoader<T>>().Contains(item.ContextObject))
                {
                    detailsitems.Remove(item);
                    Items.Remove(item);
                }
            }
            foreach (var loader in Application.OpenForms.OfType<ILoader<T>>())
            {
                if (loader.LoaderType == "Details" && loader != OwnerForm)
                {
                    var item = Items.OfType<ToolStripMenuItemWithContext<ILoader<T>>>().Where(x => x.ContextObject == loader && x.Text.StartsWith(loader.LoaderType)).FirstOrDefault();
                    if (item != null)
                        item.Text = DetailsMenuText(loader.LoaderCaption);
                    else
                    {
                        item = new ToolStripMenuItemWithContext<ILoader<T>>(DetailsMenuText(loader.LoaderCaption), Details_Click, loader);
                        detailsitems.Add(item);
                        Items.Insert(detailsindex, item);
                    }
                }
            }
            if (tmsRefreshClicked != null && !Items.OfType<ToolStripMenuItem>().Any(x => x.Name == "tsmRefresh"))
                Items.Add(new ToolStripMenuItem(tmsRefreshText, null, tmsRefreshClicked, "tsmRefresh"));
            Initialized = true;
        }
        protected override void OnOpening(CancelEventArgs e)
        {
            base.OnOpening(e);
            if (OwnerForm == null && this.SourceControl.TopLevelControl is Form)
                OwnerForm = (Form)this.SourceControl.TopLevelControl;
            Initialize();

            foreach (var handler in HandlerBase<T>.Instances)
            {
                if (IsDetailsHandler(handler))
                    continue;
                var item = Items.OfType<ToolStripMenuItemWithContext<HandlerBase<T>>>().Where(x => x.ContextObject == handler).FirstOrDefault();
                if (item == null)
                {
                    if (handler.IsActive && handler.ValidFor(CurrentObject))
                    {
                        item = new ToolStripMenuItemWithContext<HandlerBase<T>>(handler.HandlerCaption, async (s, e2) =>
                        {
                            var tsm = (ToolStripMenuItemWithContext<HandlerBase<T>>)s;
                            await tsm.ContextObject.HandleAsync(CurrentObject);
                            DetailsClicked?.Invoke(handler);
                        }, handler);
                        Items.Add(item);                        
                    }
                }
                else
                {
                    if (!handler.IsActive || !handler.ValidFor(CurrentObject))
                        Items.Remove(item);
                }
            }
        }
        public virtual async void Details_Click(object sender, EventArgs e)
        {
            if (sender == detailsnew && NewDetailsHandlerAsync != null)
            {
                var handler = await NewDetailsHandlerAsync(CurrentObject, null);
                if (handler != null)
                {
                    if (handler.ReferencingObject == null)
                        handler.ReferencingObject = OwnerForm;
                    HandlerDetailsClicked?.Invoke(handler);
                    DetailsClicked?.Invoke(handler);
                }
                ContextActionTaken?.Invoke(sender, CurrentObject);
                return;
            }

            var tsm = sender as ToolStripMenuItemWithContext<ILoader<T>>;
            if (tsm?.ContextObject == null)
            {
                var frm = NewDetailForm();
                await frm.LoadItemAsync(CurrentObject);
                frm.ReferencingObject = OwnerForm;
                DetailsClicked?.Invoke(frm);
            }
            else
            {
                await tsm.ContextObject.LoadItemAsync(CurrentObject);
                //tsm.ContextObject.LoadItem(CurrentObject);
                DetailsClicked?.Invoke(tsm.ContextObject);
            }
            ContextActionTaken?.Invoke(tsm, CurrentObject);
        }

        public virtual async void HandlerDetails_Click(object sender, EventArgs e)
        {
            var tsm = (ToolStripMenuItemWithContext<HandlerBase<T>>)sender;
            await tsm.ContextObject.HandleAsync(CurrentObject);
            HandlerDetailsClicked?.Invoke(tsm.ContextObject);
            DetailsClicked?.Invoke(tsm.ContextObject);
            ContextActionTaken?.Invoke(tsm, CurrentObject);
        }

        private bool IsAvailableDetailsHandler(HandlerBase<T> handler)
        {
            return handler != null
                && IsDetailsHandler(handler)
                && handler.IsActive
                && handler.ValidFor(CurrentObject);
        }

        private bool IsDetailsHandler(HandlerBase<T> handler)
        {
            return handler != null && handler.HandlerType == "Details";
        }

        private string DetailsMenuText(string caption)
        {
            return string.IsNullOrWhiteSpace(caption) ? "Details" : $"Details -> {caption}";
        }

        public static ILoader<T> NewDetailForm()
        {
            var currenthandlers = HandlerBase<T>.Instances.ToList();
            var c = LoaderType.GetConstructor(new Type[] { });
            var frm = c.Invoke(new object[] { }) as Form;
            frm.ShowInstance();
            if (frm is ILoader<T> ilt)
                return ilt;
            var newhandler = HandlerBase<T>.Instances.Except(currenthandlers).FirstOrDefault();
            return newhandler;
        }

        public Func<Task> RefreshAction { get; set; }

        public event EventHandler tmsRefreshClicked;
        public Action<object,T> ContextActionTaken;
        public string tmsRefreshText { get; set; } = "Refresh Grid";

    }
}
