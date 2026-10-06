using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.User.Controls
{
    public partial class ctrlUserCard : UserControl
    {
        clsUser _user;
        public ctrlUserCard()
        {
            InitializeComponent();
        }

        public void LoadUserInfo(int userId)
        {
            _user = clsUser.Find(userId);

            if (_user == null)
            {
                MessageBox.Show($"No user with id = {_user}", "user Not Found", MessageBoxButtons.OK);
                return;
            }
            _FillUserInfo();
        }

        void _FillUserInfo()
        {
            ctrlPersonCard1.LoadPersonInfo(_user.PersonID);
            lblUserID.Text = _user.UserID.ToString();
            lblUserName.Text = _user.UserName.ToString();

            if (_user.IsActive)
                lblIsActive.Text = "Yes";
            else
                lblIsActive.Text = "No";
        }
    }
}
