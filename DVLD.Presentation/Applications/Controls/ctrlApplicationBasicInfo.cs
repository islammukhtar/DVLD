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
using DVLD.Presentation.People;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Presentation.Applications.Controls
{
    public partial class ctrlApplicationBasicInfo : UserControl
    {

        clsApplication _application;

        public int ApplicationID {
            get; 
        }  

        public ctrlApplicationBasicInfo()
        {
            InitializeComponent();
        }
        public void ResetApplicationInfo()
        {
         
            lblApplicationID.Text = "[????]";
            lblStatus.Text = "[????]";
            lblType.Text = "[????]";
            lblFees.Text = "[$$$$]";
            lblApplicant.Text = "[????]";
            lblDate.Text = "[????]";
            lblStatusDate.Text = "[????]";
            lblCreatedByUser.Text = "[????]";

        }
        public void LoadApplicationBasicInfo(int applicationId)
        {

            _application = clsApplication.FindBaseApplication(applicationId);

            if (_application == null) {
                MessageBox.Show(
                    "No Application with ApplicationID = " + applicationId.ToString(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);


                ResetApplicationInfo();
                return;
            }

            FillApplicationInfo();
        }
        private void FillApplicationInfo() {

            lblApplicationID.Text = _application.ApplicationID.ToString();
            lblStatus.Text = _application.StatusText;
            lblFees.Text = _application.ApplicationTypeInfo?.Fees.ToString("0.##");
            lblType.Text = _application.ApplicationTypeInfo?.Title.ToString();
            lblApplicant.Text = _application.ApplicantPersonInfo?.FullName;
            lblDate.Text = _application.ApplicationDate.ToShortDateString();
            lblStatusDate.Text = _application.LastStatusDate.ToShortDateString();
            lblCreatedByUser.Text = _application.CreatedByUserInfo?.UserName;
    
        }
        private void llViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmPersonDetails frm = new frmPersonDetails(_application.ApplicantPersonID);
            frm.ShowDialog();
        }
    }
}
