using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using static DVLD.Presentation.People.frmManagePeople;

namespace DVLD.Presentation.User
{
    public partial class frmManageUsers : Form
    {
  
        public frmManageUsers()
        {
            InitializeComponent();
        }

        DataTable _dtUsers;
        void _LoadUsersList()
        {
            _dtUsers=clsUser.GetAllUsers();
            dgvUsers.DataSource = _dtUsers;
            lblUsersCount.Text = dgvUsers.RowCount.ToString();
        }
        private void frmManageUsers_Load(object sender, EventArgs e)
        {
            _LoadUsersList();

            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            cbFilterBy.SelectedIndex = 0;
            cbUserState.SelectedIndex = 0;
           
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Visible = cbFilterBy.SelectedItem != null && cbFilterBy.SelectedItem.ToString() != "None" && cbFilterBy.SelectedItem.ToString() != "IsActive";
            cbUserState.Visible = cbFilterBy.SelectedItem.ToString() == "IsActive";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (cbFilterBy.Text == "None")
                return;

            ApplyFilter(
    (DataTable)dgvUsers.DataSource,
     cbFilterBy.Text,
     txtSearch.Text);

            lblUsersCount.Text = dgvUsers.Rows.Count.ToString();
        }

        private void cbUserState_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            switch (cbUserState.SelectedItem)
            {
                case "Yes":
                    _dtUsers.DefaultView.RowFilter = "IsActive=1";
                    break;

                case "No":
                    _dtUsers.DefaultView.RowFilter = "IsActive=0";
                    break;
                default:
                    _dtUsers.DefaultView.RowFilter = "";
                    break;

            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (GetInputType(cbFilterBy.Text))
            {
                case enInputType.Number:
                    e.Handled = !char.IsDigit(e.KeyChar) &&
                                !char.IsControl(e.KeyChar);
                    break;

                case enInputType.Text:
                    e.Handled = !char.IsLetter(e.KeyChar) &&
                                !char.IsWhiteSpace(e.KeyChar) &&
                                !char.IsControl(e.KeyChar);
                    break;

                case enInputType.AlphaNumeric:
                    e.Handled = !char.IsLetterOrDigit(e.KeyChar) &&
                                !char.IsControl(e.KeyChar);
                    break;
            }
        }

        private enInputType GetInputType(string ColumnName)
        {
            switch (ColumnName)
            {
                case "PersonID":
                case "UserID":
                    return enInputType.Number;

                case "UserName":
                case "FullName":
                    return enInputType.Text;

                default:
                    return enInputType.AlphaNumeric;
            }
        }

        public static void ApplyFilter(DataTable dt, string ColumnName, string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                dt.DefaultView.RowFilter = "";
                return;
            }

            Type type = dt.Columns[ColumnName].DataType;

            if (type == typeof(string))
                dt.DefaultView.RowFilter = $"[{ColumnName}] LIKE '{Value}%'";
            else
                dt.DefaultView.RowFilter = $"[{ColumnName}] = {Value}";
        }

        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.ShowDialog();
            frmManageUsers_Load(null,null);
        }

        private void addNewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.ShowDialog();
            frmManageUsers_Load(null, null);
        }

        private void ediltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser((int)dgvUsers.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            frmManageUsers_Load(null, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show($"Are you sure you want to delete user with id {dgvUsers.CurrentRow.Cells[0].Value}",
                "Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {

                if (clsUser.DeleteUser((int)dgvUsers.CurrentRow.Cells[0].Value))
                {
                   
                    frmManageUsers_Load(null, null);
                    MessageBox.Show(
                                   "User deleted successfully.",
                                   "Success",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);

                    frmManageUsers_Load(null, null);
                }
                else
                {
                    MessageBox.Show(
                             "Failed to delete user.",
                             "Error",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
                }

            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUserInfo frm = new frmUserInfo((int)dgvUsers.CurrentRow.Cells[0].Value);
            frm.ShowDialog();   
        }

        private void chaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword((int)dgvUsers.CurrentRow.Cells[0].Value);
            frm.ShowDialog();   
        }
    }
}
