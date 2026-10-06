using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsUser
    {
        enum enMode { AddNew, Update };

        enMode _Mode = enMode.AddNew;
        public int UserID { get; set; }
        public int PersonID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public clsPerson PersonInfo 
        { 
            get;
        }
        public clsUser()
        {

            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = string.Empty;
            this.FullName = string.Empty;
            this.Password = string.Empty;
            this.IsActive = false;
            this._Mode = enMode.AddNew;

        }

        clsUser(int userID, int personID, string userName, string password, bool isActive)
        {
            this.UserID = userID;
            this.PersonID = personID;
            this.UserName = userName;
            this.Password = password;
            this.IsActive = isActive;
            this.PersonInfo = clsPerson.Find(personID);
            this._Mode = enMode.Update;
        }

        static public DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }

        static public bool isUserExist(int userID)
        {
            return clsUserData.IsUserExist(userID);
        }

        private bool _AddNewUser()
        {

            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName, this.Password, this.IsActive);
            return (this.UserID != -1);

        }

        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(this.UserID, this.UserName, this.Password, this.IsActive);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateUser();


            }

            return false;
        }
        static public bool IsThePersonLinkedToAUser(int personId)
        {
            return clsUserData.IsThePersonLinkedToAUser(personId);
        }
        static public clsUser Find(int userId)
        {
            int personID = -1;
            string userName = string.Empty, password = string.Empty;
            bool isActive = false;


            if (clsUserData.GetUserInfoByID(userId, ref personID, ref userName, ref password, ref isActive))
            {
                return new clsUser(userId, personID, userName, password, isActive);
            }
            else
            {
                return null;
            }
        }
        static public clsUser Find(string userName)
        {
            int personID = -1, userID = -1;
            string password = string.Empty;
            bool isActive = false;


            if (clsUserData.GetUserInfoByUserName(userName, ref userID, ref personID, ref password, ref isActive))
            {
                return new clsUser(userID, personID, userName, password, isActive);
            }
            else
            {
                return null;
            }
        }
        static public clsUser Find(string userName,string password)
        {
            int personID = -1, userID = -1;
            bool isActive = false;

            if (clsUserData.GetUserInfoByUserNameAndPassword(userName, password, ref userID, ref personID, ref isActive))
            {
                return new clsUser(userID, personID, userName, password, isActive);
            }
            else
            {
                return null;
            }
        }
        static public bool DeleteUser(int userID)
        {
            return clsUserData.DeleteUser(userID);
        }
        static public bool IsUserActive(string userName)
        {
            return clsUserData.IsUserIsActive(userName);
        }
        static public bool ChangePassword(int userId,string newPassword)
        {
            return clsUserData.ChangePassword(userId,newPassword);
        }
    }
}
