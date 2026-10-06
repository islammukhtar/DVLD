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
using DVLD.Presentation.Properties;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Presentation.Tests.Controls
{
    public partial class ctrlScheduledTest : UserControl
    {

        public event EventHandler TestAppointmentLoaded;

        clsTestAppointment _testAppointment;
        public ctrlScheduledTest()
        {
            InitializeComponent();
        }

        void _FillControls()
        {

            lblLocalDrivingLicenseAppID.Text
                = _testAppointment.LocalDrivingLicenseApplicationInfo.LocalDrivingLicenseApplicationID.ToString();

            lblDrivingClass.Text = _testAppointment.LocalDrivingLicenseApplicationInfo.LicenseClassInfo.ClassName;


            lblFullName.Text = _testAppointment.LocalDrivingLicenseApplicationInfo.ApplicantPersonInfo.FullName;

            lblTrial.Text = clsTestAppointment.TotalTrials
                (_testAppointment.LocalDrivingLicenseApplicationID, (int)_testAppointment.TestTypeID).ToString();

            lblDate.Text = _testAppointment.AppointmentDate.ToShortDateString();

            lblFees.Text = _testAppointment.PaidFees.ToString();

        }

        public void LoadTestInfo(int testAppointment)
        {
            _testAppointment = clsTestAppointment.Find(testAppointment);

            if (_testAppointment == null)
            {
                MessageBox.Show(
                    "Test appointment was not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                return;
            }

            TestAppointmentLoaded?.Invoke(_testAppointment, EventArgs.Empty);
            ConfigureForm();
            _FillControls();

        }

        private void ConfigureForm()
        {

            switch (_testAppointment?.TestTypeID)
            {
                case clsTestType.enTestTypes.VisionTest:
                    gbTestType.Text = "Vision Test";
                    pbTestTypeImage.Image = Resources.Vision_512;
                    break;

                case clsTestType.enTestTypes.WrittenTest:
                    gbTestType.Text = "Written Test";
                    pbTestTypeImage.Image = Resources.Written_Test_512;
                    break;

                case clsTestType.enTestTypes.PracticalTest:
                    gbTestType.Text = "Practical Test";
                    pbTestTypeImage.Image = Resources.driving_test_512;
                    break;

                default:
                    pbTestTypeImage.Image = null;
                    break;

            }
        }
    }
}
