using System;
using System.ComponentModel;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using Microsoft.Win32;

namespace DVLD.Presentation.Login
{
    public partial class frmLogin : Form
    {
        private const string RegistryPath = @"SOFTWARE\DVLD";
        private const string RegistryFullPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD";

        private const string UserNameValue = "UserName";
        private const string PasswordValue = "Password";

        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            LoadRememberMeData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Validation

        private void ValidateEmptyTextBox(object sender, CancelEventArgs e)
        {
            if (!(sender is TextBox txt))
                return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txt, "This field is required.");
            }
            else
            {
                errorProvider1.SetError(txt, string.Empty);
            }
        }

        private bool ValidateLoginInputs()
        {
            return ValidateChildren();
        }

        #endregion

        #region Remember Me

        private void LoadRememberMeData()
        {
            string user = Registry.GetValue(RegistryFullPath, UserNameValue, null) as string;
            string password = Registry.GetValue(RegistryFullPath, PasswordValue, null) as string;

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                txtUserName.Text = user;
                txtPassword.Text = password;
                chkRememberMe.Checked = true;
            }
        }

        private bool SaveRememberMeData()
        {
            try
            {
                Registry.SetValue(
                    RegistryFullPath,
                    UserNameValue,
                    txtUserName.Text,
                    RegistryValueKind.String);

                Registry.SetValue(
                    RegistryFullPath,
                    PasswordValue,
                    txtPassword.Text,
                    RegistryValueKind.String);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ClearRememberMeData()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                if (key == null)
                    return;

                key.DeleteValue(UserNameValue, false);
                key.DeleteValue(PasswordValue, false);
            }
        }

        private void HandleRememberMe()
        {
            if (chkRememberMe.Checked)
            {
                if (!SaveRememberMeData())
                {
                    MessageBox.Show(
                        "Failed to save login information.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else
            {
                ClearRememberMeData();
            }
        }

        #endregion

        #region Authentication

        private void ResetAfterFailedLogin(bool clearUserName = false)
        {
            if (clearUserName)
                txtUserName.Clear();

            txtPassword.Clear();
            txtPassword.Focus();
        }

        private bool FailLogin(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                icon);

            ResetAfterFailedLogin();
            return false;
        }

        private bool AuthenticateUser(out clsUser user)
        {
            user = clsUser.Find(txtUserName.Text, txtPassword.Text);

            if (user == null)
            {
                return FailLogin(
                    "Invalid username or password.",
                    "Login Failed",
                    MessageBoxIcon.Error);
            }

            if (!user.IsActive)
            {
                return FailLogin(
                    "Your account is not active. Please contact the administrator.",
                    "Account Disabled",
                    MessageBoxIcon.Warning);
            }

            return true;
        }

        #endregion

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateLoginInputs())
                return;

            if (!AuthenticateUser(out clsUser user))
                return;

            clsGlobal.CurrentUser = user;

            HandleRememberMe();

            using (frmMain frm = new frmMain())
            {
                Hide();
                frm.ShowDialog();
                Show();
            }
        }
    }
}