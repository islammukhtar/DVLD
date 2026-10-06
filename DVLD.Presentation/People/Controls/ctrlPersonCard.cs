using System.Windows.Forms;
using DVLD.Presentation.Properties;
using DVLD.BusinessLogic;
using System.IO;

namespace DVLD.Presentation.People.Controls
{
    public partial class ctrlPersonCard : UserControl
    {
        enum enGender { Male, Female }

        int _PersonID;

        clsPerson _Person;
        public int PersonID
        {
            get { return _PersonID; }
        }
        public clsPerson SelectedPersonInfo
        {
            get { return _Person; }
        }
        public ctrlPersonCard()
        {
            InitializeComponent();
        }
        void FillPersonInfo()
        {
            llEditPersonInfo.Visible = true;
            lblPersonID.Text = _Person.ID.ToString();
            lblFullName.Text = _Person.FullName;
            lblNationalNo.Text = _Person.NationalNo;
            lblEmail.Text = _Person.Email != string.Empty ? _Person.Email : "Unkown";
            lblGender.Text = _Person.Gender == 0 ? "Male" : "Female";
            lblDateOfBirth.Text = _Person.DateOfBirth.ToShortDateString();
            lblPhone.Text = _Person.Phone;
            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID).CountryName;
            lblAddress.Text = _Person.Address;
            LoadPersonImage();
        }
        void LoadPersonImage()
        {

            if (_Person.Gender == (short)enGender.Male)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            if (_Person.ImagePath != string.Empty)
                if (File.Exists(_Person.ImagePath))
                    pbPersonImage.ImageLocation = _Person.ImagePath;
                else
                    MessageBox.Show(
                        "Could not find this image " + _Person.ImagePath,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);


        }
        public void LoadPersonInfo(int personId)
        {
            _Person = clsPerson.Find(personId);

            if (_Person == null)
            {
                MessageBox.Show(
                   $"No person with id {personId}",
                   "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);

                return;
            }

            FillPersonInfo();

        }
        public void LoadPersonInfo(string nationalNo)
        {
            _Person = clsPerson.Find(nationalNo);

            if (_Person == null)
            {
                MessageBox.Show(
                   $"No person with national no  {nationalNo}",
                   "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);

                return;
            }

            FillPersonInfo();
        }
        private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson(_Person.ID);
            frm.ShowDialog();
            //refresh
            LoadPersonInfo(_Person.ID);
        }
    }
}
