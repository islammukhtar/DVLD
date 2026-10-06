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
using System.Xml.Linq;
using DVLD.BusinessLogic;
using DVLD.Presentation.Applications.ReleasedDetainedLicenseApplication;

namespace DVLD.Presentation.Licenses.Detain_License
{
    public partial class frmListDetainedLicenses : Form
    {
        public frmListDetainedLicenses()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmListDetainedLicenses_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
            dgvDetainedLicenses.DataSource = clsDetainedLicense.GetAllDetainedLicense();
            lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void btnDetain_Click(object sender, EventArgs e)
        {
            frmDetainLicense frm = new frmDetainLicense();
            frm.ShowDialog();
            frmListDetainedLicenses_Load(null, null);
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {
            frmReleasedDetainedLicenseApplication frm
                = new frmReleasedDetainedLicenseApplication(
                    (int)dgvDetainedLicenses.CurrentRow.Cells[1].Value);
            frm.ShowDialog();

            frmListDetainedLicenses_Load(null, null);
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
                case "DetainID":
                case "ReleaseApplicationID":
                    return enInputType.Number;

                case "FullName":

                    return enInputType.Text;

                default:
                    return enInputType.AlphaNumeric;
            }
        }

        public void ApplyFilter(DataTable dt, string ColumnName, string Value)
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

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
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
            txtFilterValue.Visible = (cbFilterBy.SelectedItem.ToString() != "None")
                && (cbFilterBy.SelectedItem.ToString() != "IsReleased");
            cbIsReleased.Visible = cbFilterBy.SelectedItem.ToString() == "IsReleased";
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (cbFilterBy.Text == "None")
                return;

            ApplyFilter(
                (DataTable)dgvDetainedLicenses.DataSource, 
                cbFilterBy.Text,
                txtFilterValue.Text);

            lblRecordsCount.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void cbIsReleased_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbIsReleased.SelectedItem)
            {
                case "Yes":
                    ((DataTable)dgvDetainedLicenses.DataSource).DefaultView.RowFilter = "IsReleased=1";
                    break;

                case "No":
                    ((DataTable)dgvDetainedLicenses.DataSource).DefaultView.RowFilter = "IsReleased=0";
                    break;
                default:
                    ((DataTable)dgvDetainedLicenses.DataSource).DefaultView.RowFilter = "";
                    break;

            }
        }
    }
}
