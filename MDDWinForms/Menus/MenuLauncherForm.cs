using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDDFoundation.Menus;
using MenuItem = MDDFoundation.Menus.MenuItem;

namespace MDDWinForms.Menus
{
    /// <summary>Reusable launcher. The host chooses application lifetime by its ApplicationContext.</summary>
    public class MenuLauncherForm : Form, IWorkspaceWindow
    {
        private readonly IMenuProvider provider;
        private readonly IMenuDispatcher dispatcher;
        private readonly string applicationKey;
        private readonly IMenuUserStore userStore;
        private MenuUserPreferences state;
        private bool savingFavourite;
        private readonly TextBox search = new TextBox { Dock = DockStyle.Fill };
        private readonly TreeView categories = new TreeView { Dock = DockStyle.Fill, HideSelection = false };
        private readonly ListView results = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, MultiSelect = false, HideSelection = false };
        private readonly Label status = new Label { Dock = DockStyle.Bottom, Height = 28, TextAlign = ContentAlignment.MiddleLeft };
        private readonly ContextMenuStrip itemMenu = new ContextMenuStrip();
        private readonly Panel menuPage = new Panel { Dock = DockStyle.Fill };
        private TabControl pages;
        private readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        private IReadOnlyList<MenuItem> items = new List<MenuItem>();
        private bool loading;
        private bool disposed;

        public MenuLauncherForm(IMenuProvider provider, IMenuDispatcher dispatcher, string applicationKey, IMenuUserStore userStore = null)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.applicationKey = applicationKey ?? throw new ArgumentNullException(nameof(applicationKey));
            this.userStore = userStore;
            Text = applicationKey;
            Size = new Size(960, 650);
            MinimumSize = new Size(640, 420);
            Font = new Font("Segoe UI", 10);
            KeyPreview = true;
            var header = new TableLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(6), ColumnCount = 3 };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            var reload = new Button { Text = "Reload", Dock = DockStyle.Fill };
            header.Controls.Add(new Label { Text = "Search", AutoSize = true }, 0, 0);
            header.Controls.Add(search, 1, 0);
            header.Controls.Add(reload, 2, 0);
            var split = new SplitContainer { Dock = DockStyle.Fill, Size = new Size(900, 520), SplitterDistance = 210 };
            split.Panel1.Controls.Add(categories);
            split.Panel2.Controls.Add(results);
            results.Columns.Add("Menu item", 260);
            results.Columns.Add("Category", 150);
            results.Columns.Add("Description", 340);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
            var open = new Button { Text = "Open", AutoSize = true };
            var newInstance = new Button { Text = "Open new instance", AutoSize = true, Enabled = false };
            var favorite = new Button { Text = "Toggle favorite", AutoSize = true, Enabled = false };
            actions.Controls.AddRange(new Control[] { open, newInstance, favorite });
            menuPage.Controls.Add(split);
            menuPage.Controls.Add(actions);
            menuPage.Controls.Add(status);
            menuPage.Controls.Add(header);
            Controls.Add(menuPage);
            search.TextChanged += (s, e) => RefreshResults();
            categories.AfterSelect += (s, e) => RefreshResults();
            reload.Click += async (s, e) => await ReloadAsync();
            open.Click += (s, e) => Launch(false);
            newInstance.Click += (s, e) => Launch(true);
            favorite.Click += (s, e) => ToggleFavorite();
            results.SelectedIndexChanged += (s, e) => {
                newInstance.Enabled = SelectedItem?.AllowNewInstance == true;
                favorite.Enabled = SelectedItem != null && state != null;
            };
            itemMenu.Items.Add("Open/Activate", null, (s, e) => Launch(false));
            var contextOpenNew = itemMenu.Items.Add("Open New", null, (s, e) => Launch(true));
            itemMenu.Opening += (s, e) => {
                e.Cancel = SelectedItem == null;
                contextOpenNew.Visible = SelectedItem?.AllowNewInstance == true;
            };
            // Selection is separate from activation; right-click targets the row under the pointer.
            results.MouseDoubleClick += (s, e) => {
                var row = results.GetItemAt(e.X, e.Y);
                if (e.Button != MouseButtons.Left || row == null) return;
                row.Selected = true;
                Launch(false);
            };
            results.MouseUp += (s, e) => {
                if (e.Button != MouseButtons.Right) return;
                var row = results.GetItemAt(e.X, e.Y);
                if (row == null) return;
                row.Selected = true;
                row.Focused = true;
                itemMenu.Show(results, e.Location);
            };
            results.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) { Launch(e.Control); e.Handled = true; e.SuppressKeyPress = true; }
                else if (e.KeyCode == Keys.Apps || (e.Shift && e.KeyCode == Keys.F10))
                {
                    if (results.SelectedItems.Count > 0)
                    {
                        var bounds = results.SelectedItems[0].Bounds;
                        itemMenu.Show(results, new Point(bounds.Left, bounds.Bottom));
                    }
                    e.Handled = true; e.SuppressKeyPress = true;
                }
            };
            search.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Down && results.Items.Count > 0) { results.Focus(); results.Items[0].Selected = true; e.SuppressKeyPress = true; }
                if (e.KeyCode == Keys.Enter) { if (results.SelectedItems.Count == 0 && results.Items.Count > 0) results.Items[0].Selected = true; Launch(e.Control); e.SuppressKeyPress = true; }
            };
            Shown += async (s, e) => await ReloadAsync();
        }
        /// <summary>Adds an application-supplied tab beside the menu. The launcher stays generic: it
        /// hosts whatever control the application hands it and knows nothing about the contents.
        /// The menu occupies its own tab as soon as the first page is added.</summary>
        public TabPage AddPage(string title, Control content)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("A tab title is required.", nameof(title));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (pages == null)
            {
                pages = new TabControl { Dock = DockStyle.Fill };
                Controls.Remove(menuPage);
                var menuTab = new TabPage(MenuPageTitle);
                menuTab.Controls.Add(menuPage);
                pages.TabPages.Add(menuTab);
                Controls.Add(pages);
            }
            var page = new TabPage(title) { AutoScroll = true };
            if (content.Dock == DockStyle.None) content.Dock = DockStyle.Fill;
            page.Controls.Add(content);
            pages.TabPages.Add(page);
            return page;
        }
        /// <summary>Caption of the menu's own tab. Only read when the first page is added.</summary>
        public string MenuPageTitle { get; set; } = "Menu";

        // WorkspaceManager captures every open form unless it opts out here. The launcher is the
        // application's own window, so it is not a workspace member - as frmMainMenu was not.
        public bool IgnoreWorkspaceState => true;
        public string GetWorkspaceState() => null;
        public void ApplyWorkspaceState(string state) { }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F)) { search.Focus(); search.SelectAll(); return true; }
            if (keyData == Keys.Escape && search.Text.Length > 0) { search.Clear(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed) { disposed = true; cancellation.Cancel(); cancellation.Dispose(); itemMenu.Dispose(); }
            base.Dispose(disposing);
        }
        public async Task ReloadAsync()
        {
            if (loading || IsDisposed) return;
            loading = true;
            items = new List<MenuItem>(); results.Items.Clear();
            status.Text = "Loading menu...";
            try
            {
                var preferences = userStore == null ? null : await userStore.LoadAsync(applicationKey, cancellation.Token);
                var loaded = await provider.LoadAsync(applicationKey, cancellation.Token);
                if (IsDisposed) return;
                state = preferences;
                items = MenuDefinition.Validate(loaded);
                categories.Nodes.Clear();
                var home = categories.Nodes.Add("Home"); home.Tag = "home";
                if (state != null) { categories.Nodes.Add(new TreeNode("Favorites") { Tag = "favorites" }); categories.Nodes.Add(new TreeNode("Recently used") { Tag = "recent" }); }
                AddCategories(categories.Nodes, null);
                categories.ExpandAll();
                categories.SelectedNode = home;
                RefreshResults();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { if (!IsDisposed) status.Text = "Menu could not be loaded: " + ex.GetBaseException().Message + " — use Reload to retry."; }
            finally { loading = false; }
        }
        private void AddCategories(TreeNodeCollection nodes, int? parent)
        {
            foreach (var item in items.Where(i => i.Kind == MenuItemKind.Category && i.ParentId == parent).OrderBy(i => i.SortOrder).ThenBy(i => i.Title))
            {
                var node = new TreeNode(item.Title) { Tag = item.Id };
                nodes.Add(node);
                AddCategories(node.Nodes, item.Id);
            }
        }
        private MenuItem SelectedItem => results.SelectedItems.Count == 0 ? null : results.SelectedItems[0].Tag as MenuItem;
        private void RefreshResults()
        {
            var selectedId = SelectedItem?.Id;
            var query = items.Where(i => i.Kind == MenuItemKind.Action);
            var term = search.Text.Trim();
            var selection = categories.SelectedNode?.Tag;
            // The browsed category, when one is selected and no search is narrowing the list.
            var scope = term.Length == 0 && selection is int selected ? selected : (int?)null;
            if (term.Length > 0)
                query = query.Where(i => (i.Title + " " + i.Description + " " + i.Keywords + " " + CategoryPath(i, null)).IndexOf(term, StringComparison.CurrentCultureIgnoreCase) >= 0);
            else if (scope.HasValue)
                query = query.Where(i => IsInCategory(i, scope.Value));
            else if (Equals(selection, "favorites")) query = query.Where(i => state.Favorites.Contains(i.Id));
            else if (Equals(selection, "recent")) query = query.Where(i => state.LastUsed.ContainsKey(i.Id));
            var ordered = term.Length == 0 && (Equals(selection, "recent") || Equals(selection, "home"))
                ? query.OrderByDescending(i => Equals(selection, "home") && state?.Favorites.Contains(i.Id) == true).ThenBy(i => Equals(selection, "home") ? FavoritePosition(i) : int.MaxValue).ThenByDescending(i => state != null && state.LastUsed.TryGetValue(i.Id, out var used) ? used : DateTime.MinValue).ThenBy(i => i.SortOrder).ThenBy(i => i.Title)
                : scope.HasValue
                ? query.OrderBy(i => OrderInCategory(i, scope.Value)).ThenBy(i => i.SortOrder).ThenBy(i => i.Title)
                : query.OrderBy(i => term.Length == 0 && Equals(selection, "favorites") ? FavoritePosition(i) : int.MaxValue).ThenBy(i => i.SortOrder).ThenBy(i => i.Title);
            results.BeginUpdate();
            results.Items.Clear();
            foreach (var item in ordered)
            {
                var row = new ListViewItem((state?.Favorites.Contains(item.Id) == true ? "★ " : "") + item.Title) { Tag = item };
                row.SubItems.Add(CategoryPath(item, scope)); row.SubItems.Add(item.Description ?? "");
                results.Items.Add(row); row.Selected = item.Id == selectedId;
            }
            results.EndUpdate();
            status.Text = results.Items.Count == 0 ? "No matching menu items." : $"{results.Items.Count} items • Double-click/Enter: open • Right-click: options • Ctrl+Enter: new • Ctrl+F: search";
        }
        private int FavoritePosition(MenuItem item) => state != null && state.Favorites.Contains(item.Id) && state.FavoriteOrder.TryGetValue(item.Id, out var order) ? order : int.MaxValue;
        private MenuItem Category(int id) => items.FirstOrDefault(i => i.Id == id && i.Kind == MenuItemKind.Category);
        // An action is shown under a category it is placed in, and under that category's ancestors.
        private bool IsInCategory(MenuItem item, int category) =>
            item.Categories.Any(c => c.CategoryId == category || DescendsFrom(c.CategoryId, category));
        private bool DescendsFrom(int categoryId, int ancestorId)
        {
            var category = Category(categoryId);
            while (category?.ParentId != null)
            {
                if (category.ParentId.Value == ancestorId) return true;
                category = Category(category.ParentId.Value);
            }
            return false;
        }
        private MenuCategoryRef PlacementUnder(MenuItem item, int category) =>
            item.Categories.FirstOrDefault(c => c.CategoryId == category || DescendsFrom(c.CategoryId, category));
        // Order within a category comes from that placement; the item's own SortOrder is the fallback.
        private int OrderInCategory(MenuItem item, int category) => PlacementUnder(item, category)?.SortOrder ?? item.SortOrder;
        private string PathOf(int categoryId)
        {
            var names = new List<string>();
            for (var category = Category(categoryId); category != null; category = category.ParentId.HasValue ? Category(category.ParentId.Value) : null)
                names.Insert(0, category.Title);
            return string.Join(" / ", names);
        }
        // Browsing a category shows that path; everywhere else shows every category the item is in.
        private string CategoryPath(MenuItem item, int? scope)
        {
            if (item.Categories.Count == 0) return "";
            if (scope.HasValue)
            {
                var placement = PlacementUnder(item, scope.Value);
                if (placement != null) return PathOf(placement.CategoryId);
            }
            return string.Join("; ", item.Categories.Select(c => PathOf(c.CategoryId))
                .OrderBy(path => path, StringComparer.CurrentCultureIgnoreCase));
        }
        private async void Launch(bool forceNew)
        {
            var item = SelectedItem;
            if (item == null || (forceNew && !item.AllowNewInstance)) return;
            var mode = forceNew ? MenuLaunchMode.CreateNew : item.DefaultLaunchMode;
            try { dispatcher.Launch(item, mode); }
            catch (Exception ex) { MessageBox.Show(this, ex.GetBaseException().Message, "Unable to open " + item.Title, MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (userStore == null || state == null) return;
            try
            {
                var used = await userStore.RecordUsageAsync(applicationKey, item.Id, mode, cancellation.Token);
                if (IsDisposed) return;
                if (!state.LastUsed.TryGetValue(item.Id, out var previous) || previous < used) state.LastUsed[item.Id] = used;
                RefreshResults();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { if (!IsDisposed) status.Text = "Opened, but usage could not be recorded: " + ex.GetBaseException().Message; }
        }
        private async void ToggleFavorite()
        {
            var item = SelectedItem;
            if (item == null || state == null || userStore == null || savingFavourite || loading) return;
            savingFavourite = true;
            try
            {
                await userStore.SetFavouriteAsync(applicationKey, item.Id, !state.Favorites.Contains(item.Id), null, cancellation.Token);
                var preferences = await userStore.LoadAsync(applicationKey, cancellation.Token);
                if (IsDisposed) return;
                state = preferences;
                RefreshResults();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { if (!IsDisposed) status.Text = "Favourites could not be refreshed/saved: " + ex.GetBaseException().Message; }
            finally { savingFavourite = false; }
        }
    }
}