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

namespace DVLD.Presentation.LocalDrivingLicenseApplications
{
    public partial class frmNewUpdateLocalDrivingLicenseApplication : Form
    {

        enum enMode { 
            AddNew,
            Update
        }

        enMode _mode;

        private decimal _applicationFees;

        int _applicationId;
        clsLocalDrivingLicenseApplication _application;

        public frmNewUpdateLocalDrivingLicenseApplication()
        {
            _mode = enMode.AddNew;
            InitializeComponent();
        }

        public frmNewUpdateLocalDrivingLicenseApplication(int application)
        {
            _applicationId = application;
            _mode = enMode.Update;
            InitializeComponent();
        }
        private void frmNewLocalDrivingLicenseApplication_Load(object sender, EventArgs e)
        {
            LoadDefaultValues();

            if (_mode == enMode.Update) 
                LoadData();
        }

        private void LoadDefaultValues()
        {
            LoadLicenseClasses();

            if( _mode == enMode.AddNew)
            {
                tpApplicationInfo.Enabled = false;
                btnSave.Enabled = false;
                lblTitle.Text = "New Local Driving License Application";
                this.Text = "New Local Driving License Application";
                var applicationType =
               clsApplicationType.FindApplicationTypeByID(
                   (int)clsApplication.enApplicationType.NewLocalDrivingLicense);

                _applicationFees = applicationType.Fees;

                lblApplicationDate.Text = DateTime.Now.ToShortDateString();
                lblApplicationFees.Text = _applicationFees.ToString("0.##");
                lblUserName.Text = clsGlobal.CurrentUser.UserName;

            }
            else
            {
                lblTitle.Text = "Update Local Driving License Application";
                this.Text = "Update Local Driving License Application";
                ctrlPersonCardWithFilter1.FilterEnabled = false;
                btnSave.Enabled = true;
            }

        }

        private void LoadLicenseClasses()
        {
            cbLicenseClass.DataSource = clsLicenseClass.GetAllLicenseClasses();
            cbLicenseClass.DisplayMember = "ClassName";
            cbLicenseClass.ValueMember = "LicenseClassID";

            cbLicenseClass.SelectedValue = 3;
        }

        private void LoadData()
        {
            _application = clsLocalDrivingLicenseApplication.Find(_applicationId);
            if (_application == null)
            {
                MessageBox.Show($"No application with id = {_applicationId}", "Application Not Found", MessageBoxButtons.OK);
                this.Close();
                return;
            }
           
            ctrlPersonCardWithFilter1.LoadPersonInfo(_application.ApplicantPersonID);
            lblDLApplicationID.Text = _application.ApplicationID.ToString();
            lblApplicationDate.Text = _application.ApplicationDate.ToString();
            cbLicenseClass.SelectedValue = _application.LicenseClassID;
            lblApplicationFees.Text = _application.PaidFess.ToString();
            lblUserName.Text = _application.CreatedByUserInfo.UserName;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ctrlPersonCardWithFilter1.SelectedPersonInfo == null)
            {
                MessageBox.Show(
                    "Please select a person before continuing.",
                    "Selection Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            btnSave.Enabled = true;
            tpApplicationInfo.Enabled = true;
            tcLocalDrivingLicenseApplication.SelectedTab = tpApplicationInfo;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int personId = ctrlPersonCardWithFilter1.SelectedPersonInfo.ID;
            int licenseClassId = (int)cbLicenseClass.SelectedValue;

            if (HasActiveApplication(personId, licenseClassId))
                return;

            if (clsLicense.HisLicense(
               personId,
               licenseClassId))
            {
                MessageBox.Show(
                    "The applicant already has an active license for this license class.",
                    "License Already Exists",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);

                return;
            }

            if (_mode == enMode.AddNew)
            {
                _application =
            CreateApplication(personId, licenseClassId);
            }
           
            _application.LicenseClassID = licenseClassId;

            if (_application.Save())
            {
                lblDLApplicationID.Text = _application.ApplicationID.ToString();

                MessageBox.Show(
                    "Application saved successfully.",
                    "Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                btnSave.Enabled = false;
            }
            else
            {
                MessageBox.Show(
                    "Failed to save application.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool HasActiveApplication(int personID, int licenseClassID)
        {
            int applicationID =
                clsApplication.GetActiveApplicationForLicenseClass(
                    personID,
                    (int)clsApplication.enApplicationType.NewLocalDrivingLicense,
                    licenseClassID);

            if (applicationID == -1)
                return false;

            MessageBox.Show(
                $"The selected person already has an active application for this license class.\nApplication ID: {applicationID}",
                "Application Exists",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return true;
        }

        private clsLocalDrivingLicenseApplication CreateApplication(
            int personID,
            int licenseClassID)
        {
            return new clsLocalDrivingLicenseApplication
            {
                ApplicantPersonID = personID,
                LicenseClassID = licenseClassID,
                ApplicationDate = DateTime.Now,
                ApplicationTypeID = (int)clsApplication.enApplicationType.NewLocalDrivingLicense,
                ApplicationStatus = (int)clsApplication.enApplicationStatus.New,
                PaidFess = _applicationFees,
                CreatedByUserID = clsGlobal.CurrentUser.UserID
            };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

       
    }
}
