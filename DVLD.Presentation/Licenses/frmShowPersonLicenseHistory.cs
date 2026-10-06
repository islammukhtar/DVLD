using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Presentation.Licenses
{
    public partial class frmShowPersonLicenseHistory : Form
    {
        int _personId;
        public frmShowPersonLicenseHistory()
        {
            InitializeComponent();
        }
        public frmShowPersonLicenseHistory(int personId)
        {
            InitializeComponent();
            _personId = personId;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void frmLicenseHistory_Load(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.LoadPersonInfo(_personId);
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            ctrlDriverLicenses1.LoadLicensesInfo(_personId);
        }
    }
}
