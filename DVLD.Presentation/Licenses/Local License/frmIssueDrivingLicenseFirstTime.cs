using System;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.Licenses
{
    public partial class frmIssueDrivingLicenseFirstTime : Form
    {
        private readonly int _localDrivingLicenseApplicationId;

        public frmIssueDrivingLicenseFirstTime(int localDrivingLicenseApplicationId)
        {
            InitializeComponent();

            _localDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
        }

        private void frmIssueDrivingLicenseFirstTime_Load(object sender, EventArgs e)
        {
            ctrlDrivingLicenseApplicationInfo1
                .LoadDrivingLicenseApplicationInfo(_localDrivingLicenseApplicationId);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
            clsLocalDrivingLicenseApplication localApplication =
                clsLocalDrivingLicenseApplication.Find(
                    _localDrivingLicenseApplicationId);

            if (localApplication == null)
            {
                ShowError(
                    $"No application found with ID = {_localDrivingLicenseApplicationId}.",
                    "Application Not Found");

                return;
            }

            if (!clsTest.PassedAllTests(_localDrivingLicenseApplicationId))
            {
                ShowWarning(
                    "The applicant has not passed all required tests.",
                    "Cannot Issue License");

                return;
            }

            if (clsLicense.HisLicense(
                localApplication.ApplicantPersonID,
                localApplication.LicenseClassID))
            {
                ShowWarning(
                    "The applicant already has an active license for this license class.",
                    "License Already Exists");

                return;
            }

            clsDriver driver = GetOrCreateDriver(
                localApplication.ApplicantPersonID);

            if (driver == null)
            {
                return;
            }

            clsLicense license = CreateLicense(localApplication, driver);

            if (!license.Save())
            {
                ShowError(
                    "Failed to issue the license.",
                    "License Issuance Failed");

                return;
            }

            if (!clsApplication.CompletingApplication(
                localApplication.ApplicationID))
            {
                ShowWarning(
                    $"License #{license.LicenseID} was issued successfully, " +
                    "but the application could not be marked as completed.",
                    "Application Update Failed");

                return;
            }

            MessageBox.Show(
                $"License issued successfully.\n\nLicense ID: {license.LicenseID}",
                "License Issued",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            btnIssue.Enabled = false;
        }

        private clsDriver GetOrCreateDriver(int personId)
        {
            clsDriver driver = clsDriver.FindByPersonID(personId);

            if (driver != null)
            {
                return driver;
            }

            driver = new clsDriver
            {
                PersonID = personId,
                CreatedByUserID = clsGlobal.CurrentUser.UserID,
                CreatedDate = DateTime.Now
            };

            if (driver.Save())
            {
                return driver;
            }

            ShowError(
                "Failed to create a driver record.",
                "Driver Creation Failed");

            return null;
        }

        private clsLicense CreateLicense(
            clsLocalDrivingLicenseApplication localApplication,
            clsDriver driver)
        {
            DateTime issueDate = DateTime.Now;

            return new clsLicense
            {
                ApplicationID = localApplication.ApplicationID,
                DriverID = driver.DriverID,
                LicenseClass = localApplication.LicenseClassID,
                IssueDate = issueDate,
                ExpirationDate = issueDate.AddYears(
                    localApplication.LicenseClassInfo.DefaultValidityLength),
                Notes = txtNotes.Text.Trim(),
                PaidFess = localApplication.LicenseClassInfo.ClassFees,
                IsActive = true,
                IssueReason = (byte)clsLicense.enIssueReason.FirstTime,
                CreatedByUserID = clsGlobal.CurrentUser.UserID
            };
        }

        private void ShowError(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ShowWarning(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }


}
