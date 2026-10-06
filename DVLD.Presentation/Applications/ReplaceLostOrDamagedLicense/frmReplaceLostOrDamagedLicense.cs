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
using DVLD.Presentation.Licenses.Local_License;
using DVLD.Presentation.Licenses;

namespace DVLD.Presentation.Applications.ReplaceLostOrDamagedLicense
{
    public partial class frmReplaceLostOrDamagedLicense : Form
    {
        private decimal _applicationFees;
        private clsLicense _oldLicense;
        private int _newLicenseId;
        public frmReplaceLostOrDamagedLicense()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateFormTitle()
        {
            lblTitle.Text = (rbDamagedLicense.Checked) ? "Replacement for damaged license" :
                "Replacement for lost license";
        }
        private void UpdateApplicationFees()
        {
            _applicationFees =
                clsApplicationType.FindApplicationTypeByID(
                    rbDamagedLicense.Checked
                    ? (int)clsApplicationType.enApplicationTypes.ReplacementforaDamagedDrivingLicense
                    : (int)clsApplicationType.enApplicationTypes.ReplacementforaLostDrivingLicense)
                .Fees;

            lblApplicationFees.Text = _applicationFees.ToString("0.#");
        }
        private void _LoadDefaultValues()
        {

            lblApplicationDate.Text = DateTime.Now.ToShortDateString();

            UpdateFormTitle();
            UpdateApplicationFees();

            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;

        }
        private bool ValidateLicense()
        {
            if (_oldLicense == null)
                return false;

            if (!_oldLicense.IsActive)
            {
                MessageBox.Show(
                    "The selected driving license is inactive. Only an active driving license can be used.",
                    "Inactive License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }
            if (_oldLicense.IsExpired())
            {
                MessageBox.Show(
      $"The selected driving license has expired. Expired driving licenses cannot be replaced.{Environment.NewLine}{Environment.NewLine}" +
      $"Expiration Date: {_oldLicense.ExpirationDate:dd/MM/yyyy}",
      "License Replacement Not Allowed",
      MessageBoxButtons.OK,
      MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        private void CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected(object sender, EventArgs e)
        {
            _oldLicense = clsLicense.FindLicenseInfoByLicenseId((int)sender);

            if (!ValidateLicense())
            {
                btnIssueReplacement.Enabled = false;
                return;
            }

            btnIssueReplacement.Enabled = true;

            lblOldLicenseID.Text = _oldLicense.LicenseID.ToString();


            llShowLicenseHistory.Enabled = (_oldLicense != null);
        }
        private void frmReplaceLostOrDamagedLicense_Load(object sender, EventArgs e)
        {
            rbDamagedLicense.Checked = true;

            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected
                += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;
            _LoadDefaultValues();
        }

        private void rbDamagedLicense_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormTitle();
            UpdateApplicationFees();
        }

        private void rbLostLicense_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormTitle();
            UpdateApplicationFees();
        }
        private clsLicense _CreateReplacementLicense(clsApplication newApplication)
        {
            DateTime issueDate = DateTime.Now;

            return new clsLicense
            {
                ApplicationID = newApplication.ApplicationID,
                DriverID = _oldLicense.DriverID,
                LicenseClass = _oldLicense.LicenseClass,
                IssueDate = issueDate,
                ExpirationDate = issueDate.AddYears(
                    _oldLicense.LicenseClassInfo.DefaultValidityLength),
                PaidFess = _oldLicense.LicenseClassInfo.ClassFees,
                IsActive = true,
                IssueReason =
    (byte)(rbDamagedLicense.Checked
        ? clsLicense.enIssueReason.ReplacementForDamaged
        : clsLicense.enIssueReason.ReplacementForLost),
                CreatedByUserID = clsGlobal.CurrentUser.UserID
            };
        }
        private void btnIssueReplacement_Click(object sender, EventArgs e)
        {
            if (!ValidateLicense())
            {
                return;
            }

            var newApplication = new clsApplication();

            newApplication.ApplicantPersonID = _oldLicense.ApplicationInfo.ApplicantPersonID;
            newApplication.ApplicationDate = DateTime.Now;
            newApplication.ApplicationTypeID =
                (rbDamagedLicense.Checked)
                ? (int)clsApplication.enApplicationType.ReplacementforaDamagedDrivingLicense
                : (int)clsApplication.enApplicationType.ReplacementforaLostDrivingLicense;
            newApplication.ApplicationStatus = (int)clsApplication.enApplicationStatus.Completed;
            newApplication.LastStatusDate = DateTime.Now;
            newApplication.PaidFess = _applicationFees;
            newApplication.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            if (!newApplication.Save())
            {

                MessageBox.Show(
                   "Failed to save application.",
                   "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                return;
            }

            lblApplicationID.Text = newApplication.ApplicationID.ToString();

            var newLicense = _CreateReplacementLicense(newApplication);

            if (!newLicense.Save())
            {
                MessageBox.Show(
                    "Failed to issue the renewed driving license.",
                    "License Issuance Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            _newLicenseId = newLicense.LicenseID;
            lblRreplacedLicenseID.Text = _newLicenseId.ToString();
            if (!_oldLicense.Lock())
            {
                MessageBox.Show(
                    "The renewed driving license was issued successfully, but the previous license could not be deactivated.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            MessageBox.Show(
     $"The driving license has been replaced successfully.\".{Environment.NewLine}{Environment.NewLine}" +
     $"New License ID: {newLicense.LicenseID}",
      "License Replaced",
     MessageBoxButtons.OK,
     MessageBoxIcon.Information);
            llShowLicenseInfo.Enabled = true;
            btnIssueReplacement.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnable = false;

            var internationalLicense
                = clsInternationalLicense.FindInternationalLicenseByLocalLicenseId(_oldLicense.LicenseID);

            if (internationalLicense == null)
                return;

            internationalLicense.IssuedUsingLocalLicenseId = newLicense.LicenseID;

            if (!internationalLicense.UpdateLocalLicense())
            {
                MessageBox.Show(
                    "The international driving license could not be linked to the renewed local driving license.",
                    "Update Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }



        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm
                = new frmShowPersonLicenseHistory(_oldLicense.ApplicationInfo.ApplicantPersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_newLicenseId);
            frm.ShowDialog();
        }
    }
}

