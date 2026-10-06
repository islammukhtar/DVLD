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
using DVLD.BusinessLogic;
using DVLD.Presentation.Properties;
using static System.Net.Mime.MediaTypeNames;


namespace DVLD.Presentation.Tests.Controls
{
    public partial class ctrlScheduleTest : UserControl
    {
        enum enMode
        {
            AddNew,
            Update
        }

        enMode _mode;

        int _localDrvingLicenseApplicationId;
        clsTestType.enTestTypes _testType;
        clsLocalDrivingLicenseApplication _application;
        clsTestAppointment _testAppointment;
        bool _isRetakeTest;

        public ctrlScheduleTest()
        {
            InitializeComponent();
        }
        void _FillControls()
        {

            lblLocalDrivingLicenseAppID.Text = _application.LocalDrivingLicenseApplicationID.ToString();
            lblDrivingClass.Text = _application.LicenseClassInfo?.ClassName;
            lblFullName.Text = _application.ApplicantPersonInfo?.FullName;
            lblTrial.Text = clsTestAppointment.TotalTrials(_localDrvingLicenseApplicationId, (int)_testType).ToString();
           
            decimal testFees = clsTestType.FindTestTypeByID((int)_testType).Fees;
            lblFees.Text = testFees.ToString("0.##");  // 12();

            if (_isRetakeTest)
            {
                decimal retakeAppFees = clsApplicationType.FindApplicationTypeByID(
                          (int)clsApplicationType.enApplicationTypes.RetakeTest).Fees;


                lblRetakeAppFees.Text = retakeAppFees.ToString("0.##");

                lblTotalFees.Text = (testFees + retakeAppFees).ToString("0.##");
            }
         

        }

        public void LoadForAddNew(
        int localDrivingLicenseApplicationId,
        clsTestType.enTestTypes testType,
        bool isRetakeTest = false)
        {
            _mode = enMode.AddNew;
            _localDrvingLicenseApplicationId = localDrivingLicenseApplicationId;
            _testType = testType;
            _isRetakeTest = isRetakeTest;

            ConfigureForm();

            _application =
                clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationId);

            if (_application == null)
            {
                MessageBox.Show(
                    "Application was not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                btnSave.Enabled = false;
                return;
            }

            _testAppointment = new clsTestAppointment();

            _FillControls();
            _ConfigureTestDate();


        }

        public void LoadForUpdate(int testAppointmentId)
        {
            _mode = enMode.Update;
            _testAppointment = clsTestAppointment.Find(testAppointmentId);
            gbRetakeTestInfo.Enabled = false;

            if (_testAppointment == null) 
            { 
                MessageBox.Show(
                    "Test appointment was not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    ); 
                btnSave.Enabled = false; return;
            }

            if (_testAppointment.IsLocked)
            {
                dtpAppointmentDate.Enabled = false;
                btnSave.Enabled = false;
                lblUserMessage.Visible = true;
                lblUserMessage.Text = "Person already sat for the test,Appointment locked.";
               
                MessageBox.Show(
                    "This test appointment is locked. You cannot change the appointment date or save a new test result.",
                    "Locked Test Appointment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                
            }
              

            lblLocalDrivingLicenseAppID.Text = _testAppointment.LocalDrivingLicenseApplicationID.ToString();
           

            var localDrvingLicenseApplicationInfo=clsLocalDrivingLicenseApplication.Find(
                _testAppointment.LocalDrivingLicenseApplicationID);

            lblDrivingClass.Text = localDrvingLicenseApplicationInfo?.LicenseClassInfo?.ClassName;

            lblFullName.Text = localDrvingLicenseApplicationInfo?.ApplicantPersonInfo?.FullName;

            lblTrial.Text = clsTestAppointment.TotalTrials(
                _testAppointment.LocalDrivingLicenseApplicationID, 
                (int)_testAppointment.TestTypeID).ToString();


            _ConfigureTestDate();

            lblFees.Text = _testAppointment.PaidFees.ToString();
        }

        private void ConfigureForm()
        {
            gbRetakeTestInfo.Enabled = _isRetakeTest;
           
            System.Drawing.Image image;

            switch (_testType)
            {
                case clsTestType.enTestTypes.VisionTest:
                    gbTestType.Text = "Vision Test";
                    image = Resources.Vision_512;
                    break;

                case clsTestType.enTestTypes.WrittenTest:
                    gbTestType.Text = "Written Test";
                    image = Resources.Written_Test_512;
                    break;

                case clsTestType.enTestTypes.PracticalTest:
                    gbTestType.Text = "Practical Test";
                    image = Resources.driving_test_512;
                    break;

                default:
                    image = null;
                    break;
            }


            lblTitle.Text = (_isRetakeTest) ? "Schedule Retake Test" : "Schedule Test";
            pbTestTypeImage.Image = image;
        }

        bool _SaveForAddNew()
        {
           
            _testAppointment.TestTypeID = _testType;
            _testAppointment.LocalDrivingLicenseApplicationID = Convert.ToInt32(lblLocalDrivingLicenseAppID.Text);
            _testAppointment.AppointmentDate = dtpAppointmentDate.Value;
            _testAppointment.PaidFees = Convert.ToSingle(lblFees.Text);
            _testAppointment.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            _testAppointment.IsLocked = false;

            if (_isRetakeTest)
            {
                var retakeTestApplicationType = new clsApplication();
                retakeTestApplicationType.ApplicantPersonID
                    = clsLocalDrivingLicenseApplication.Find(_localDrvingLicenseApplicationId).ApplicantPersonID;

                retakeTestApplicationType.ApplicationDate = DateTime.Now;

                retakeTestApplicationType.ApplicationTypeID
                    = (int)clsApplication.enApplicationType.RetakeTest;

                retakeTestApplicationType.ApplicationStatus =
                   (int)clsApplication.enApplicationStatus.Completed;

                retakeTestApplicationType.LastStatusDate = DateTime.Now;

                retakeTestApplicationType.PaidFess =
                   clsApplicationType.FindApplicationTypeByID((int)clsApplication.enApplicationType.RetakeTest).Fees;


                retakeTestApplicationType.CreatedByUserID = clsGlobal.CurrentUser.UserID;

                if (!retakeTestApplicationType.Save()) {

                    MessageBox.Show(
                       "Failed to save application.",
                       "Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);

                    return false;

                }

                lblRetakeTestAppID.Text = retakeTestApplicationType.ApplicationID.ToString();
                _testAppointment.PaidFees = Convert.ToSingle(lblTotalFees.Text);
                _testAppointment.RetakeTestApplicationID = retakeTestApplicationType.ApplicationID;

            }

            if (_testAppointment.Save())
            {
                MessageBox.Show(
                    "Test appointment saved successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return true;
            }
            else
            {
                MessageBox.Show(
                "Failed to save test appointment.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return false;
            }

        }

        bool _SaveForUpdate()
        {
            _testAppointment.AppointmentDate = dtpAppointmentDate.Value;

            if (_testAppointment.Save())
            {
                MessageBox.Show(
                    "Test appointment updated successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return true;
            }
            else
            {
                MessageBox.Show(
                "Failed to update test appointment.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return false;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            switch (_mode) {


                case enMode.AddNew:
                    {
                        if (!_SaveForAddNew())
                            return;
                        
                    }
                    break;
                case enMode.Update:
                    {
                        if (!_SaveForUpdate())
                            return;
                    }
                   break;
                    
            }

        }

        private void _ConfigureTestDate()
        {
          
            if (_mode == enMode.AddNew)
            {
                dtpAppointmentDate.Enabled = true;
                dtpAppointmentDate.MinDate = DateTime.Today;
                dtpAppointmentDate.Value = DateTime.Today;
                return;
            }

            // Update mode
            if (_testAppointment == null)
                return;
           
            dtpAppointmentDate.Value = _testAppointment.AppointmentDate;
            dtpAppointmentDate.MinDate = DateTime.Today;
            //dtpTestDate.Enabled =
            //    _testAppointment.AppointmentDate.Date >= DateTime.Today;


        }
        

    }
}
