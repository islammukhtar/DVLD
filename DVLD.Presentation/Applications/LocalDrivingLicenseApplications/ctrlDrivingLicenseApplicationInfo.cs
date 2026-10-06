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
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Presentation.Applications.LocalDrivingLicenseApplications
{
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {

        clsLocalDrivingLicenseApplication _application;
        clsLicense _license;
        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }

        public void LoadDrivingLicenseApplicationInfo(int LocalDrivingLicenseAppID)
        {

            _application = clsLocalDrivingLicenseApplication.Find(LocalDrivingLicenseAppID);
            if (_application == null)
            {
                MessageBox.Show(
                    "No Application with ApplicationID = " + LocalDrivingLicenseAppID.ToString(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            FillDrivingLicenseApplicationInfo();
        }

        private void FillDrivingLicenseApplicationInfo()
        {
            ctrlApplicationBasicInfo1.LoadApplicationBasicInfo(_application.ApplicationID);
            lblPassedTests.Text = _application.GetPassedTestsCount().ToString() + " \\ 3";
            lblLocalDrivingLicenseApplicationID.Text = _application.LocalDrivingLicenseApplicationID.ToString();
            lblAppliedFor.Text = clsLicenseClass.Find(_application.LicenseClassID).ClassName;

            _license
                  = clsLicense.FindLicenseInfoByApplicationId(_application.ApplicationID);

            llShowLicenceInfo.Enabled = (_license != null);
           
        }

        private void llShowLicenceInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            frmShowLicenseInfo frm = new frmShowLicenseInfo(_license.LicenseID);
            frm.ShowDialog();
            
        }
    }
}
