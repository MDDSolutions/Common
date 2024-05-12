using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsDataAccess
{
    public partial class frmConnectDatabase : Form
    {
        public frmConnectDatabase()
        {
            InitializeComponent();
        }

        public frmConnectDatabase(string connstr) : this()
        {
            ConnectionStringBuilder = new SqlConnectionStringBuilder(connstr);
            txtServer.Text = ConnectionStringBuilder.DataSource;
            cbxDatabase.Text = ConnectionStringBuilder.InitialCatalog;
            chkSQLAuthentication.Checked = !ConnectionStringBuilder.IntegratedSecurity;

            txtUserName.Text = chkSQLAuthentication.Checked ? ConnectionStringBuilder.UserID : null;
            txtPassword.Text = chkSQLAuthentication.Checked ? ConnectionStringBuilder.Password : null;

            txtPassword.Enabled = chkSQLAuthentication.Checked;
            txtUserName.Enabled = chkSQLAuthentication.Checked;

        }

        private async void cbxDatabase_Enter(object sender, EventArgs e)
        {
            cbxDatabase.Items.Clear();
            try
            {
                SetConnectionString();
                ConnectionStringBuilder.ConnectTimeout = 3;
                using (SqlConnection cn = new SqlConnection(ConnectionStringBuilder.ToString()))
                {
                    await cn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases",cn))
                    {
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                cbxDatabase.Items.Add(rdr[0].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

        }
        public SqlConnectionStringBuilder ConnectionStringBuilder { get; set; } = new SqlConnectionStringBuilder();
        private void SetConnectionString()
        {
            ConnectionStringBuilder = new SqlConnectionStringBuilder();
            ConnectionStringBuilder.DataSource = txtServer.Text;
            ConnectionStringBuilder.InitialCatalog = DatabaseName;
            ConnectionStringBuilder.IntegratedSecurity = !chkSQLAuthentication.Checked;
            if (chkSQLAuthentication.Checked)
            {
                ConnectionStringBuilder.UserID = txtUserName.Text;
                ConnectionStringBuilder.Password = txtPassword.Text;
            }
        }
        public string ConnectionString 
        {
            get
            {
                SetConnectionString();
                return ConnectionStringBuilder.ToString();
            }
        }

        public string DatabaseName
        {
            get
            {
                return string.IsNullOrWhiteSpace(cbxDatabase.Text) ? "master" : cbxDatabase.Text;
            }
        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                btnTest.Enabled = false;
                SetConnectionString();
                using (SqlConnection cn = new SqlConnection(ConnectionStringBuilder.ToString()))
                {
                    await cn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SELECT @@VERSION", cn))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        MessageBox.Show(result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnTest.Enabled = true;
            }
        }

        private void chkSQLAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = chkSQLAuthentication.Checked;
            txtUserName.Enabled = chkSQLAuthentication.Checked;
        }
    }
}
