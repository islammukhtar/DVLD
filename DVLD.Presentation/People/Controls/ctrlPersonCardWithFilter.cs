using System;
using System.ComponentModel;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.People.Controls
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {
     
        public bool FilterEnabled
        {
            set { gbFilter.Enabled = value;  }
        }
        public int PersonID
        {
            get { return ctrlPersonCard1.PersonID; }
        }
        public clsPerson SelectedPersonInfo
        {
            get { return ctrlPersonCard1.SelectedPersonInfo; }
        }
        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();
        }
        public void ResetDefaultValues()
        {
            txtSearch.Text = string.Empty;
            txtSearch.Focus();
        }
        public void LoadPersonInfo(int personId)
        {
            cbFilterBy.SelectedIndex = 1;
            txtSearch.Text = personId.ToString();
            FindNow();
        }

        void FindNow()
        {
            switch (cbFilterBy.Text) {

                case "Person ID":
                    ctrlPersonCard1.LoadPersonInfo(int.Parse(txtSearch.Text));
                    break;
                case "National No":
                    ctrlPersonCard1.LoadPersonInfo(txtSearch.Text);
                    break;
                default:
                    break;
         
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                return;

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid! Please hover over the red icon for details.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            FindNow();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
        }
        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetDefaultValues();
        }
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==(int)13)
            {
                btnFind.PerformClick();
            }

            if (cbFilterBy.SelectedItem.ToString() == "Person ID") 
            {
                e.Handled = !char.IsDigit(e.KeyChar) &&
                            !char.IsControl(e.KeyChar);
            }
        }

        private void txtSearch_Validating(object sender, CancelEventArgs e)
        {
            errorProvider1.SetError(
         txtSearch,
         string.IsNullOrWhiteSpace(txtSearch.Text)
             ? "This field is required!"
             : null);
        }

        private void ctrlPersonCardWithFilter_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
        }
    }
}
