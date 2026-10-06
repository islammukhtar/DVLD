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
using DVLD.Presentation.Applications.LocalDrivingLicenseApplications;
using DVLD.Presentation.Licenses;
using DVLD.Presentation.Licenses.Local_License;
using DVLD.Presentation.Tests;

namespace DVLD.Presentation.LocalDrivingLicenseApplications
{
    public partial class frmListLocalDrivingLicenseApplications : Form
    {
        DataTable _localDrivingLicenseApplications;
        public frmListLocalDrivingLicenseApplications()
        {
            InitializeComponent();
        }

        private void LoadLocalDrivingLicenseApplicationList()
        {
            _localDrivingLicenseApplications = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicensesApplications();
            dgvLocalDrivingLicenseApplications.DataSource = _localDrivingLicenseApplications;

            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
        }

        private void frmLocalDrivingLicenseApplications_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
            LoadLocalDrivingLicenseApplicationList();
            dgvLocalDrivingLicenseApplications.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10,FontStyle.Regular);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                case "LDLAppID":
                    return enInputType.Number;

               
                case "FullName":
                case "Status":

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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter(
                      (DataTable)dgvLocalDrivingLicenseApplications.DataSource,
                        cbFilterBy.Text,
                        txtSearch.Text);

            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
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

        private void btnAddNewApplication_Click(object sender, EventArgs e)
        {
            frmNewUpdateLocalDrivingLicenseApplication frm=new frmNewUpdateLocalDrivingLicenseApplication();
            frm.ShowDialog();
            LoadLocalDrivingLicenseApplicationList();
        }

        private void editApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewUpdateLocalDrivingLicenseApplication frm = new frmNewUpdateLocalDrivingLicenseApplication(
                (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value
                );
            frm.ShowDialog();
            LoadLocalDrivingLicenseApplicationList();
        }

        private void deleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow != null)
            {
                if (MessageBox.Show(
                $"Are you sure you want to delete application with id {(int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value}",
                "Confirm",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    if (clsLocalDrivingLicenseApplication.Delete((int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value))
                    {
                        MessageBox.Show(
                                   "application deleted successfully.",
                                   "Success",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);

                        LoadLocalDrivingLicenseApplicationList(); // Refresh application list
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete application.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void cancelApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow != null)
            {
                if (MessageBox.Show(
                $"Are you sure you want to cancel application with id {(int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value}",
                "Confirm",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    if (clsLocalDrivingLicenseApplication.Cancel((int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value))
                    {
                        MessageBox.Show(
                                   "application cancelled successfully.",
                                   "Success",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);

                        LoadLocalDrivingLicenseApplicationList(); // Refresh application list
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to cancel application.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmLocalDrivingLicenseApplicationInfo frm = new frmLocalDrivingLicenseApplicationInfo(
                (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value
                );
            frm.ShowDialog();
            LoadLocalDrivingLicenseApplicationList();
        }

        private void sechduleViToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListTestAppointments frm = new frmListTestAppointments(
                (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value,clsTestType.enTestTypes.VisionTest
                );

            frm.ShowDialog();
            LoadLocalDrivingLicenseApplicationList();
        }

        private void sechduleToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmListTestAppointments frm = new frmListTestAppointments(
                (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value, clsTestType.enTestTypes.WrittenTest
                );

            frm.ShowDialog();
            LoadLocalDrivingLicenseApplicationList();
        }

        private void sechduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListTestAppointments frm = new frmListTestAppointments(
               (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value, clsTestType.enTestTypes.PracticalTest
               );

            frm.ShowDialog();
            LoadLocalDrivingLicenseApplicationList();
        }
        private void UpdateScheduleTestsMenu()
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow == null)
                return;

            int localDrivingLicenseApplicationId =
                Convert.ToInt32(
                    dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);

            bool passedVisionTest = clsTestAppointment.ApplicantPassedTest(
                localDrivingLicenseApplicationId,
                (int)clsTestType.enTestTypes.VisionTest);

            bool passedWrittenTest = clsTestAppointment.ApplicantPassedTest(
                localDrivingLicenseApplicationId,
                (int)clsTestType.enTestTypes.WrittenTest);

            bool passedPracticalTest = clsTestAppointment.ApplicantPassedTest(
                localDrivingLicenseApplicationId,
                (int)clsTestType.enTestTypes.PracticalTest);


            sechduleViToolStripMenuItem.Enabled = !passedVisionTest;

            sechduleWrittenTest.Enabled =
                passedVisionTest && !passedWrittenTest;

            sechduleStreetTestToolStripMenuItem.Enabled =
                passedVisionTest &&
                passedWrittenTest &&
                !passedPracticalTest;


            // Disable the main Schedule menu if no test can be scheduled.
                schToolStripMenuItem.Enabled =
                sechduleViToolStripMenuItem.Enabled ||
                sechduleWrittenTest.Enabled ||
                sechduleStreetTestToolStripMenuItem.Enabled;


         

        }
        private void UpdateApplicationMenuItems()
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow == null)
                return;

            int localDrivingLicenseApplicationId =
                Convert.ToInt32(
                    dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);

            clsLocalDrivingLicenseApplication localApplication =
                clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationId);

            if (localApplication == null)
                return;

            bool isCompleted =
                localApplication.ApplicationStatus ==
                (byte)clsApplication.enApplicationStatus.Completed;

            editApplicationToolStripMenuItem.Enabled = !isCompleted;
            deleteApplicationToolStripMenuItem.Enabled = !isCompleted;
            cancelApplicationToolStripMenuItem.Enabled = !isCompleted;
            issueDrivingLicensefirstTmieToolStripMenuItem.Enabled= !isCompleted;

            bool isCancelled =
                localApplication.ApplicationStatus ==
                (byte)clsApplication.enApplicationStatus.Cancelled;

            cmsApplication.Enabled = !isCancelled;


            bool hasLicense =
                clsLicense.HisLicense(localApplication.ApplicantPersonID, localApplication.LicenseClassID);

            bool passedAllTests =
                clsTest.PassedAllTests(localApplication.LocalDrivingLicenseApplicationID);

            issueDrivingLicensefirstTmieToolStripMenuItem.Enabled = !hasLicense && passedAllTests;


            showLicenseToolStripMenuItem.Enabled = hasLicense;


        }
        private void cmsApplication_Opening(object sender, CancelEventArgs e)
        {
            UpdateApplicationMenuItems();
            UpdateScheduleTestsMenu();
        }
        private void issueDrivingLicensefirstTmieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmIssueDrivingLicenseFirstTime frm = new frmIssueDrivingLicenseFirstTime(
                (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);    
            frm.ShowDialog();
           frmLocalDrivingLicenseApplications_Load(null,null);
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int localDrivingLicenseApplicationId =
               Convert.ToInt32(
                   dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);

            clsLocalDrivingLicenseApplication localApplication =
                clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationId);

            int localLicenseId = clsLicense.FindLicenseInfoByApplicationId(localApplication.ApplicationID).LicenseID;

            frmShowLicenseInfo frm = new frmShowLicenseInfo(
               localLicenseId);
            frm.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int localDrivingLicenseApplicationId =
                Convert.ToInt32(
                    dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);

            clsLocalDrivingLicenseApplication localApplication =
                clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationId);
            
            if (localApplication == null)
                return;

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(localApplication.ApplicantPersonID);
            frm.ShowDialog();

        }
    }
}
