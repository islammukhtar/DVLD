using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.People
{
    public partial class frmManagePeople : Form
    {
        public frmManagePeople()
        {
            InitializeComponent();
        }

        private DataTable _dtPeople;

        private void _LoadPeopleList()
        {
            _dtPeople = clsPerson.GetAllPeople();

            dgvPeople.DataSource = _dtPeople;

            dgvPeople.ColumnHeadersDefaultCellStyle.Font =
                       new Font("Segoe UI", 10, FontStyle.Regular);


            lblRecordsCount.Text = _dtPeople.Rows.Count.ToString();
        }

        private void frmManagePeople_Load(object sender, EventArgs e)
        {
            _LoadPeopleList();
            cbFilterBy.SelectedIndex = 0;

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
            //Refresh people list
            _LoadPeopleList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson((int)dgvPeople.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            //Refresh people list
            _LoadPeopleList();
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
            //Refresh people list
            _LoadPeopleList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow != null)
            {
                if (MessageBox.Show(
                $"Are you sure you want to delete person with id {(int)dgvPeople.CurrentRow.Cells[0].Value}",
                "Confirm",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    if (clsPerson.DeletePerson((int)dgvPeople.CurrentRow.Cells[0].Value))
                    {
                        MessageBox.Show(
                                   "Person deleted successfully.",
                                   "Success",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);

                        _LoadPeopleList(); // Refresh People list
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete person.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        public enum enInputType
        {
            Number,
            Text,
            AlphaNumeric
        }

        private enInputType GetInputType(string ColumnName)
        {

            switch (ColumnName)
            {
                case "PersonID":
                case "CountryID":
                case "Phone":
                    return enInputType.Number;

                case "FirstName":
                case "SecondName":
                case "ThirdName":
                case "LastName":
                case "Email":
                case "Nationality":
                case "Gender":

                    return enInputType.Text;

                default:
                    return enInputType.AlphaNumeric;
            }
        }

        public  void ApplyFilter(DataTable dt, string ColumnName, string Value)
        {

            if (string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(cbFilterBy.Text)) 
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

        private void showDetialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonDetails frm = new frmPersonDetails((int)dgvPeople.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            //Refresh people list 
            _LoadPeopleList();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter(
                      (DataTable)dgvPeople.DataSource,
                        cbFilterBy.Text,
                        txtSearch.Text);
                       
            lblRecordsCount.Text = dgvPeople.Rows.Count.ToString();
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

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Visible = cbFilterBy.Text != "None";
            
            if (txtSearch.Visible)
            {
                txtSearch.Text = string.Empty;
                txtSearch.Focus();
            }
        }

    }
}
