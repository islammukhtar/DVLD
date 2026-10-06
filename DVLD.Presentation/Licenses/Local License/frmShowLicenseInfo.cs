using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Presentation.Licenses.Local_License
{
    public partial class frmShowLicenseInfo : Form
    {
       
        int _localLicenseId;
        public frmShowLicenseInfo( int localLicenseId)
        {
            InitializeComponent();
           
            _localLicenseId = localLicenseId;
        }

        private void frmShowLicenseInfo_Load(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfo1.LoadDriverLicenseInfoByLicenseId(_localLicenseId);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
