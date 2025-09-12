using MDDFoundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using SortOrder = System.Windows.Forms.SortOrder;

namespace MDDWinForms
{
    public partial class ctlFolderCompare : UserControl
    {
        public ctlFolderCompare()
        {
            InitializeComponent();
            bsTasks.DataSource = monitortasks;
            lbxTasks.DisplayMember = "DisplayText";
        }
        private CustomConfiguration Config = null;
        public ctlFolderCompare(CustomConfiguration config, List<string> folders1, List<string> folders2) : this()
        {
            Config = config;

            var bs1 = new BindingSource();
            var bl1 = new BindingList<string>(folders1);
            bs1.DataSource = bl1;
            cbxFolder1.DataSource = bs1;

            var bs2 = new BindingSource();
            var bl2 = new BindingList<string>(folders2);
            bs2.DataSource = bl2;
            cbxFolder2.DataSource = bs2;
        }
        private Dictionary<int,ListInfo> lists = new Dictionary<int,ListInfo>();
        private DisplayContentEnum displayContent = DisplayContentEnum.None;
        private BindingList<MonitorTask> monitortasks = new BindingList<MonitorTask>();
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        public DisplayContentEnum DisplayContent
        {
            get { return displayContent; }
            set
            {
                if (displayContent != value)
                {
                    displayContent = value;
                    OnDisplayContentChanged();
                }
            }
        }
        private void OnDisplayContentChanged()
        {
            if (!lists.TryGetValue(1, out ListInfo li1))
                li1 = InitListInfo(1);
            if (!lists.TryGetValue(2, out ListInfo li2))
                li2 = InitListInfo(2);
            if (DisplayContent != DisplayContentEnum.None)
            {
                if (li1.FCList == null) UpdateList(1, cbxFolder1.Text, false);
                if (li2.FCList == null) UpdateList(2, cbxFolder2.Text, false);
            }
            switch (DisplayContent)
            {
                case DisplayContentEnum.None:
                    bsResult.DataSource = null;
                    break;
                case DisplayContentEnum.All:
                    var fd1 = li1.FCList.FirstOrDefault()?.Directory;
                    var fd2 = li2.FCList.FirstOrDefault()?.Directory;
                    var u = li1.FCList.Concat(li2.FCList)
                        .GroupBy(x => x.Name, (name, fg) => new ComparisonResult 
                        { 
                            File1 = fg.Where(f => f.Directory == fd1).FirstOrDefault(), 
                            File2 = fg.Where(f => f.Directory == fd2).FirstOrDefault() 
                        }, StringComparer.OrdinalIgnoreCase);
                    bsResult.DataSource = u;
                    break;
                case DisplayContentEnum.Matching:
                    var m = li1.FCList
                        .Join(li2.FCList,
                                l1 => l1.Name,
                                f2 => f2.Name,
                                (l1, f2) => new ComparisonResult
                                {
                                    File1 = l1,
                                    File2 = f2,
                                },
                                StringComparer.OrdinalIgnoreCase);
                    bsResult.DataSource = m;
                    break;
                case DisplayContentEnum.Folder1:
                    var f1results = li1.FCList
                                .GroupJoin( li2.FCList, 
                                            file1 => file1.Name, 
                                            file2 => file2.Name, 
                                            (file1, matches) => new { File1 = file1, Matches = matches }, StringComparer.OrdinalIgnoreCase)
                                .SelectMany(
                                    x => x.Matches.DefaultIfEmpty(),
                                    (f1f1, f1f2) => new ComparisonResult
                                    {
                                        File1 = f1f1.File1,
                                        File2 = f1f2

                                    })
                                .Where(x => x.Length2 == 0 || x.LastWrite1 != x.LastWrite2 || x.Length1 != x.Length2);
                    bsResult.DataSource = f1results;
                    break;
                case DisplayContentEnum.Folder2:
                    var f2results = li2.FCList
                        .GroupJoin(li1.FCList,
                                    file2 => file2.Name,
                                    file1 => file1.Name,
                                    (file2, matches) => new { File1 = file2, Matches = matches }, StringComparer.OrdinalIgnoreCase)
                        .SelectMany(
                            x => x.Matches.DefaultIfEmpty(),
                            (f1f1, f1f2) => new ComparisonResult
                            {
                                File1 = f1f2,
                                File2 = f1f1.File1
                            })
                        .Where(x => x.Length1 == 0 || x.LastWrite1 != x.LastWrite2 || x.Length1 != x.Length2);
                    bsResult.DataSource = f2results;
                    break;
                default:
                    break;
            }
        }
        private ListInfo InitListInfo(int index)
        {
            ListInfo li = new ListInfo();
            li.Status = Controls[$"txtStatus{index}"] as TextBox;
            lists[index] = li;
            return li;
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (int.TryParse((sender as Button).Name.Substring(7), out int index))
                UpdateList(index, (this.Controls[$"cbxFolder{index}"] as ComboBox).Text, true);
        }
        private Tuple<DirectoryInfo,string> ParseFolderSpec(string f)
        {
            DirectoryInfo di;
            string filter = null;
            if (string.IsNullOrWhiteSpace(f)) return null;
            if (f.Contains("*."))
            {
                di = new DirectoryInfo(f.Substring(0, f.LastIndexOf("\\")));
                filter = f.Substring(f.LastIndexOf("\\") + 1);
            }
            else
            {
                di = new DirectoryInfo(f);
            }
            if (di.Exists) 
                return new Tuple<DirectoryInfo, string>(di, filter);
            else
                return null;
        }
        private void UpdateList(int index, string folder, bool reset)
        {
            if (!lists.TryGetValue(index, out ListInfo listinfo))
                listinfo = InitListInfo(index);

            if (folder == null)
            {
                listinfo.FCList = null;
                listinfo.Updated = DateTime.MinValue;
                listinfo.Folder = null;
                listinfo.Status.Text = string.Empty;
            }
            else if (folder.StartsWith("server", StringComparison.OrdinalIgnoreCase))
            {
                var parts = folder.Split(';');
                var connstr = new SqlConnectionStringBuilder();
                string cmdtext = null;
                foreach (var part in parts)
                {
                    if (!string.IsNullOrWhiteSpace(part))
                    {
                        if (part.StartsWith("server", StringComparison.OrdinalIgnoreCase))
                            connstr.DataSource = part.Substring(part.IndexOf('=')+1).Trim();
                        if (part.StartsWith("database", StringComparison.OrdinalIgnoreCase))
                            connstr.InitialCatalog = part.Substring(part.IndexOf("=")+1).Trim();
                        if (part.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                            cmdtext = part;
                    }
                }
                connstr.ApplicationName = "Database Utilities";
                connstr.IntegratedSecurity = true;
                using (SqlConnection cn = new SqlConnection(connstr.ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdtext,cn))
                    {
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            listinfo.FCList = new List<FCFileInfo>();
                            listinfo.Updated = DateTime.Now;
                            listinfo.Folder = folder;   
                            while (rdr.Read())
                            {
                                var fi = new FCFileInfo();
                                fi.Name = rdr["FileName"].ToString();
                                fi.Length = long.Parse(rdr["FileSizeBytes"].ToString());
                                listinfo.FCList.Add(fi);
                            }
                            listinfo.Status.Text = $"{listinfo.FCList.Count} {listinfo.Updated:t}";
                        }
                    }
                }
            }
            {
                var fs = ParseFolderSpec(folder);
                if (fs != null)
                {
                    listinfo.FCList = fs.Item1.GetFiles(fs.Item2 ?? "*.*").Select(x => FCFileInfo.FromFileInfo(x)).ToList();
                    listinfo.Updated = DateTime.Now;
                    listinfo.Folder = folder;
                    listinfo.Status.Text = $"{listinfo.FCList.Count} {listinfo.Updated:t}";
                }
                else
                {
                    reset = true;
                }
            }
            if (reset || folder == null)
            {
                DisplayContent = DisplayContentEnum.None;
                rbFolder1.Checked = false;
                rbFolder2.Checked = false;
                rbMatching.Checked = false;
                rbAll.Checked = false;
            }
        }
        private void cbxFolder_Validating(object sender, CancelEventArgs e)
        {
            var cbx = sender as ComboBox;
            if (cbx != null)
            {
                var txt = cbx.Text;
                if (int.TryParse(cbx.Name.Substring(9), out int index) && lists.TryGetValue(index, out ListInfo li))
                {
                    if (txt != li.Folder)
                    {
                        UpdateList(index, null, true);
                    }
                }
                var ds = cbx.DataSource as BindingSource;
                if (ds != null && !ds.Contains(txt))
                {
                    if (ParseFolderSpec(txt) != null)
                    {
                        ds.Insert(0, txt);
                        if (Config != null)
                            Config.Save();
                        cbx.SelectedItem = txt;
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            //dgvResult.SortOrder = SortOrder.Descending;

            //var c = dgvResult.Columns[0];

            //dgvResult.Sort(new dgvLengthComparer());

            //var lst = bsResult.DataSource as SortableBindingList<ComparisonResult>;
            //lst = new SortableBindingList<ComparisonResult>(lst.OrderByDescending(x => x, new CRLengthComparer()).ToList());
            //bsResult.DataSource = lst;
        }
        private void dgvResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var rw = dgvResult.Rows[e.RowIndex].DataBoundItem as ComparisonResult;
            if (rw != null)
            {
                if (rw.Length1 == 0 || rw.Length2 == 0)
                    e.CellStyle.BackColor = Color.White;
                else if (rw.Length1 != rw.Length2)
                    e.CellStyle.BackColor = Color.Red;
                else if (rw.LastWrite1 != rw.LastWrite2)
                    e.CellStyle.BackColor = Color.Yellow;
                else
                    e.CellStyle.BackColor = Color.White;
            }
        }
        private List<ComparisonResult> contextitems;
        private void dgvResult_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dgvResult.SelectedRows.Count > 0 && dgvResult.Rows[e.RowIndex].Selected)
                    contextitems = dgvResult.SelectedRows.OfType<DataGridViewRow>().Select(x => x.DataBoundItem as ComparisonResult).ToList();
                else
                {
                    contextitems = new List<ComparisonResult> { dgvResult.Rows[e.RowIndex].DataBoundItem as ComparisonResult };
                }
                if (contextitems != null && contextitems.Count > 0)
                {
                    var cms = new ContextMenuStrip();
                    cms.Items.Add(new ToolStripMenuItem("Compare Hashes", null, async (s, eh) => await CompareHashes()));
                    cms.Items.Add(new ToolStripMenuItem("Copy 1 -> 2", null, async (s, eh) => await CopyFiles(1)));
                    cms.Items.Add(new ToolStripMenuItem("Copy 2 -> 1", null, async (s, eh) => await CopyFiles(2)));
                    cms.Items.Add(new ToolStripMenuItem("Delete 1", null, (s, eh) => DeleteFiles(1)));
                    cms.Items.Add(new ToolStripMenuItem("Delete 2", null, (s, eh) => DeleteFiles(2)));
                    e.ContextMenuStrip = cms;
                }
            }
        }

        private void DeleteFiles(int sourcefolderindex)
        {
            var list = contextitems.ToList();
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                if (sourcefolderindex == 1 && item.File1 != null)
                {
                    sb.AppendLine($@"del ""{item.File1.FileInfo.FullName}""");
                }
                if (sourcefolderindex == 2 && item.File2 != null)
                {
                    sb.AppendLine($@"del ""{item.File2.FileInfo.FullName}""");
                }
            }
            sb.AppendLine("PAUSE");
            Clipboard.SetText(sb.ToString());
        }

        private async Task CopyFiles(int sourcefolderindex)
        {
            var list = contextitems.ToList();
            var taskq = new Queue<ComparisonResult>();
            foreach (var item in list)
            {
                taskq.Enqueue(item);
            }
            txtQItems.Text = $"{taskq.Count}";
            var tasks = new List<Tuple<ComparisonResult, Task>>();

            while(tasks.Count > 0 || taskq.Count > 0)
            {
                if (tasks.Count < 2 && taskq.Count > 0)
                {
                    var item = taskq.Dequeue();
                    txtQItems.SynchronizedInvoke(() => txtQItems.Text = $"{taskq.Count}");
                    if (sourcefolderindex == 1 && item.File1 != null)
                    {
                        tasks.Add(new Tuple<ComparisonResult, Task>(item, item.File1.FileInfo.CopyToAsync(new FileInfo(Path.Combine(lists[2].Folder, item.File1.Name)), true, tokenSource.Token, false, (x) => TaskCallBack(x), TimeSpan.FromMilliseconds(200), (x) => AllowTaskToStart(x))));
                        //txtOutput.AppendText($"{DateTime.Now:T}: Started copying {item.File1.Name} 1 -> 2\r\n");
                    }
                    if (sourcefolderindex == 2 && item.File2 != null)
                    {
                        tasks.Add(new Tuple<ComparisonResult, Task>(item, item.File2.FileInfo.CopyToAsync(new FileInfo(Path.Combine(lists[1].Folder, item.File2.Name)), true, tokenSource.Token, false, (x) => TaskCallBack(x), TimeSpan.FromMilliseconds(200), (x) => AllowTaskToStart(x))));
                        //txtOutput.AppendText($"{DateTime.Now:T}: Started copying {item.File2.Name} 2 -> 1\r\n");
                    }
                }
                if (tasks.Count >= 2)
                {
                    var t = await Task.WhenAny(tasks.Select(x => x.Item2)).ConfigureAwait(false);
                    var tp = tasks.FirstOrDefault(x => x.Item2 == t);
                    if (sourcefolderindex == 1)
                    {
                        var fi = new FileInfo(Path.Combine(lists[2].Folder, tp.Item1.File1.Name));
                        if (fi.Exists)
                            tp.Item1.File2 = FCFileInfo.FromFileInfo(fi);
                    }
                    if (sourcefolderindex == 2)
                    {
                        var fi = new FileInfo(Path.Combine(lists[1].Folder, tp.Item1.File2.Name));
                        if (fi.Exists)
                            tp.Item1.File1 = FCFileInfo.FromFileInfo(fi);
                    }
                    dgvResult.SynchronizedInvoke(() =>
                    {
                        bsResult.ResetBindings(false);
                        //txtOutput.AppendText($"{DateTime.Now:T}: Finished copying {tp.Item1.Name}\r\n");
                    });
                    tasks.Remove(tp);
                }
                await Task.Delay(10).ConfigureAwait(false);
            }




            //foreach (var item in list)
            //{
            //    if (sourcefolderindex == 1 && item.File1 != null)
            //    {
            //        tasks.Add(new Tuple<ComparisonResult, Task>(item, item.File1.FileInfo.CopyToAsync(new FileInfo(Path.Combine(lists[2].Folder, item.File1.Name)), true, tokenSource.Token, false, (x) => TaskCallBack(x), TimeSpan.FromMilliseconds(200), (x) => AllowTaskToStart(x))));
            //        //txtOutput.AppendText($"{DateTime.Now:T}: Started copying {item.File1.Name} 1 -> 2\r\n");
            //    }
            //    if (sourcefolderindex == 2 && item.File2 != null)
            //    {
            //        tasks.Add(new Tuple<ComparisonResult, Task>(item, item.File2.FileInfo.CopyToAsync(new FileInfo(Path.Combine(lists[1].Folder, item.File2.Name)), true, tokenSource.Token, false, (x) => TaskCallBack(x), TimeSpan.FromMilliseconds(200), (x) => AllowTaskToStart(x))));
            //        //txtOutput.AppendText($"{DateTime.Now:T}: Started copying {item.File2.Name} 2 -> 1\r\n");
            //    }
            //}
            //while (tasks.Count > 0)
            //{
            //    var t = await Task.WhenAny(tasks.Select(x => x.Item2)).ConfigureAwait(false);
            //    var tp = tasks.FirstOrDefault(x => x.Item2 == t);
            //    if (sourcefolderindex == 1)
            //    {
            //        var fi = new FileInfo(Path.Combine(lists[2].Folder, tp.Item1.File1.Name));
            //        if (fi.Exists)
            //            tp.Item1.File2 = FCFileInfo.FromFileInfo(fi);
            //    }
            //    if (sourcefolderindex == 2)
            //    {
            //        var fi = new FileInfo(Path.Combine(lists[1].Folder, tp.Item1.File2.Name));
            //        if (fi.Exists)
            //            tp.Item1.File1 = FCFileInfo.FromFileInfo(fi);
            //    }
            //    dgvResult.SynchronizedInvoke(() =>
            //    {
            //        bsResult.ResetBindings(false);
            //        //txtOutput.AppendText($"{DateTime.Now:T}: Finished copying {tp.Item1.Name}\r\n");
            //    });
            //    tasks.Remove(tp);
            //}

        }
        private async Task CompareHashes()
        {
            var list = contextitems.ToList();
            var tasks = new List<Tuple<FCFileInfo, Task<byte[]>>>();
            foreach (var item in list)
            {
                if (item.HashesMatch == "ComputeHashes" || item.HashesMatch.StartsWith("NotApplicable"))
                {
                    if (item.File1 != null && item.File1.Hash == null)
                    {
                        tasks.Add(new Tuple<FCFileInfo, Task<byte[]>>(item.File1, Foundation.ReadFileHashAsync(item.File1.FileInfo, tokenSource.Token, (x) => TaskCallBack(x), TimeSpan.FromMilliseconds(200), (x) => AllowTaskToStart(x))));
                        //txtOutput.AppendText($"{DateTime.Now:T}: Started reading hash for {item.File1.Name}\r\n");
                    }
                    if (item.File2 != null && item.File2.Hash == null)
                    {
                        tasks.Add(new Tuple<FCFileInfo, Task<byte[]>>(item.File2, Foundation.ReadFileHashAsync(item.File2.FileInfo, tokenSource.Token, (x) => TaskCallBack(x), TimeSpan.FromMilliseconds(200), (x) => AllowTaskToStart(x))));
                        //txtOutput.AppendText($"{DateTime.Now:T}: Started reading hash for {item.File1.Name}\r\n");
                    }
                }
            }
            while (tasks.Count > 0)
            {
                var t = await Task.WhenAny(tasks.Select(x => x.Item2)).ConfigureAwait(false);
                var tp = tasks.FirstOrDefault(x => x.Item2 == t);
                if (!tp.Item2.IsCanceled && !tp.Item2.IsFaulted)
                    tp.Item1.Hash = tp.Item2.Result;
                dgvResult.SynchronizedInvoke(() =>
                {
                    bsResult.ResetBindings(false);
                    //txtOutput.AppendText($"{DateTime.Now:T}: Finished reading hash for {tp.Item1.Name}\r\n");
                });
                tasks.Remove(tp);
            }
        }
        private int runningtasks = 0;
        private bool AllowTaskToStart(FileCopyProgress progress)
        {
            bool retval = false;

            if (runningtasks < 2)
            {
                if (progress != null)
                {
                    progress.Queued = false;
                    lbxTasks.SynchronizedInvoke(() =>
                    {
                        if (!monitortasks.Any(x => x.Progress == progress))
                            monitortasks.Add(new MonitorTask { Progress = progress });
                        txtOutput.AppendText($"{DateTime.Now:T}: Started {progress.OperationComplete} for {progress.FileName}\r\n");
                    });
                }
                runningtasks += 1;
                retval = true;
            }

            //lbxTasks.SynchronizedInvoke(() =>
            //{
            //    if (monitortasks.Where(x => !x.Progress.Queued).Count() < 2)
            //    {
            //        if (progress != null)
            //        {
            //            progress.Queued = false;
            //            if (!monitortasks.Any(x => x.Progress == progress))
            //                monitortasks.Add(new MonitorTask { Progress = progress });
            //            txtOutput.AppendText($"{DateTime.Now:T}: Started {progress.OperationComplete} for {progress.FileName}\r\n");
            //        }
            //        retval = true;
            //    }
            //});
            return retval;
        }
        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null && rb.Checked) DisplayContent = (DisplayContentEnum) Enum.Parse(typeof(DisplayContentEnum), rb.Name.Substring(2));
        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cbx = cmsCombobox.SourceControl as ComboBox;
            if (cbx != null)
            {
                var ds = cbx.DataSource as BindingSource;
                var txt = cbx.Text;
                if (ds.Contains(txt))
                {
                    ds.Remove(txt);
                    if (Config != null)
                        Config.Save();
                }
            }
        }
        private void dgvResult_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (bsResult.DataSource is IEnumerable<ComparisonResult> list)
            {
                var column = dgvResult.Columns[e.ColumnIndex];
                SortOrder direction = SortOrder.Descending;
                if (column.HeaderCell.SortGlyphDirection == SortOrder.None || column.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                    direction = SortOrder.Ascending;



                switch (column.DataPropertyName)
                {
                    case "Name":
                        bsResult.DataSource = direction == SortOrder.Ascending ? list.OrderBy(x => x.Name) : list.OrderByDescending(x => x.Name);
                        column.HeaderCell.SortGlyphDirection = direction;
                        break;
                    case "Length1":
                    case "Length2":
                        bsResult.DataSource = direction == SortOrder.Ascending ? list.OrderBy(x => x, new CRLengthComparer()) : list.OrderByDescending(x => x, new CRLengthComparer());
                        foreach (DataGridViewColumn col in dgvResult.Columns)
                        {
                            if (col.DataPropertyName.StartsWith("Length"))
                            {
                                col.HeaderCell.SortGlyphDirection = direction;
                            }
                        }
                        break;
                    case "LastWrite1":
                    case "LastWrite2":
                        bsResult.DataSource = direction == SortOrder.Ascending ? list.OrderBy(x => x, new CRLastWriteComparer()) : list.OrderByDescending(x => x, new CRLastWriteComparer());
                        foreach (DataGridViewColumn col in dgvResult.Columns)
                        {
                            if (col.DataPropertyName.StartsWith("LastWrite"))
                            {
                                col.HeaderCell.SortGlyphDirection = direction;
                            }
                        }
                        break;
                    case "HashesMatch":
                        bsResult.DataSource = direction == SortOrder.Ascending ? list.OrderBy(x => x.HashesMatch) : list.OrderByDescending(x => x.HashesMatch);
                        column.HeaderCell.SortGlyphDirection = direction;
                        break;
                    default:
                        break;
                }


                //var l = ((dgvResult.DataSource as BindingSource).DataSource as IEnumerable<ComparisonResult>).ToList();
                //bsResult.DataSource = l.OrderByDescending(x => x.Name);
            }

        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (tokenSource != null) { tokenSource.Cancel(); }
        }
        private void tmrMain_Tick(object sender, EventArgs e)
        {
            lbxTasks.SynchronizedInvoke(() =>
            {
                if (btnCancel.Enabled != monitortasks.Count > 0) btnCancel.Enabled = !btnCancel.Enabled;
            });
        }
        private void TaskCallBack(FileCopyProgress progress)
        {
            lbxTasks.SynchronizedInvoke(() =>
            {
                var mt = monitortasks.Where(x => x.Progress == progress).FirstOrDefault();  
                if (mt == null)
                    monitortasks.Add(new MonitorTask { Progress = progress });
                else if (progress.PercentComplete == 1 || progress.Cancelled)
                {
                    runningtasks -= 1;
                    monitortasks.Remove(mt);
                    txtOutput.AppendText($"{DateTime.Now:T}: {progress}\r\n");
                }
                else
                {
                    bsTasks.ResetBindings(false);
                }
            });
        }
    }
    public class ComparisonResult 
    {
        public string Name { get => File1?.Name ?? File2?.Name; }
        public string Path1 { get => File1?.Directory; }
        public string Path2 { get => File2?.Directory; }
        public long Length1 { get => File1?.Length ?? 0; }
        public long Length2 { get => File2?.Length ?? 0; }
        public DateTime LastWrite1 { get => File1?.LastWriteTime ?? DateTime.MinValue; }
        public DateTime LastWrite2 { get => File2?.LastWriteTime ?? DateTime.MinValue; }
        public string HashesMatch
        {
            get
            {
                if (File1 == null || File2 == null)
                {
                    if (File1?.Hash != null)
                        return "NotApplicable(h1)";
                    if (File2?.Hash != null)
                        return "NotApplicable(h2)";
                    return "NotApplicable";
                }
                if (File1.Hash == null || File2.Hash == null) return "ComputeHashes";
                if (File1.Hash.SequenceEqual(File2.Hash))
                    return "Match";
                else
                    return "NoMatch";
            }
        }
        public FCFileInfo File1 { get; set; }
        public FCFileInfo File2 { get; set; }
    }
    public class FCFileInfo
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public long Length { get; set; }
        public DateTime LastWriteTime { get; set; }
        public FileInfo FileInfo
        {
            get
            {
                if (Directory == null)
                {
                    return new FileInfo(Name);
                }
                else
                {
                    return new FileInfo(Path.Combine(Directory, Name));
                }
            }
        }
        public byte[] Hash { get; set; }
        public static FCFileInfo FromFileInfo(FileInfo fi)
        {
            DateTime lastWrite = fi.LastWriteTime;
            if (lastWrite.Millisecond != 0 && ((fi.Attributes & FileAttributes.ReadOnly) == 0))
            {
                // Truncate milliseconds
                DateTime truncated = new DateTime(
                    lastWrite.Year, lastWrite.Month, lastWrite.Day,
                    lastWrite.Hour, lastWrite.Minute, lastWrite.Second,
                    0, lastWrite.Kind);

                // Write back to file
                System.IO.File.SetLastWriteTime(fi.FullName, truncated);
                lastWrite = truncated;

                if (fi.CreationTime.Millisecond != 0)
                {
                    DateTime created = fi.CreationTime;
                    DateTime truncatedCreated = new DateTime(
                        created.Year, created.Month, created.Day,
                        created.Hour, created.Minute, created.Second,
                        0, created.Kind);
                    System.IO.File.SetCreationTime(fi.FullName, truncatedCreated);
                }
            }



            return new FCFileInfo
            {
                Name = fi.Name,
                Directory = fi.DirectoryName,
                Length = fi.Length,
                LastWriteTime = lastWrite
            };
        }
    }
    public enum DisplayContentEnum
    {
        None, All, Matching, Folder1, Folder2
    }
    public class ListInfo
    {
        public List<FCFileInfo> FCList { get; set; }
        public DateTime Updated { get; set; }
        public string Folder { get; set; }
        public TextBox Status { get; set; }
    }
    public class CRLengthComparer : IComparer<ComparisonResult>
    {
        public int Compare(ComparisonResult x, ComparisonResult y)
        {
            return (x.Length1 == 0 ? x.Length2 : x.Length1).CompareTo(y.Length1 == 0 ? y.Length2 : y.Length1);
        }
    }
    public class CRLastWriteComparer : IComparer<ComparisonResult>
    {
        public int Compare(ComparisonResult x, ComparisonResult y)
        {
            return (x.LastWrite1 == DateTime.MinValue ? x.LastWrite2 : x.LastWrite1).CompareTo(y.LastWrite1 == DateTime.MinValue ? y.LastWrite2 : y.LastWrite1);
        }
    }
    public class MonitorTask
    {
        public FileCopyProgress Progress { get; set; }
        public string DisplayText { get => Progress.ToString(); }
    }
}
