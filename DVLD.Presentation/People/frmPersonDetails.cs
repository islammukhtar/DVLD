using System;
using System.Windows.Forms;

namespace DVLD.Presentation.People
{
    public partial class frmPersonDetails : Form
    {
     
        public frmPersonDetails(int personId)
        {
            InitializeComponent();
            ctrlPersonCard1.LoadPersonInfo(personId);
        }
        public frmPersonDetails(string nationalNo)
        {
            InitializeComponent();
            ctrlPersonCard1.LoadPersonInfo(nationalNo);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
