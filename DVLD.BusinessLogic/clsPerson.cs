using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsPerson
    {
        enum enMode { AddNew, Update };
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public short Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NationalNo { get; set; }

        public clsCountry CountryInfo;
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public int NationalityCountryID { get; set; }
        enMode _Mode;
        public clsPerson()
        {
            this.ID = -1;
            this.FirstName = string.Empty;
            this.SecondName = string.Empty;
            this.ThirdName = string.Empty;
            this.LastName = string.Empty;
            this.FullName = string.Empty;
            this.Gender = -1;
            this.Address = string.Empty;
            this.DateOfBirth = DateTime.Now;
            this.NationalNo = string.Empty;
            this.Phone = string.Empty;
            this.Email = string.Empty;
            this._Mode = enMode.AddNew;
        }

        private clsPerson(
            int iD, 
            string nationalNo,
            string firstName,
            string secondName,
            string thirdName,
            string lastName, 
            DateTime dateOfBirth, 
            short gender,
            string address,
            string phone,
            string email,
            int nationalityCountryID, 
            string imagePath)
        {
            ID = iD;
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
            LastName = lastName;
            FullName = $"{firstName} {secondName} {thirdName} {lastName}";
            Gender = gender;
            Address = address;
            DateOfBirth = dateOfBirth;
            NationalNo = nationalNo;
            Phone = phone;
            Email = email;
            NationalityCountryID = nationalityCountryID;
            CountryInfo = clsCountry.Find(nationalityCountryID);
            ImagePath = imagePath;
            _Mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {

            this.ID = clsPersonData.AddNewPerson(this.NationalNo, this.FirstName, this.SecondName, this.ThirdName, this.LastName, this.DateOfBirth, this.Gender,
                this.Address, this.Phone, this.Email, this.NationalityCountryID, this.ImagePath);

            return (this.ID != -1);

        }

        private bool _UpdatePerson()
        {
            return clsPersonData.UpdatePerson(this.ID, this.NationalNo, this.FirstName, this.SecondName, this.ThirdName, this.LastName, this.DateOfBirth, this.Gender,
                this.Address, this.Phone, this.Email, this.NationalityCountryID, this.ImagePath);
        }

        public bool Save()
        {
            switch (_Mode)
            {

                case enMode.AddNew:
                    if (_AddNewPerson())
                    {

                        _Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdatePerson();


            }

            return false;
        }

        public static clsPerson Find(int personID)
        {
            string firstName = "", secondName = "", thirdName = "",
                lastName = "", address = "", phone = "", email = "", imagePath = "", nationalNo = "";
            short Gender = -1;

            int nationalityCountryID = -1;
            DateTime dateOfBirth = DateTime.Now;

            if (clsPersonData.GetPersonInfoByID(personID, ref nationalNo, ref firstName, ref secondName, ref thirdName, ref lastName, ref dateOfBirth, ref Gender, ref address, ref phone, ref email, ref nationalityCountryID, ref imagePath))
            {
                return new clsPerson(personID, nationalNo, firstName, secondName, thirdName, lastName, dateOfBirth, Gender, address, phone, email, nationalityCountryID, imagePath);
            }
            else
            {
                return null;
            }

        }
        public static clsPerson Find(string NationalNo)
        {
            string firstName = "", secondName = "", thirdName = "",
                lastName = "", address = "", phone = "", email = "", imagePath = "";
            short Gender = -1;
            int nationalityCountryID = -1, personID = -1;
            DateTime dateOfBirth = DateTime.Now;

            if (clsPersonData.GetPersonInfoByNationalNo(ref personID, NationalNo, ref firstName, ref secondName, ref thirdName, ref lastName, ref dateOfBirth, ref Gender, ref address, ref phone, ref email, ref nationalityCountryID, ref imagePath))
            {
                return new clsPerson(personID, NationalNo, firstName, secondName, thirdName, lastName, dateOfBirth, Gender, address, phone, email, nationalityCountryID, imagePath);
            }
            else
            {
                return null;
            }

        }
        public static bool DeletePerson(int personID)
        {
           return clsPersonData.DeletePerson(personID);
            
        }

        public static bool IsPersonExist(int personID)
        {
            return clsPersonData.IsPersonExist(personID);
        }

        public static bool IsPersonExist(string nationalNo)
        {
            return clsPersonData.IsPersonExist(nationalNo);
        }

        static public DataTable GetAllPeople()
        {
            return clsPersonData.GetAllPeople();
        }
        public bool IsEqual(clsPerson person)
        {
            return FirstName == person.FirstName &&
                   SecondName == person.SecondName &&
                   ThirdName == person.ThirdName &&
                   LastName == person.LastName &&
                   NationalNo == person.NationalNo &&
                   Phone == person.Phone &&
                   DateOfBirth.Date == person.DateOfBirth.Date &&
                   Email == person.Email &&
                   Address == person.Address &&
                   (ImagePath ?? string.Empty) == (person.ImagePath ?? string.Empty) &&
                   Gender == person.Gender;
        }
    }
}
