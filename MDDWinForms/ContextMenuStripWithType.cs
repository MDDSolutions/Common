﻿using MDDFoundation;
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

        public bool Initialized { get; set; } = false;

        private List<ToolStripMenuItemWithContext<ILoader<T>>> detailsitems = new List<ToolStripMenuItemWithContext<ILoader<T>>>();
        private ToolStripMenuItemWithContext<ILoader<T>> detailsnew = null;
        /// <summary>
        /// Set OwnerForm and CurrentObject if you want to Initialize before showing - Initialize if you want to populate all menu items so you can 
        /// add or insert custom menu items after all the automatic ones are done
        /// </summary>
        public virtual void Initialize()
        {
            if (LoaderType != null)
            {
                if (!Items.OfType<ToolStripMenuItemWithContext<ILoader<T>>>().Any(x => x.Text == "Details <new>"))
                {
                    detailsnew = new ToolStripMenuItemWithContext<ILoader<T>>("Details <new>", Details_Click);
                    Items.Add(detailsnew);
                }
            }
            int detailsindex = detailsnew == null ? 0 : Items.IndexOf(detailsnew);
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
                        item.Text = $"{loader.LoaderType} {loader.LoaderCaption}";
                    else
                    {
                        item = new ToolStripMenuItemWithContext<ILoader<T>>($"{loader.LoaderType} {loader.LoaderCaption}", Details_Click, loader);
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
            if (!Initialized) Initialize();
        }
        public virtual async void Details_Click(object sender, EventArgs e)
        {
            var tsm = (ToolStripMenuItemWithContext<ILoader<T>>)sender;
            if (tsm.ContextObject == null)
            {
                var c = LoaderType.GetConstructor(new Type[] { });
                var frm = c.Invoke(new object[] { });
                (frm as Form).ShowInstance();
                await (frm as ILoader<T>).LoadItemAsync(CurrentObject);
                (frm as ILoader<T>).ReferencingObject = OwnerForm;
                DetailsClicked?.Invoke((frm as ILoader<T>));
            }
            else
            {
                await tsm.ContextObject.LoadItemAsync(CurrentObject);
                //tsm.ContextObject.LoadItem(CurrentObject);
                DetailsClicked?.Invoke(tsm.ContextObject);
            }
            ContextActionTaken?.Invoke(tsm, CurrentObject);
        }
        public Action RefreshAction { get; set; }

        public event EventHandler tmsRefreshClicked;
        public Action<object,T> ContextActionTaken;
        public string tmsRefreshText { get; set; } = "Refresh Grid";

    }
}
