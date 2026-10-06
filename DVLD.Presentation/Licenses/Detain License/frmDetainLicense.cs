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

namespace DVLD.Presentation.Licenses.Detain_License
{
    public partial class frmDetainLicense : Form
    {
 
        private clsLicense _license;
        public frmDetainLicense()
        {
            InitializeComponent();
        }

        private void _LoadDefaultValues()
        {
            lblDetainDate.Text = DateTime.Now.ToShortDateString();
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
        }

        private void CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected(object sender, EventArgs e)
        {
          
            _license = clsLicense.FindLicenseInfoByLicenseId((int)sender);
           
            if (_license == null)
                return;


            lblLicenseID.Text = _license.LicenseID.ToString();
           
            llShowLicenseHistory.Enabled = true;

            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.IsDetained)
            {
                MessageBox.Show(
                    "The selected driving license is already detained.",
                    "License Already Detained",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            btnDetain.Enabled = true;
        
      
        }

        private void frmDetainLicense_Load(object sender, EventArgs e)
        {
            _LoadDefaultValues();

            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected
                += CtrlDriverLicenseInfoWithFilter1_OnLicenseSelected;
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm
                = new frmShowPersonLicenseHistory(
                    ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID
                    );

            frm.ShowDialog();

        }

        private void btnDetain_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFineFees.Text))
            {
                MessageBox.Show(
                    "Please enter the fine fees.",
                    "Missing Fine Fees",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtFineFees.Focus();
                return;
            }


            DialogResult result = MessageBox.Show(
        "Are you sure you want to detain this driving license?",
        "Confirm Detention",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;


            decimal fineFees = Convert.ToDecimal(txtFineFees.Text);

            int detainId;

            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo != null)
            {
                detainId = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Detain(
                     fineFees,
                    clsGlobal.CurrentUser.UserID);

                if (detainId == -1)
                {
                    MessageBox.Show(
                        "Failed to detain the selected driving license. Please try again.",
                        "Detain Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                MessageBox.Show(
                    "The driving license has been detained successfully.",
                    "Detain Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                lblDetainID.Text = detainId.ToString();
                btnDetain.Enabled = false;
                ctrlDriverLicenseInfoWithFilter1.FilterEnable = false;
                llShowLicenseInfo.Enabled = true;
            }



        }

        private void txtFineFees_Validating(object sender, CancelEventArgs e)
        {
            errorProvider1.SetError(
          txtFineFees,
          string.IsNullOrWhiteSpace(txtFineFees.Text)
              ? "This field is required!"
              : null);
        }

        private void txtFineFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) &&
                !char.IsControl(e.KeyChar);
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm = new frmShowLicenseInfo(_license.LicenseID);
            frm.ShowDialog();
        }
    }
}
