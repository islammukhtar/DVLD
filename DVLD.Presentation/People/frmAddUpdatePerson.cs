using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using DVLD.BusinessLogic;
using DVLD.Presentation.Properties;

namespace DVLD.Presentation.People
{
    public partial class frmAddUpdatePerson : Form
    {
        enum enGender { Male, Female };
        enum enMode { AddNew, Update };

        private enMode _Mode;

        private int _PersonID;

        clsPerson _Person;
        public frmAddUpdatePerson()
        {
            _Mode = enMode.AddNew;
            InitializeComponent();

        }
        public frmAddUpdatePerson(int PersonID)
        {
            _PersonID = PersonID;
            _Mode = enMode.Update;


            InitializeComponent();
        }

        private void _FillCountriesInComboBox()
        {
            DataTable dtCountries = clsCountry.GetAllCountries();

            foreach (DataRow row in dtCountries.Rows)
            {
                cbCountry.Items.Add(row["CountryName"]);
            }

        }
        private void _ResetDefualtValues()
        {
            _FillCountriesInComboBox();

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Person";
                _Person = new clsPerson();
               
            }
            else
            {
                lblTitle.Text = "Update Person";
            }

            pbPersonImage.Image = Resources.Male_512;

            //hide/show remove link incase there is no image for the person
            llRemoveImage.Visible = (pbPersonImage.ImageLocation != null);

            //we set the max date to 18 years from today,and set the defualt value the same 
            dtpDateOfBirth.MaxDate = DateTime.Now.Date.AddYears(-18);
            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;

            //should not allow adding age more than 100 years
            dtpDateOfBirth.MinDate = DateTime.Now.Date.AddYears(-100);

            //this we set defualt country to egypt.
            cbCountry.SelectedIndex = cbCountry.FindString("Egypt");

            txtFirstName.Text = string.Empty;
            txtSecondName.Text = string.Empty;
            txtThirdName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtNationalNo.Text = string.Empty;
            txtAddress.Text = string.Empty;
            rbMale.Checked = true;

        }
        private void _LoadData()
        {
            _Person = clsPerson.Find(_PersonID);

            if (_Person == null)
            {
                MessageBox.Show($"No person with id = {_Person}", "Perosn Not Found", MessageBoxButtons.OK);
                this.Close();
                return;
            }

            // the following code will not be executed if the person was not found 
            lblPersonID.Text = _Person.ID.ToString();
            txtFirstName.Text = _Person.FirstName;
            txtSecondName.Text = _Person.SecondName;
            txtThirdName.Text = _Person.ThirdName;
            txtLastName.Text = _Person.LastName;
            txtNationalNo.Text = _Person.NationalNo;
            txtAddress.Text = _Person.Address;
            txtEmail.Text = _Person.Email;
            txtPhone.Text = _Person.Phone;
            dtpDateOfBirth.Value = _Person.DateOfBirth;

            if (_Person.Gender == 0)
            {
                rbMale.Checked = true;
            }
            else
            {
                rbFemale.Checked = true;
            }

            cbCountry.SelectedIndex = cbCountry.FindString(clsCountry.Find(_Person.NationalityCountryID).CountryName);
            //load perons image incase it was set
            if (_Person.ImagePath != "")
            {
                pbPersonImage.ImageLocation = _Person.ImagePath;
            }
            else
            {
                pbPersonImage.Image = (_Person.Gender == 0) ? Resources.Male_512 : Resources.Female_512;
            }

            //hide/show remove link incase there is no image for the person
            llRemoveImage.Visible = (_Person.ImagePath != "");

        }
        private void frmAddUpdatePerson_Load(object sender, EventArgs e)
        {
            _ResetDefualtValues();

            if (_Mode == enMode.Update)
                _LoadData();
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

            if (!HandlePersonImage())
                return;

            if (!HasChanges())
            {
                MessageBox.Show("No changes have been made.");
                return;
            }
               
            int NationalityCountryID = clsCountry.Find(cbCountry.Text).ID;

            _Person.FirstName = txtFirstName.Text.Trim();
            _Person.SecondName = txtSecondName.Text.Trim();
            _Person.ThirdName = txtThirdName.Text.Trim();
            _Person.LastName = txtLastName.Text.Trim();
            _Person.NationalNo = txtNationalNo.Text.Trim();
            _Person.Address = txtAddress.Text.Trim();
            _Person.Email = txtEmail.Text.Trim();
            _Person.Phone = txtPhone.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth.Value;
            _Person.NationalityCountryID = NationalityCountryID;

            if (rbMale.Checked)
            {
                _Person.Gender = (short)enGender.Male;
            }
            else
            {
                _Person.Gender = (short)enGender.Female;
            }


            if (pbPersonImage.ImageLocation != null)
            {
                _Person.ImagePath = pbPersonImage.ImageLocation.ToString();
            }
            else
            {
                _Person.ImagePath = string.Empty;
            }


            if (_Person.Save())
            {
                lblPersonID.Text = _Person.ID.ToString();
                //chanage form mode to update
                _Mode = enMode.Update;
                lblTitle.Text = "Update Person";

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
        private void ValidateEmptyTextBox(object sender, CancelEventArgs e)
        {
            TextBox Temp = (TextBox)sender;
            if (string.IsNullOrEmpty(Temp.Text))
            {
                //e.Cancel = true;
                errorProvider1.SetError(Temp, "This field is required!");
            }
            else
            {
                errorProvider1.SetError(Temp, null);
            }

        }
        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            //no need to validate the email incase it's empty.
            if (txtEmail.Text.Trim() == string.Empty)
                return;

            //validate email format
            if (!clsValidation.IsValidEmail(txtEmail.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "Invalid email address format!");
            }
            else
            {
                errorProvider1.SetError(txtEmail, null);
            }

        }
        private void txtNationalNo_Validating(object sender, CancelEventArgs e)
        {

            string NationalNo = txtNationalNo.Text.Trim();

            if (string.IsNullOrWhiteSpace(NationalNo))
            {
                //e.Cancel = true;
                errorProvider1.SetError(txtNationalNo, "This field is required!");
                return;
            }

            //make sure the national number is not used by another person
            if (NationalNo != _Person.NationalNo && clsPerson.IsPersonExist(NationalNo))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNationalNo, "National number is used for another person!");
                return;
            }

            errorProvider1.SetError(txtNationalNo, null);
        }
        private void rbMale_CheckedChanged(object sender, EventArgs e)
        {
            pbPersonImage.Image = Resources.Male_512;
        }
        private void rbFemale_CheckedChanged(object sender, EventArgs e)
        {
            pbPersonImage.Image = Resources.Female_512;
        }
        private void llSetPicture_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pbPersonImage.ImageLocation = openFileDialog1.FileName;
                llRemoveImage.Visible = true;
            }

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool HasChanges()
        {
            return !_Person.IsEqual(_GetPersonFromScreen()) ||
                   _Person.CountryInfo?.CountryName != cbCountry.Text;
        }
        private void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pbPersonImage.ImageLocation = null;

            if (rbMale.Checked)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            llRemoveImage.Visible = false;
        }
        private bool HandlePersonImage()
        {
            if (pbPersonImage.ImageLocation == _Person.ImagePath)
                return true;

            if (!string.IsNullOrEmpty(_Person.ImagePath))
            {
                try
                {
                    File.Delete(_Person.ImagePath);
                }
                catch
                {
                    // Log Error
                }
            }

            if (string.IsNullOrEmpty(pbPersonImage.ImageLocation))
                return true;

            string sourceImageFile = pbPersonImage.ImageLocation;

            if (!clsUtil.CopyImageToProjectImageFolder(ref sourceImageFile))
            {
                MessageBox.Show(
                    "Error copying image file.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            pbPersonImage.ImageLocation = sourceImageFile;

            return true;
        }
        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar);
        }
        private clsPerson _GetPersonFromScreen()
        {
            return new clsPerson
            {
                FirstName = txtFirstName.Text.Trim(),
                SecondName = txtSecondName.Text.Trim(),
                ThirdName = txtThirdName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                NationalNo = txtNationalNo.Text.Trim(),
                DateOfBirth = dtpDateOfBirth.Value.Date,
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                ImagePath = pbPersonImage.ImageLocation ?? string.Empty,
                Gender = rbMale.Checked
                            ? (short)enGender.Male
                            : (short)enGender.Female
            };
        }
    }
}
 