using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDDWinForms
{
    public partial class ctlWorkspaceManager : UserControl
    {
        WorkspaceConfiguration currentWorkspace;

        private void SetCurrentWorkspace(WorkspaceConfiguration workspace)
        {
            currentWorkspace = workspace;
            if (currentWorkspace != null)
            {
                txtName.Text = currentWorkspace.Name;
                bsWindowState.DataSource = currentWorkspace.WindowStates;
            }
            else
            {
                txtName.Text = string.Empty;
                bsWindowState.DataSource = null;
            }
        }

        public ctlWorkspaceManager()
        {
            InitializeComponent();
        }

        private void ctlWorkspaceManager_Load(object sender, EventArgs e)
        {
            if (!WorkspaceManager.Instance.ValidateMonitorLayout())
                MessageBox.Show("The current monitor layout does not match the saved workspace configuration. Please adjust your monitors or reset the workspace.", "Monitor Layout Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (WorkspaceManager.Instance.Workspaces == null)
                WorkspaceManager.Instance.Workspaces = new List<WorkspaceConfiguration>();

            lbxWorkspaces.DisplayMember = "Name";
            lbxWorkspaces.DataSource = WorkspaceManager.Instance.Workspaces;
        }

        private void lbxWorkspaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxWorkspaces.SelectedItem is WorkspaceConfiguration selectedWorkspace)
                SetCurrentWorkspace(selectedWorkspace);
        }

        private void btnGetCurrentState_Click(object sender, EventArgs e)
        {
            SetCurrentWorkspace(WorkspaceManager.GetCurrentWorkspace());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var current = currentWorkspace;
            if (!WorkspaceManager.Instance.Workspaces.Contains(currentWorkspace))
            {
                var existing = WorkspaceManager.Instance.Workspaces.FirstOrDefault(ws => ws.Name == currentWorkspace.Name);
                if (existing != null && existing != currentWorkspace)
                    WorkspaceManager.Instance.Workspaces.Remove(existing);
                WorkspaceManager.Instance.Workspaces.Add(currentWorkspace);
            }
            WorkspaceManager.Instance.Save();

            lbxWorkspaces.DataSource = null;
            lbxWorkspaces.DisplayMember = "Name";
            lbxWorkspaces.DataSource = WorkspaceManager.Instance.Workspaces;
            lbxWorkspaces.SelectedItem = current;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (currentWorkspace != null)
                WorkspaceManager.Instance.ApplyWorkspace(currentWorkspace);
        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            string newName = txtName.Text.Trim();
            if (currentWorkspace != null && currentWorkspace.Name != newName)
            {
                currentWorkspace.Name = newName;
                lbxWorkspaces.Refresh();
            }
        }

        private void lbxWorkspaces_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var selectedWorkspace = lbxWorkspaces.SelectedItem as WorkspaceConfiguration;
                if (selectedWorkspace != null)
                {
                    WorkspaceManager.Instance.Workspaces.Remove(selectedWorkspace);
                    WorkspaceManager.Instance.Save();
                    lbxWorkspaces.DataSource = null;
                    lbxWorkspaces.DisplayMember = "Name";
                    lbxWorkspaces.DataSource = WorkspaceManager.Instance.Workspaces;
                    //SetCurrentWorkspace(null);
                }
            }
        }
    }
}
