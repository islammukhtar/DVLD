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

namespace DVLD.Presentation.Licenses.Local_License.Controls
{
    public partial class ctrlDriverLicenseInfoWithFilter : UserControl
    {

        public event EventHandler OnLicenseSelected;
        public clsLicense SelectedLicenseInfo
        {
            get
            {
                return ctrlDriverLicenseInfo1.SelectedLicenseInfo;
            }
        }
        public int LicenseId
        {
            get { return ctrlDriverLicenseInfo1.LicenseId; }
        }

        public ctrlDriverLicenseInfoWithFilter()
        {
            InitializeComponent();
        }
        public bool FilterEnable
        {
            set
            {
                gbFilter.Enabled = value;
            }
        }
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) &&
                !char.IsControl(e.KeyChar);
        }
        private void txtSearch_Validating(object sender, CancelEventArgs e)
        {
            errorProvider1.SetError(
                txtSearch,
                string.IsNullOrWhiteSpace(txtSearch.Text)
                ? "This field is required!"
                : null
                );
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                return;


            int licenseId = int.Parse(txtSearch.Text);

            ctrlDriverLicenseInfo1.LoadDriverLicenseInfoByLicenseId(
             licenseId);
            OnLicenseSelected?.Invoke(licenseId, EventArgs.Empty);
        }
        public void LoadDriverLicenseInfo(int licenseId)
        {
            txtSearch.Text = licenseId.ToString();
            ctrlDriverLicenseInfo1.LoadDriverLicenseInfoByLicenseId(
             licenseId);
            OnLicenseSelected?.Invoke(licenseId, EventArgs.Empty);

        }
        public void FilterFocus()
        {
            txtSearch.Focus();
        }
        public void ResetDefaultValues()
        {
            ctrlDriverLicenseInfo1.ResetDefaultValues();
        }
    }
}
