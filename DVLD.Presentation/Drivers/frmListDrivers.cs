using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.Drivers
{
    public partial class frmListDrivers : Form
    {
        public frmListDrivers()
        {
            InitializeComponent();
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
                case "DriverID":
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
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter(
                      (DataTable)dgvDrivers.DataSource,
                        cbFilterBy.Text,
                        txtSearch.Text);

            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
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

        private void frmListDrivers_Load(object sender, EventArgs e)
        {

            cbFilterBy.SelectedIndex = 0;
            dgvDrivers.ColumnHeadersDefaultCellStyle.Font =
                     new Font("Segoe UI", 10, FontStyle.Regular);
            dgvDrivers.DataSource = clsDriver.GetAllDrivers();
            lblRecordsCount.Text = dgvDrivers.Rows.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
