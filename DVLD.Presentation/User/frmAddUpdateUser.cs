using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using DVLD.Presentation.Properties;

namespace DVLD.Presentation.User
{
    public partial class frmAddUpdateUser : Form
    {

        enum enMode
        { 
            AddNew,
            Update
        }

        enMode _Mode;
        
        int _userId = -1;

        clsUser _user;
        public frmAddUpdateUser()
        {
            _Mode = enMode.AddNew;
            InitializeComponent();
        }
        public frmAddUpdateUser(int userId)
        {
            _userId = userId;
            _Mode = enMode.Update;
            InitializeComponent();
        }
        private void _ResetDefaultValues()
        {
           
            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New User";
                _user = new clsUser();
                tpLoginInfo.Enabled = false;
            }
            else
            {
                lblTitle.Text = "Update User";
            }

            this.Text = lblTitle.Text;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        void _LoadData()
        {
            _user = clsUser.Find(_userId);

            if (_user == null)
            {
                MessageBox.Show($"No user with id = {_user}", "user Not Found", MessageBoxButtons.OK);
                this.Close();
                return;
            }
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            btnNext.Enabled = false;
            btnSave.Enabled = true;
            ctrlPersonCardWithFilter1.LoadPersonInfo(_user.PersonID);
            lblUserID.Text = _user.UserID.ToString();
            txtUserName.Text = _user.UserName;
            txtPassword.Text = _user.Password;
            txtConfirmPassword.Text = _user.Password;
            chkIsActive.Checked = _user.IsActive;
           
        }
        private void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (_Mode == enMode.Update)
                _LoadData();
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ctrlPersonCardWithFilter1.SelectedPersonInfo == null)
            {
                MessageBox.Show("Please select person before continuing.",
                    "Selection Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!clsUser.IsThePersonLinkedToAUser(ctrlPersonCardWithFilter1.SelectedPersonInfo.ID))
            {
                tpLoginInfo.Enabled = true;
                tcUserInfo.SelectedTab = tpLoginInfo;
                btnSave.Enabled = true;
         
            }
            else
            {
                MessageBox.Show(
                               "This person already has a user account. Please choose another person.",
                               "Selection Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Warning
                              );

                ctrlPersonCardWithFilter1.ResetDefaultValues();
            }
            
        }
        private void ValidateEmptyTextBox(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox Temp = (TextBox)sender;

            errorProvider1.SetError(
         Temp,
         string.IsNullOrWhiteSpace(Temp.Text)
             ? "This field is required!"
             : null);
        }
        private void txtConfirmPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            errorProvider1.SetError(
        txtConfirmPassword,
       txtPassword.Text!=txtConfirmPassword.Text
            ? "Password confirmation does not match password!"
            : null);
        }

        private bool HasChanges()
        {
            return _user.IsActive != chkIsActive.Checked ||
                   _user.Password != txtPassword.Text ||
                   _user.UserName != txtUserName.Text;
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid! Please hover over the red icon for details.",
                   "Validation Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                return;
            }

            if (!HasChanges())
            {
                MessageBox.Show("No changes have been made.");
                return;
            }

            _user.PersonID = ctrlPersonCardWithFilter1.SelectedPersonInfo.ID;
            _user.UserName = txtUserName.Text;
            _user.Password = clsUtil.ComputeHash(txtConfirmPassword.Text);
            _user.IsActive = chkIsActive.Checked;

            if (_user.Save())
            {
                MessageBox.Show("Data saved successfully",
                   "Saved",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);

                lblUserID.Text = _user.UserID.ToString();
            }

        }
    }
}
