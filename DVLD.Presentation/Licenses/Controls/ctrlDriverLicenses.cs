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
using DVLD.Presentation.Licenses.International_License;
using DVLD.Presentation.Licenses.Local_License;

namespace DVLD.Presentation.Licenses.Controls
{
    public partial class ctrlDriverLicenses : UserControl
    {
        clsDriver _driver;
        public ctrlDriverLicenses()
        {
            InitializeComponent();
        }
        private void ShowWarning(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        private void _LoadLocalDrivingLicense()
        {
            dgvLocalLicenses.DataSource = clsDriver.GetDriverLicense(_driver.DriverID);
            lblLocalLicCount.Text = dgvLocalLicenses.Rows.Count.ToString();
        }
        private void _LoadInternationalDrivingLicense()
        {
            dgvInternationalLicenses.DataSource = clsDriver.GetDriverInternationalLicense(_driver.DriverID);
            lblInternationalLicCount.Text = dgvInternationalLicenses.Rows.Count.ToString();
        }
        public void LoadLicensesInfo(int personId)
        {
            _driver = clsDriver.FindByPersonID(personId);


            if (_driver == null) {

                ShowWarning(
                    "No driver record was found. The selected person is not linked to a driver.",
                    "Driver Not Found");
         
                return;
            }

            _LoadLocalDrivingLicense();
            _LoadInternationalDrivingLicense();
        }
        private void showLicenseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowLicenseInfo frm
                = new frmShowLicenseInfo((int ) dgvLocalLicenses.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
        }

        private void showLicenseInfoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmShowInternationalLicenseInfo frm
                            = new frmShowInternationalLicenseInfo(
                                (int)dgvInternationalLicenses.CurrentRow.Cells[0].Value);
            frm.ShowDialog();   
        }
    }
}