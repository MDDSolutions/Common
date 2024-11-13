using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDDFoundation;

namespace FormsDataAccess
{
    public partial class ctlQuery : UserControl
    {
        public ctlQuery()
        {
            InitializeComponent();
        }
        public ctlQuery(string connstr) : this()
        {
            ConnStr = connstr;
        }
        public ctlQuery(string connstr, string query) : this(connstr)
        {
            txtQuery.Text = query;
        }
        private string _ConnStr;
        public string ConnStr
        {
            get => _ConnStr;
            set
            {
                if (_ConnStr != value)
                {
                    _ConnStr = value;
                    var csb = new SqlConnectionStringBuilder();
                    csb.ConnectionString = _ConnStr;
                    txtConnect.Text = $"{csb.DataSource} - {csb.InitialCatalog}";
                }
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            var frm = new frmConnectDatabase(ConnStr);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ConnStr = frm.ConnectionString;
            }
        }
        public DataTable QueryResult { get; set; }
        public string QueryText 
        { 
            get => txtQuery.Text;
            set => txtQuery.Text = value; 
        }
        public async Task ExecuteQuery()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnStr))
                {
                    await connection.OpenAsync();

                    QueryResult = new DataTable();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(txtQuery.Text, connection))
                    {
                        await Task.Run(() => adapter.Fill(QueryResult)); // SqlDataAdapter.Fill is not natively async
                        dgvResults.SynchronizedInvoke(() => dgvResults.DataSource = QueryResult);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private async void btnExecute_Click(object sender, EventArgs e)
        {
            await ExecuteQuery();
        }

        private void dgvResults_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
        private static bool IsValidImage(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        // If we successfully reach here, the byte[] is a valid image
                        return true;
                    }
                }
            }
            catch (ArgumentException) // Catch the specific exception thrown when byte[] is invalid
            {
                return false;
            }
        }
        private void dgvResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn column in dgvResults.Columns)
                columns.Add(column);



            foreach (DataGridViewColumn column in columns)
            {
                int index = column.Index;
                if (column.ValueType == typeof(byte[]))
                {
                    var table = dgvResults.DataSource as DataTable;
                    if (table != null)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            var bytes = row.ItemArray[index] as byte[];
                            if (bytes != null)
                            {
                                if (!IsValidImage(bytes))
                                {
                                    dgvResults.Columns.RemoveAt(index);

                                    DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
                                    textColumn.Name = column.Name;
                                    textColumn.HeaderText = column.HeaderText;
                                    textColumn.DataPropertyName = column.DataPropertyName;
                                    textColumn.ValueType = typeof(string);  // or other appropriate type

                                    dgvResults.Columns.Insert(index, textColumn);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                Task.Run(async () => { await ExecuteQuery(); });
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
