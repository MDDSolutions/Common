using MDDFoundation;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
using System.Threading.Channels;

namespace MDDWinForms
{
    public partial class ctlFolderCompare : UserControl
    {
        private const int DefaultMaxConcurrentOperations = 2;
        //private readonly SemaphoreSlim _operationSemaphore;
        //private readonly ConcurrentQueue<FileOperation> _operationQueue;
        private int _maxConcurrentOperations = DefaultMaxConcurrentOperations;
        //private Task _queueProcessorTask;
        private int _queuedItemCount = 0;


        private Channel<FileOperation> _opChannel;
        //private Task[] _workers;
        private CancellationTokenSource _queueCts;
        private readonly ConcurrentDictionary<string, byte> _dedupe = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        private readonly object _workerGate = new object();
        private readonly List<(Task task, CancellationTokenSource stopCts)> _workerPool = new List<(Task, CancellationTokenSource)>();

        public ctlFolderCompare()
        {
            InitializeComponent();
            bsTasks.DataSource = monitortasks;
            lbxTasks.DisplayMember = "DisplayText";
            //_operationSemaphore = new SemaphoreSlim(_maxConcurrentOperations, _maxConcurrentOperations);
            //_operationQueue = new ConcurrentQueue<FileOperation>();
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

        public int MaxConcurrentOperations
        {
            get => _maxConcurrentOperations;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                _maxConcurrentOperations = value;

                if (_opChannel != null)
                    EnsureWorkers(_maxConcurrentOperations);
            }
        }

        private Dictionary<int, ListInfo> lists = new Dictionary<int, ListInfo>();
        private DisplayContentEnum displayContent = DisplayContentEnum.None;
        private BindingList<MonitorTask> monitortasks = new BindingList<MonitorTask>();
        //private CancellationTokenSource tokenSource = new CancellationTokenSource();
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
                                .GroupJoin(li2.FCList,
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
        private Tuple<DirectoryInfo, string> ParseFolderSpec(string f)
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
                            connstr.DataSource = part.Substring(part.IndexOf('=') + 1).Trim();
                        if (part.StartsWith("database", StringComparison.OrdinalIgnoreCase))
                            connstr.InitialCatalog = part.Substring(part.IndexOf("=") + 1).Trim();
                        if (part.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                            cmdtext = part;
                    }
                }
                connstr.ApplicationName = "Database Utilities";
                connstr.IntegratedSecurity = true;
                using (SqlConnection cn = new SqlConnection(connstr.ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdtext, cn))
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
                    listinfo.FCList = fs.Item1.GetFiles(fs.Item2 ?? "*.*").Select(x => FCFileInfo.FromFileInfo(x, false)).ToList();
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
                    cms.Items.Add(new ToolStripMenuItem("Compare Hashes", null, (s, eh) => CompareHashes()));
                    cms.Items.Add(new ToolStripMenuItem("Copy 1 -> 2", null, (s, eh) => CopyFiles(1)));
                    cms.Items.Add(new ToolStripMenuItem("Copy 2 -> 1", null, (s, eh) => CopyFiles(2)));
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

        private void CopyFiles(int sourcefolderindex)
        {
            var list = contextitems.ToList();
            var addedCount = 0;

            foreach (var item in list)
            {
                if (sourcefolderindex == 1 && item.File1 != null)
                {
                    TryQueueOperation(new CopyFileOperation
                    {
                        SourceFile = item.File1.FileInfo,
                        DestFile = new FileInfo(Path.Combine(lists[2].Folder, item.File1.Name)),
                        ComparisonResult = item,
                        SourceFolderIndex = sourcefolderindex
                    });
                    addedCount++;
                }
                else if (sourcefolderindex == 2 && item.File2 != null)
                {
                    TryQueueOperation(new CopyFileOperation
                    {
                        SourceFile = item.File2.FileInfo,
                        DestFile = new FileInfo(Path.Combine(lists[1].Folder, item.File2.Name)),
                        ComparisonResult = item,
                        SourceFolderIndex = sourcefolderindex
                    });
                    addedCount++;
                }
            }

            //Interlocked.Add(ref _queuedItemCount, addedCount);
            UpdateQueueCount();
            //EnsureQueueProcessorRunning();
        }

        private void CompareHashes()
        {
            var list = contextitems.ToList();
            var addedCount = 0;

            foreach (var item in list)
            {
                if (item.HashesMatch == "ComputeHashes" || item.HashesMatch.StartsWith("NotApplicable"))
                {
                    if (item.File1 != null && item.File1.Hash == null)
                    {
                        TryQueueOperation(new HashFileOperation
                        {
                            FileInfo = item.File1
                        });
                        addedCount++;
                    }
                    if (item.File2 != null && item.File2.Hash == null)
                    {
                        TryQueueOperation(new HashFileOperation
                        {
                            FileInfo = item.File2
                        });
                        addedCount++;
                    }
                }
            }

            //Interlocked.Add(ref _queuedItemCount, addedCount);
            UpdateQueueCount();
            //EnsureQueueProcessorRunning();
        }

        //private void EnsureQueueProcessorRunning()
        //{
        //    if (_queueProcessorTask == null || _queueProcessorTask.IsCompleted)
        //    {
        //        _queueProcessorTask = Task.Run(() => ProcessOperationQueue());
        //    }
        //}

        //private async Task ProcessOperationQueue()
        //{
        //    while (!tokenSource.Token.IsCancellationRequested)
        //    {
        //        if (_operationQueue.TryDequeue(out FileOperation operation))
        //        {
        //            await _operationSemaphore.WaitAsync(tokenSource.Token).ConfigureAwait(false);

        //            var processingTasks = new List<Task>();
        //            processingTasks.Add(ProcessOperation(operation));

        //            // Try to dequeue additional operations up to the concurrency limit
        //            for (int i = 1; i < _maxConcurrentOperations; i++)
        //            {
        //                if (_operationQueue.TryDequeue(out FileOperation additionalOp))
        //                {
        //                    await _operationSemaphore.WaitAsync(tokenSource.Token).ConfigureAwait(false);
        //                    processingTasks.Add(ProcessOperation(additionalOp));
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }

        //            await Task.WhenAll(processingTasks).ConfigureAwait(false);
        //        }
        //        else
        //        {
        //            // Queue is empty, wait a bit before checking again
        //            await Task.Delay(100, tokenSource.Token).ConfigureAwait(false);
        //        }
        //    }
        //}

        private async Task ProcessOperation(FileOperation operation)
        {
            //var progress = new FileCopyProgress
            //{
            //    FileName = operation.FileName,
            //    OperationComplete = operation.OperationType,
            //    Queued = false
            //};

            try
            {
                Interlocked.Decrement(ref _queuedItemCount);
                UpdateQueueCount();

                lbxTasks.SynchronizedInvoke(() =>
                {
                    txtOutput.AppendText($"{DateTime.Now:T}: Started {operation.OperationType} for {operation.FileName}\r\n");
                });

                if (_queueCts == null || _queueCts.Token.IsCancellationRequested)
                {
                    lbxTasks.SynchronizedInvoke(() =>
                    {
                        txtOutput.AppendText($"{DateTime.Now:T}: Operation cancelled before start: {operation.OperationType} for {operation.FileName}\r\n");
                    });
                    return;
                }

                if (operation is CopyFileOperation copyOp)
                {
                    var result = await copyOp.SourceFile.CopyToAsync(
                        destination:copyOp.DestFile, 
                        overwrite: true, 
                        token: _queueCts.Token, 
                        MoveFile: false,
                        progresscallback: TaskCallBack, 
                        progressreportinterval: TimeSpan.FromMilliseconds(200),
                        hashmode: FileCopyHashMode.FastNativeHashWithResumeReread,
                        resumable: true,
                        profile: FileCopyProfile.Auto,
                        maxUsage: 1).ConfigureAwait(false);

                    // Update the comparison result with the new file info

                    if (result.IsCompleted)
                    {
                        copyOp.DestFile.Refresh();
                        if (copyOp.SourceFolderIndex == 1)
                            copyOp.ComparisonResult.File2 = FCFileInfo.FromFileInfo(copyOp.DestFile, false);
                        else
                            copyOp.ComparisonResult.File1 = FCFileInfo.FromFileInfo(copyOp.DestFile, false);
                    }

                    dgvResult.SynchronizedInvoke(() =>
                    {
                        bsResult.ResetBindings(false);
                    });
                }
                else if (operation is HashFileOperation hashOp)
                {
                    var hash = await Foundation.ReadFileHashAsync(hashOp.FileInfo.FileInfo, _queueCts.Token,
                        (p) => TaskCallBack(p), TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);

                    if (!_queueCts.Token.IsCancellationRequested)
                    {
                        hashOp.FileInfo.Hash = hash;

                        dgvResult.SynchronizedInvoke(() =>
                        {
                            bsResult.ResetBindings(false);
                        });
                    }
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                //progress.Cancelled = true;
            }
            catch (Exception ex)
            {
                lbxTasks.SynchronizedInvoke(() =>
                {
                    txtOutput.AppendText($"{DateTime.Now:T}: Error {operation.OperationType} {operation.FileName}: {ex.Message}\r\n");
                });
            }
            finally
            {
                //progress.IsCompleted = true;
                _dedupe.TryRemove(operation.OpKey, out _);
                lbxTasks.SynchronizedInvoke(() =>
                {
                    //var mt = monitortasks.FirstOrDefault(x => x.Progress == progress);
                    //if (mt != null)
                    //{
                    //    monitortasks.Remove(mt);
                    //}
                    txtOutput.AppendText($"{DateTime.Now:T}: Finished {operation.OperationType} for {operation.FileName}\r\n");
                });
                //_operationSemaphore.Release();
            }
        }

        private void UpdateQueueCount()
        {
            txtQItems.SynchronizedInvoke(() => txtQItems.Text = $"{_queuedItemCount}");
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null && rb.Checked) DisplayContent = (DisplayContentEnum)Enum.Parse(typeof(DisplayContentEnum), rb.Name.Substring(2));
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
        private async void btnCancel_Click(object sender, EventArgs e)
        {
            await ResetQueueAsync(cancelInFlight: true);
        }
        private void tmrMain_Tick(object sender, EventArgs e)
        {
            lbxTasks.SynchronizedInvoke(() =>
            {
                if (btnCancel.Enabled != monitortasks.Count > 0) btnCancel.Enabled = !btnCancel.Enabled;
                var now = Environment.TickCount;
                for (int i = monitortasks.Count - 1; i >= 0; i--)
                {
                    if (monitortasks[i].RemoveAt < now)
                    {
                        monitortasks.RemoveAt(i);
                    }
                }
            });
        }
        private void TaskCallBack(FileCopyProgress progress)
        {
            lbxTasks.SynchronizedInvoke(() =>
            {
                var mt = monitortasks.Where(x => x.Progress == progress).FirstOrDefault();
                if (mt == null)
                {
                    if (progress.Cancelled || progress.IsCompleted) return;
                    mt = new MonitorTask { Progress = progress };
                    monitortasks.Add(mt);
                }
                if (progress.Cancelled || progress.IsCompleted)
                {
                    mt.RemoveAt = Environment.TickCount + 10000; // show completed/ cancelled for 10 seconds
                    //monitortasks.Remove(mt);
                }   
                bsTasks.ResetBindings(false);
            });
        }


        private void StartQueue()
        {
            if (_opChannel != null)
                return; // already started

            _queueCts?.Dispose();
            _queueCts = new CancellationTokenSource();

            _opChannel = Channel.CreateUnbounded<FileOperation>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            });

            EnsureWorkers(_maxConcurrentOperations);
        }

        private async Task WorkerLoop(int workerId, CancellationToken retireToken)
        {
            try
            {
                while (true)
                {
                    // Wait for work OR retirement. If retired while idle, exit.
                    if (!await _opChannel.Reader.WaitToReadAsync(retireToken).ConfigureAwait(false))
                        return;

                    // Drain available work
                    while (_opChannel.Reader.TryRead(out var op))
                    {
                        await ProcessOperation(op).ConfigureAwait(false);

                        // If we were asked to retire, we retire AFTER finishing current job.
                        if (retireToken.IsCancellationRequested)
                            return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // normal retirement while idle
            }
        }
        private void EnsureWorkers(int desiredCount)
        {
            if (_opChannel == null) return;

            lock (_workerGate)
            {
                while (_workerPool.Count < desiredCount)
                {
                    var stopCts = new CancellationTokenSource();
                    var idx = _workerPool.Count;
                    var task = Task.Run(() => WorkerLoop(idx, stopCts.Token));
                    _workerPool.Add((task, stopCts));
                }

                while (_workerPool.Count > desiredCount)
                {
                    var last = _workerPool[_workerPool.Count - 1];
                    last.stopCts.Cancel();
                    _workerPool.RemoveAt(_workerPool.Count - 1);
                }
            }
        }
        private bool TryQueueOperation(FileOperation op)
        {
            // If StartQueue hasn’t been called yet, do it lazily.
            if (_opChannel == null) StartQueue();

            var key = op.OpKey;
            if (!_dedupe.TryAdd(key, 0))
            {
                // already queued or processing
                return false;
            }

            var written = _opChannel.Writer.TryWrite(op);
            if (written)
            {
                Interlocked.Increment(ref _queuedItemCount);
                UpdateQueueCount();
            }
            else
            {
                // This should never happen with an unbounded channel, but handle it just in case.
                _dedupe.TryRemove(key, out _);
            }
            return written;
        }
        private async Task ResetQueueAsync(bool cancelInFlight)
        {
            // Snapshot old state so we can shut it down safely
            Channel<FileOperation> oldChannel = _opChannel;
            CancellationTokenSource oldQueueCts = _queueCts;

            List<(Task task, CancellationTokenSource stopCts)> oldWorkers;
            lock (_workerGate)
            {
                oldWorkers = _workerPool.ToList();
                _workerPool.Clear();
            }

            // 1) Stop accepting new work on the old channel (optional but nice)
            try { oldChannel?.Writer.TryComplete(); } catch { }

            // 2) Retire workers (they’ll stop when idle; canceling in-flight ops helps them reach idle)
            foreach (var w in oldWorkers)
            {
                try { w.stopCts.Cancel(); } catch { }
            }

            // 3) Cancel in-flight file ops (copy/hash) if requested
            if (cancelInFlight)
            {
                try { oldQueueCts?.Cancel(); } catch { }
            }

            // 4) Await worker exit (non-blocking UI because we await)
            try
            {
                await Task.WhenAll(oldWorkers.Select(w => w.task)).ConfigureAwait(true);
            }
            catch { /* swallow cancellation and any shutdown noise */ }

            // 5) Dispose old tokens
            try { oldQueueCts?.Dispose(); } catch { }
            foreach (var w in oldWorkers)
            {
                try { w.stopCts.Dispose(); } catch { }
            }

            // 6) Clear dedupe + queued count + visible task list
            _dedupe.Clear();
            Interlocked.Exchange(ref _queuedItemCount, 0);

            // Important: also clear any in-progress UI items
            lbxTasks.SynchronizedInvoke(() =>
            {
                monitortasks.Clear();
                bsTasks.ResetBindings(false);
            });
            UpdateQueueCount();

            // 7) Drop the old channel entirely (this is what “clears the queue”)
            _opChannel = null;
            _queueCts = null;
        }

        private void txtConcurrentOperations_Validating(object sender, CancelEventArgs e)
        {
            if (int.TryParse(txtConcurrentOperations.Text, out int newCount) && newCount > 0)
            {
                MaxConcurrentOperations = newCount;
            }
            else
            {
                MessageBox.Show("Please enter a valid positive integer for concurrent operations.");
                e.Cancel = true;
            }
        }
    }

    public abstract class FileOperation
    {
        public abstract string FileName { get; }
        public abstract string OperationType { get; }
        public abstract string OpKey { get; }
    }

    public class CopyFileOperation : FileOperation
    {
        public FileInfo SourceFile { get; set; }
        public FileInfo DestFile { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
        public int SourceFolderIndex { get; set; }

        public override string FileName => SourceFile?.Name;
        public override string OperationType => "Copy";
        public override string OpKey => $"C|{SourceFile?.FullName ?? ""}|{DestFile?.FullName ?? ""}";
    }

    public class HashFileOperation : FileOperation
    {
        public FCFileInfo FileInfo { get; set; }

        public override string FileName => FileInfo?.Name;
        public override string OperationType => "Hash";
        public override string OpKey => $"H|{FileInfo?.FileInfo?.FullName ?? FileInfo?.Name ?? ""}";
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
        public static FCFileInfo FromFileInfo(FileInfo fi, bool truncatems = true)
        {
            DateTime lastWrite = fi.LastWriteTime;
            if (truncatems && lastWrite.Millisecond != 0 && ((fi.Attributes & FileAttributes.ReadOnly) == 0))
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
        public long RemoveAt { get; set; } = long.MaxValue;
        public string DisplayText { get => Progress.ToString(); }
    }
}