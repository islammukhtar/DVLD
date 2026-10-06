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
using DVLD.Presentation.Licenses;
using DVLD.Presentation.Licenses.Local_License;

namespace DVLD.Presentation.Applications.ReleasedDetainedLicenseApplication
{
    public partial class frmReleasedDetainedLicenseApplication : Form
    {
        clsLicense _license;
        int _licenseId = -1;
        public frmReleasedDetainedLicenseApplication()
        {
            InitializeComponent();
        }
        public frmReleasedDetainedLicenseApplication(int licenseId)
        {
            InitializeComponent();
            _licenseId = licenseId;
        }

        private void frmReleasedDetainedLicenseApplication_Load(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected
                += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;

            if (_licenseId != -1) {
                ctrlDriverLicenseInfoWithFilter1.LoadDriverLicenseInfo(_licenseId);
                ctrlDriverLicenseInfoWithFilter1.FilterEnable = false;
            }

           
        }

        private void CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected(object sender, EventArgs e)
        {
            _license = clsLicense.FindLicenseInfoByLicenseId((int)sender);

            if (_license == null)
                return;

            if (!_license.IsDetained)
            {
                MessageBox.Show(
                    "The selected driving license is not detained. Only detained licenses can be released.",
                    "License Not Detained",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var detainedLicense = clsDetainedLicense.FindByLicenseId(_license.LicenseID);

            lblDetainID.Text = detainedLicense.DetainID.ToString();
            lblLicenseID.Text = detainedLicense.LicenseID.ToString();
            lblDetainDate.Text = detainedLicense.DetainDate.ToString();
            lblCreatedByUser.Text = clsUser.Find(detainedLicense.CreatedByUserID).UserName;
            decimal fineFees = detainedLicense.FineFees;
            lblFineFees.Text = fineFees.ToString("0.#");
            decimal applicationFees 
                = clsApplicationType.FindApplicationTypeByID(
                    (int)clsApplicationType.enApplicationTypes.ReleaseDetainedDrivingLicsense).Fees;

            lblApplicationFees.Text = applicationFees.ToString("0.#");

            lblTotalFees.Text = (fineFees + applicationFees).ToString("0.#");

            llShowLicenseHistory.Enabled = true;
            btnRelease.Enabled = true;
        }

        private void btnRelease_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
     "Are you sure you want to release this detained driving license?",
     "Release Detained License",
     MessageBoxButtons.YesNo,
     MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;


            int releasedApplication =
    ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Release(
        _license.LicenseID,
        clsGlobal.CurrentUser.UserID);

            if (releasedApplication == -1)
            {
                MessageBox.Show(
                    "Failed to release the driving license. Please try again.",
                    "Release Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
             
            }

            MessageBox.Show(
                 $"The driving license has been released successfully.{Environment.NewLine}{Environment.NewLine}" +
                 $"Release Application ID: {releasedApplication}",
                 "License Released",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Information);

            btnRelease.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnable = false;
            llShowLicenseInfo.Enabled = true;

        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm
                 = new frmShowPersonLicenseHistory(
                     ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID
                     );

            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_license.LicenseID);
            frm.ShowDialog();
        }
  
    }
}
