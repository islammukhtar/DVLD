using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using DVLD.Presentation.Properties;

namespace DVLD.Presentation.Licenses.International_License.Controls
{
    public partial class ctrlDriverInternationalLicenseInfo : UserControl
    {
        clsInternationalLicense _internationalLicense;
        enum enGender
        {
            Male,
            Female
        }
        public ctrlDriverInternationalLicenseInfo()
        {
            InitializeComponent();
        }
        void _LoadPersonImage()
        {
            short gender
                = _internationalLicense.ApplicationInfo.ApplicantPersonInfo.Gender;
            string imagePath =
                _internationalLicense.ApplicationInfo.ApplicantPersonInfo.ImagePath;

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
        public void LoadInternationalLicenseInfo(int internationalLicense)
        {

            _internationalLicense = clsInternationalLicense.Find(internationalLicense);

            if (_internationalLicense == null)
            {
                MessageBox.Show(
                    $"No international driving license was found with ID = {internationalLicense}.",
                    "International License Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            lblFullName.Text = _internationalLicense.ApplicationInfo.ApplicantPersonInfo.FullName;
            lblInternationalLicenseID.Text = _internationalLicense.InternationalLicenseId.ToString();
            lblLocalLicenseID.Text = _internationalLicense.IssuedUsingLocalLicenseId.ToString();
            lblNationalNo.Text = _internationalLicense.ApplicationInfo.ApplicantPersonInfo.NationalNo;
            lblGender.Text = _internationalLicense.ApplicationInfo.ApplicantPersonInfo.Gender == (short)enGender.Male ? "Male" : "Female";
            lblIssueDate.Text = _internationalLicense.IssueDate.ToShortDateString();
            lblApplicationID.Text = _internationalLicense.ApplicationId.ToString();
            lblIsActive.Text = (_internationalLicense.IsActive) ? "Yes" : "No";
            lblDateOfBirth.Text = _internationalLicense.ApplicationInfo.ApplicantPersonInfo.DateOfBirth.ToShortDateString();
            lblDriverID.Text = _internationalLicense.DriverId.ToString();
            lblExpirationDate.Text = _internationalLicense.ExpirationDate.ToShortDateString();
            _LoadPersonImage();
        }
    }
}
