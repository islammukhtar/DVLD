using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Presentation.Licenses.International_License
{
    public partial class frmShowInternationalLicenseInfo : Form
    {
        int _internationalLicense;
        public frmShowInternationalLicenseInfo(int internationalLicense)
        {
            InitializeComponent();
            _internationalLicense = internationalLicense;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmShowInternationalLicenseInfo_Load(object sender, EventArgs e)
        {
            ctrlDriverInternationalLicenseInfo1.LoadInternationalLicenseInfo(_internationalLicense);
        }
    }
}
