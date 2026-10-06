using System;
using System.Drawing;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using DVLD.Presentation.Properties;

namespace DVLD.Presentation.Tests
{
    public partial class frmListTestAppointments : Form
    {
        private readonly int _localDrivingLicenseApplicationId;
        private readonly clsTestType.enTestTypes _testType;

        public frmListTestAppointments(
            int localDrivingLicenseApplicationId,
            clsTestType.enTestTypes testType)
        {
            InitializeComponent();

            _localDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
            _testType = testType;
        }

        private void frmListTestAppointments_Load(object sender, EventArgs e)
        {
            ConfigureForm();

            if (!LoadApplication())
                return;

            LoadAppointments();
        }

        private void ConfigureForm()
        {
            string title;
            Image image;

            switch (_testType)
            {
                case clsTestType.enTestTypes.VisionTest:
                    title = "Vision Test Appointments";
                    image = Resources.Vision_512;
                    break;

                case clsTestType.enTestTypes.WrittenTest:
                    title = "Written Test Appointments";
                    image = Resources.Written_Test_512;
                    break;

                case clsTestType.enTestTypes.PracticalTest:
                    title = "Practical Test Appointments";
                    image = Resources.driving_test_512;
                    break;

                default:
                    title = "Test Appointments";
                    image = null;
                    break;
            }

            Text = title;
            lblTitle.Text = title;
            pbTestType.Image = image;
            dgvAppointments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

        }

        private bool LoadApplication()
        {
            var application = clsLocalDrivingLicenseApplication.Find(_localDrivingLicenseApplicationId);

            if (application == null)
            {
                MessageBox.Show(
                    "Application was not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
                return false;
            }

            ctrlDrivingLicenseApplicationInfo1
                .LoadDrivingLicenseApplicationInfo(_localDrivingLicenseApplicationId);

            return true;
        }

        private void LoadAppointments()
        {
            dgvAppointments.DataSource =
                clsTestAppointment.GetApplicationTestAppointmentPerTestType(
                    _localDrivingLicenseApplicationId,
                    (int)_testType);

            lblRecordsCount.Text = dgvAppointments.Rows.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private bool CanScheduleTestAppointment()
        {
            if (clsTestAppointment.ApplicantPassedTest(
                _localDrivingLicenseApplicationId,
                (int)_testType))
            {
                MessageBox.Show(
                    "This person already passed this test before.\n" +
                    "You can only retake failed tests.",
                    "Not Allowed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            if (clsTestAppointment.IsThereAnActiveAppointment(
                _localDrivingLicenseApplicationId,
                (int)_testType))
            {
                MessageBox.Show(
                    "This person already has an active appointment for this test.\n" +
                    "You cannot add a new appointment.",
                    "Not Allowed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            return true;
        }
        private void btnAddAppointment_Click(object sender, EventArgs e)
        {

            if (!CanScheduleTestAppointment())
                return;

            bool isRetakeTest =
                clsTestAppointment.ApplicantFailedTest(_localDrivingLicenseApplicationId, (int)_testType);

            frmScheduleTest frm = new frmScheduleTest(_localDrivingLicenseApplicationId, _testType, isRetakeTest);
            frm.ShowDialog();   

            frmListTestAppointments_Load(null,null);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmScheduleTest frm = new frmScheduleTest(
               (int) dgvAppointments.CurrentRow.Cells[0].Value);
            frm.ShowDialog();

            frmListTestAppointments_Load(null, null);//refresh list appointments
        }

        private void takeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTakeTest frm = new frmTakeTest(
                (int)dgvAppointments.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            frmListTestAppointments_Load(null, null);//refresh list appointments
        }
    }
}