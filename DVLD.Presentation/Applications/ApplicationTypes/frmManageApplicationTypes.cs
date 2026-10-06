using System;
using System.Drawing;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.ApplicationTypes
{
    public partial class frmManageApplicationTypes : Form
    {
        public frmManageApplicationTypes()
        {
            InitializeComponent();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadListApplicationType()
        {
            dgvApplicationTypes.DataSource = clsApplicationType.GetAllApplicationTypes();
        }

        private void frmManageApplicationTypes_Load(object sender, EventArgs e)
        {

            dgvApplicationTypes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvApplicationTypes.RowsDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            LoadListApplicationType();

            if (dgvApplicationTypes.Rows.Count > 0)
            {

                lblRecords.Text = dgvApplicationTypes.Rows.Count.ToString();

                dgvApplicationTypes.Columns[0].HeaderText = "ID";
                dgvApplicationTypes.Columns[0].Width = 80;

                dgvApplicationTypes.Columns[1].HeaderText = "Title";
                dgvApplicationTypes.Columns[1].Width = 400;

                dgvApplicationTypes.Columns[2].HeaderText = "Fees";
                dgvApplicationTypes.Columns[2].Width = 110;
            }
        }

        private void editApplicationTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmUpdateApplicationType((int)dgvApplicationTypes.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            LoadListApplicationType();//refresh list application type
        }


    }
}
