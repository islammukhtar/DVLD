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

namespace DVLD.Presentation.Applications.Renew_Local_License
{
    public partial class frmRenewLocalDrivingLicenseApplication : Form
    {
        public frmRenewLocalDrivingLicenseApplication()
        {
            InitializeComponent();
        }

        private decimal _applicationFees;
        private clsLicense _oldLicense;
        private int _newLicenseId;

        private void _LoadDefaultValues()
        {
          
            lblApplicationDate.Text = DateTime.Now.ToShortDateString();

            lblIssueDate.Text = DateTime.Now.ToShortDateString();

            _applicationFees = clsApplicationType.FindApplicationTypeByID(
                   (int)clsApplicationType.enApplicationTypes.RenewDrivingLicenseService).Fees;

            lblApplicationFees.Text = _applicationFees.ToString("0.#");
            
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;

            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected 
                += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;

        }
        private bool ValidateLicense()
        {
            if (_oldLicense == null)
                return false;

            if (!_oldLicense.IsActive)
            {
                MessageBox.Show(
                    "The selected driving license is inactive. Only an active driving license can be used to issue an international driving license.",
                    "Inactive License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!_oldLicense.IsExpired())
            {
                MessageBox.Show(
             $"The selected driving license has not expired yet. Only expired licenses can be renewed.{Environment.NewLine}{Environment.NewLine}" +
             $"This license will expire on: {_oldLicense.ExpirationDate:dd/MM/yyyy}",
             "License Renewal Not Allowed",
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
                btnRenewLicense.Enabled = false;
                return;
            }

            btnRenewLicense.Enabled = true;

            lblOldLicenseID.Text = _oldLicense.LicenseID.ToString();

            lblExpirationDate.Text
                = DateTime.Now.AddYears(_oldLicense.LicenseClassInfo.DefaultValidityLength).ToShortDateString();
            
            lblLicenseFees.Text = _oldLicense.LicenseClassInfo.ClassFees.ToString();

            lblTotalFees.Text
                = (Convert.ToSingle(lblLicenseFees.Text) + Convert.ToSingle(lblApplicationFees.Text)).ToString();
           

            llShowLicenseHistory.Enabled = (_oldLicense != null);
            
        }
        private void frmRenewLocalDrivingLicenseApplication_Load(object sender, EventArgs e)
        {
            _LoadDefaultValues();
        }
        private clsLicense CreateLicense(clsApplication newApplication)
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
                Notes = txtNotes.Text.Trim(),
                PaidFess = _oldLicense.LicenseClassInfo.ClassFees,
                IsActive = true,
                IssueReason = (byte)clsLicense.enIssueReason.Renew,
                CreatedByUserID = clsGlobal.CurrentUser.UserID
            };
        }
        private void btnRenewLicense_Click(object sender, EventArgs e)
        {

            if (!ValidateLicense())
            {
                return;
            }

            var newApplication = new clsApplication();

            newApplication.ApplicantPersonID = _oldLicense.ApplicationInfo.ApplicantPersonID;
            newApplication.ApplicationDate = DateTime.Now;
            newApplication.ApplicationTypeID = (int)clsApplication.enApplicationType.RenewDrivingLicense;
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
            
            var newLicense = CreateLicense(newApplication);

            if (!newLicense.Save())
            {
                MessageBox.Show(
                    "Failed to issue the renewed driving license.",
                    "License Issuance Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

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
     $"The driving license has been renewed successfully.{Environment.NewLine}{Environment.NewLine}" +
     $"New License ID: {newLicense.LicenseID}",
     "License Renewed",
     MessageBoxButtons.OK,
     MessageBoxIcon.Information);
            llShowLicenseInfo.Enabled = true;
            btnRenewLicense.Enabled = false;
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
