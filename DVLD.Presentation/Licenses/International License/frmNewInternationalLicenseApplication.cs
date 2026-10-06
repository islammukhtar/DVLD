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
using DVLD.Presentation.People.Controls;
using static System.Net.Mime.MediaTypeNames;
using static DVLD.BusinessLogic.clsLicenseClass;

namespace DVLD.Presentation.Licenses.International_License
{
    public partial class frmNewInternationalLicenseApplication : Form
    {
        clsLicense _license;
        decimal _applicationFees;
        int _internationalLicenseId;
        public frmNewInternationalLicenseApplication()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _ResetDefaultValues()
        {
            ctrlDriverLicenseInfoWithFilter1.ResetDefaultValues();
            lblApplicationID.Text = "[???]";
            lblInternationalLicenseID.Text = "[???]";
            lblLocalLicenseID.Text = "[???]";
            lblCreatedByUser.Text = "[????]";
            lblApplicationDate.Text = "[??/??/????]";
            lblIssueDate.Text = "[??/??/????]";
            lblExpirationDate.Text = "[??/??/????]";
            lblFees.Text = "[$$$]";
            llShowLicenseInfo.Enabled= false;
            llShowLicenseHistory.Enabled= false;
        }
        private void _LoadDefaultValues()
        {

            lblApplicationDate.Text = DateTime.Now.ToShortDateString();
            lblIssueDate.Text = DateTime.Now.ToShortDateString();
            lblExpirationDate.Text = DateTime.Now.AddYears(1).ToShortDateString();
            _applicationFees = clsApplicationType.FindApplicationTypeByID(
                    (int)clsApplication.enApplicationType.NewInternationalLicense).Fees;
           
            lblFees.Text = _applicationFees.ToString("0.#");

        
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;

            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;
           

        }
        private bool ValidateLicense()
        {
            if (_license == null)
                return false;

            if (_license.LicenseClass != (int)clsLicenseClass.enLicenseClass.Ordinarydrivinglicense)
            {
                MessageBox.Show(
                    "Only an ordinary driving license can be used to issue an international driving license.",
                    "Ordinary License Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (!_license.IsActive)
            {
                MessageBox.Show(
                    "The selected driving license is inactive. Only an active driving license can be used to issue an international driving license.",
                    "Inactive License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (_license.IsExpired())
            {
                MessageBox.Show(
                    "The selected driving license has expired. Please renew it before applying for an international driving license.",
                    "Expired License",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (clsInternationalLicense.IsThereAnActiveInternationalLicense(_license.LicenseID))
            {
                MessageBox.Show(
                    "An active international driving license already exists for this driving license.",
                    "International License Already Exists",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return false;
            }
            btnIssue.Enabled = true;
            return true;
        }
        private void CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected(object sender, EventArgs e)
        {
            _license = clsLicense.FindLicenseInfoByLicenseId((int)sender);

            if (!ValidateLicense())
            {
                return;
            }

            lblLocalLicenseID.Text = _license.LicenseID.ToString();

            llShowLicenseHistory.Enabled = (_license != null);

        }
        private void frmNewInternationalLicenseApplication_Load(object sender, EventArgs e)
        {
            _LoadDefaultValues();   
        }
        public static clsInternationalLicense Create(
         clsApplication application,
         clsLicense license,
         int createdByUserId)
        {
            DateTime issueDate = DateTime.Now;

            return new clsInternationalLicense
            {
                ApplicationId = application.ApplicationID,
                DriverId = license.DriverID,
                IssuedUsingLocalLicenseId = license.LicenseID,
                IssueDate = issueDate,
                ExpirationDate = issueDate.AddYears(1),
                IsActive = true,
                CreatedByUserId = createdByUserId
            };
        }
        private void btnIssue_Click(object sender, EventArgs e)
        {

            if (!ValidateLicense())
            {
                return;
            }

            var newApplication = new clsApplication();

            newApplication.ApplicantPersonID = _license.ApplicationInfo.ApplicantPersonID;
            newApplication.ApplicationDate = DateTime.Now;
            newApplication.ApplicationTypeID = (int)clsApplication.enApplicationType.NewInternationalLicense;
            newApplication.ApplicationStatus = (int)clsApplication.enApplicationStatus.Completed;
            newApplication.LastStatusDate = DateTime.Now;
            newApplication.PaidFess = _applicationFees;
            newApplication.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            if (!newApplication.Save())
            {
                lblApplicationID.Text = newApplication.ApplicationID.ToString();

                MessageBox.Show(
                   "Failed to save application.",
                   "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                return;
            }
          

            var newInternationalLicense = Create(
              newApplication,
              _license,
              clsGlobal.CurrentUser.UserID);
             

            if (newInternationalLicense.Save()) {

                MessageBox.Show(
                $"The international driving license has been issued successfully.\n\nInternational License ID: {newInternationalLicense.InternationalLicenseId}",
                "Operation Completed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                _internationalLicenseId = newInternationalLicense.InternationalLicenseId;
                llShowLicenseInfo.Enabled = true;
            }
            else
            {
                MessageBox.Show(
               "Failed to issue the international driving license.",
               "Operation Failed",
               MessageBoxButtons.OK,
               MessageBoxIcon.Error);
            }


        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            int personId = _license.ApplicationInfo.ApplicantPersonID;
            frmShowPersonLicenseHistory frm =new frmShowPersonLicenseHistory(personId);
            frm.ShowDialog();

        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowInternationalLicenseInfo frm
                = new frmShowInternationalLicenseInfo(_internationalLicenseId);
            frm.ShowDialog();
        }
    }
}
