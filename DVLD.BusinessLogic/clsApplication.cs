using System;
using System.Diagnostics.Contracts;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices.WindowsRuntime;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsApplication
    {
        public enum enApplicationType
        {
            NewLocalDrivingLicense = 1,
            RenewDrivingLicense,
            ReplacementforaLostDrivingLicense,
            ReplacementforaDamagedDrivingLicense,
            ReleaseDetainedDrivingLicsense,
            NewInternationalLicense,
            RetakeTest

        }
       
        public enum enApplicationStatus { 
            New = 1,
            Cancelled ,
            Completed
        }
        protected enum enMode { AddNew ,Update}
        protected enMode Mode;
        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }
        public clsPerson ApplicantPersonInfo {
            get;  
        }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public clsApplicationType ApplicationTypeInfo {
            get; 
        }
        public int ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFess { get; set; }
        public int CreatedByUserID { get; set; }
        public clsUser CreatedByUserInfo {
            get;
        }

        public string StatusText
        {
            get
            {
                switch (ApplicationStatus)
                {
                    case 1:
                        return "New";
                        
                    case 2:
                        return "Cancelled";
                     
                    case 3:
                        return "Completed";
                    default:
                       return "Unkown";
                      
                }
            }
        }
        public clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.ApplicationStatus = -1;
            this.LastStatusDate = DateTime.Now;
            this.PaidFess = 0;
            this.CreatedByUserID = -1;
            this.Mode = enMode.AddNew;
        }

        protected clsApplication(int applicationId,int applicatPersonId,DateTime applicationDate,int applicatonTypeId,
            int applicationStatus,DateTime lastStatusDate,decimal paidFess,int createdByUserId)
        {
            this.ApplicationID = applicationId;
            this.ApplicantPersonID = applicatPersonId;
            this.ApplicantPersonInfo = clsPerson.Find(applicatPersonId);
            this.ApplicationDate = applicationDate;
            this.ApplicationTypeID = applicatonTypeId;
            this.ApplicationTypeInfo = clsApplicationType.FindApplicationTypeByID(applicatonTypeId);
            this.ApplicationStatus = applicationStatus;
            this.LastStatusDate = lastStatusDate;
            this.LastStatusDate=lastStatusDate;
            this.PaidFess = paidFess;
            this.CreatedByUserID = createdByUserId;
            this.CreatedByUserInfo = clsUser.Find(createdByUserId);
            this.Mode = enMode.Update;
        }

        private bool _AddNewApplication()
        {
            this.ApplicationID = clsApplicationData.AddNewApplication(this.ApplicantPersonID, this.ApplicationDate, this.ApplicationTypeID,
                this.ApplicationStatus, this.LastStatusDate, this.PaidFess, this.CreatedByUserID);

            return (this.ApplicationID != -1);
        }
        private bool _UpdateApplication()
        {
            return clsApplicationData.UpdateApplication(this.ApplicationID, this.ApplicantPersonID, this.ApplicationDate, this.ApplicationTypeID,
                  this.ApplicationStatus, this.LastStatusDate, this.PaidFess, this.CreatedByUserID);
        }
        public bool Save()
        {
            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNewApplication())
                    {

                        Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateApplication();


            }

            return false;
        }

        public static bool CompletingApplication(int applicationID)
        {
            return clsApplicationData.UpdateStatus(applicationID, (int)enApplicationStatus.Completed);
        }
        public static bool CancelApplication(int applicationID)
        {
            return clsApplicationData.UpdateStatus(applicationID, (int)enApplicationStatus.Cancelled);
        }
        static public bool DeleteApplication(int applicationID) {
            return clsApplicationData.DeleteApplication(applicationID);
        }
        static public bool IsApplicatonExist(int applicationID) {
           return clsApplicationData.IsApplicationExist(applicationID);
        
        }
        static public clsApplication FindBaseApplication(int applicationID) {

            int applicantPersonID = -1, applicationTypeID = -1, applicationStatus = -1, createdByUserID = -1;
            DateTime applicationDate = DateTime.Now, lastApplicationDate = DateTime.Now;
            decimal paidFees = 0;

            if (clsApplicationData.GetApplicationInfoByID(applicationID,ref applicantPersonID,ref applicationDate,
                ref applicationTypeID,ref applicationStatus,ref lastApplicationDate,ref paidFees,ref createdByUserID))
            {
                return new clsApplication(applicationID, applicantPersonID, applicationDate, applicationTypeID,
                    applicationStatus, lastApplicationDate, paidFees, createdByUserID);
            }
            return null;
        }
        static public int GetActiveApplicationForLicenseClass(
            int applicantPersonId,
            int applicationTypeId,
            int licenseClassID)
        {
            return clsApplicationData.GetActiveApplicationIdForLicenseClass(applicantPersonId,applicationTypeId,licenseClassID);    
        }

    }
}
