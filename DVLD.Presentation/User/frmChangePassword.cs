using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.User
{
    public partial class frmChangePassword : Form
    {
        int _userId = -1;
        clsUser _user;

        public frmChangePassword(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }


        void _ResetDefaultValues()
        {
            txtCurrectPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtCurrectPassword.Focus();
        }

        private void frmChangePassword_Load(object sender, EventArgs e)
        {

            _ResetDefaultValues();

            _user = clsUser.Find(_userId);

            if (_user == null)
            {
                MessageBox.Show($"No user with id = {_user}", "user Not Found", MessageBoxButtons.OK);
                return;
            }
            ctrlUserCard1.LoadUserInfo(_userId);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCurrectPassword_Validating(object sender, CancelEventArgs e)
        {
            string currentPassword = txtCurrectPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                errorProvider1.SetError(txtCurrectPassword, "This field is required!");
                return;
            }

            if (currentPassword != _user.Password)
            {
                errorProvider1.SetError(txtCurrectPassword, "Current password is wrong!");
                return;
            }

            errorProvider1.SetError(txtCurrectPassword, null);
        }

        private void txtNewPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                errorProvider1.SetError(txtNewPassword, "This field is required!");
                return;
            }

            errorProvider1.SetError(txtNewPassword,null);
        }

        private void txtConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                errorProvider1.SetError(txtConfirmPassword, "This field is required!");
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                errorProvider1.SetError(txtConfirmPassword,
                    "Password confirmation does not match password!");
                return;
            }

            errorProvider1.SetError(txtConfirmPassword,null);
        }

        bool IsValidData()
        {
            return txtCurrectPassword.Text.Trim() == _user.Password &&
                !string.IsNullOrWhiteSpace(txtNewPassword.Text.Trim()) &&
                txtNewPassword.Text.Trim() == txtConfirmPassword.Text.Trim();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidData())
            {
                MessageBox.Show("Please correct the error first.");
                return;
            }

            if (clsUser.ChangePassword(_user.UserID, clsUtil.ComputeHash(txtNewPassword.Text.Trim()))) 
            {
         
                MessageBox.Show("Data saved successfully",
                    "Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erorr: Data is not saved successfully",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }


        }
    }
}
