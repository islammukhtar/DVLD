using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using DVLD.Presentation.Properties;

namespace DVLD.Presentation.Licenses.Local_License.Controls
{
    public partial class ctrlDriverLicenseInfo : UserControl
    {
        
        enum enGender
        {
            Male,
            Female
        }
        int _licenseId;
        clsLicense _license;
        clsLocalDrivingLicenseApplication _localApplication;

        public int LicenseId
        {
            get { return _licenseId; }
        }
        public clsLicense SelectedLicenseInfo
        {
            get 
            {
                return _license;
            }
        }
        public ctrlDriverLicenseInfo()
        {
            InitializeComponent();
        }
        private void ShowError(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        void _LoadPersonImage()
        {
            short gender
                = _license.ApplicationInfo.ApplicantPersonInfo.Gender;
            string imagePath =
                _license.ApplicationInfo.ApplicantPersonInfo.ImagePath;

            if (gender == (short)enGender.Male)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            if (imagePath != string.Empty)
                if (File.Exists(imagePath))
                    pbPersonImage.ImageLocation = imagePath;
                else
                    MessageBox.Show(
                        "Could not find this image " + imagePath,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);


        }
        private void _LoadLicenseDataIntoTheControls()
        {
            lblClass.Text = _license.LicenseClassInfo.ClassName ;
            lblFullName.Text = _license.ApplicationInfo.ApplicantPersonInfo?.FullName;
            lblLicenseID.Text = _license.LicenseID.ToString();
            lblNationalNo.Text = _license.ApplicationInfo.ApplicantPersonInfo?.NationalNo.ToString();
            lblGender.Text = _license.ApplicationInfo.ApplicantPersonInfo.Gender == (short)enGender.Male ? "Male" : "Female";
            lblIssueDate.Text = _license.IssueDate.ToShortDateString();
            lblIssueReason.Text = _license.IssueReasonText;
            lblNotes.Text = _license.Notes != string.Empty ? _license.Notes : "No Notes";
            lblIsActive.Text = (_license.IsActive) ? "Yes" : "No";
            lblDateOfBirth.Text = _license.ApplicationInfo.ApplicantPersonInfo.DateOfBirth.ToShortDateString();
            lblDriverID.Text = _license.DriverID.ToString();
            lblExpirationDate.Text = _license.ExpirationDate.ToShortDateString();
            lblIsDetained.Text = (_license.IsDetained) ? "Yes" : "No";
            _LoadPersonImage();
        }

        public void LoadDriverLicenseInfoByLocalApplication(int localDrivingLicenseApplicationId)
        {

            _localApplication =
               clsLocalDrivingLicenseApplication.Find(
                   localDrivingLicenseApplicationId);

            if (_localApplication == null)
            {
                ShowError(
                    $"No application found with ID = {localDrivingLicenseApplicationId}.",
                    "Application Not Found");

                return;
            }

            _license =
      clsLicense.FindLicenseInfoByApplicationId(_localApplication.ApplicationID);

            if (_license == null)
            {
                ShowError(
                    $"No issued driving license was found for local application ID = {localDrivingLicenseApplicationId}.",
                    "License Not Found");

                return;
            }

            // Load license data into the controls here.
            _LoadLicenseDataIntoTheControls();
        }

        public void ResetDefaultValues()
        {
            lblClass.Text = "[???]";
            lblFullName.Text = "[????]";
            lblLicenseID.Text = "[????]";
            lblNationalNo.Text = "[????]";
            lblGender.Text = "[????]";
            lblIssueDate.Text = "[????]";
            lblIssueReason.Text = "[????]";
            lblNotes.Text = "[????]";
            lblIsActive.Text = "[????]";
            lblDateOfBirth.Text = "[????]";
            lblDriverID.Text = "[????]";
            lblExpirationDate.Text = "[????]";
            lblIsDetained.Text = "[????]";
            pbPersonImage.Image = Resources.Male_512;
        }

        public void LoadDriverLicenseInfoByLicenseId(int licenseId)
        {
            _licenseId = licenseId;
            _license = clsLicense.FindLicenseInfoByLicenseId(_licenseId);

            if (_license == null)
            {
                ShowError(
                    $"No issued driving license was found for License ID = {_licenseId}.",
                    "License Not Found");

               
                return;
            }

            _LoadLicenseDataIntoTheControls();
        }
    }
}
