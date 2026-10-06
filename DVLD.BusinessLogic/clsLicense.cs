using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.BusinessLogic
{
    public class clsLicense
    {
       
        public enum enIssueReason
        {
            FirstTime = 1,
            Renew,
            ReplacementForDamaged,
            ReplacementForLost
        }
        enum enMode
        {
            AddNew,
            Update
        }

        enMode Mode;
        public int LicenseID { get; set; }  
        public int ApplicationID    { get; set; }
        public clsApplication ApplicationInfo { get; set; }
        public int DriverID {  get; set; }
        public clsDriver DriverInfo { get; set; }
        public int LicenseClass {  get; set; }
        public clsLicenseClass LicenseClassInfo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }    
        public string Notes { get; set; }   
        public float PaidFess {  get; set; }    
        public bool IsActive {  get; set; }
        public byte IssueReason { get; set; }
        public int CreatedByUserID { get; set; }
        public string IssueReasonText
        {
            get
            {
                switch ((enIssueReason)IssueReason)
                {
                    case enIssueReason.FirstTime:
                        return "First Time";


                    case enIssueReason.Renew:
                        return "Renewal";

                    case enIssueReason.ReplacementForLost:
                        return "Replacement for Lost License";

                    case enIssueReason.ReplacementForDamaged:
                        return "Replacement for Damaged License";

                    default:
                        return "Unknown";
                }
            }


        }

        public bool IsDetained { get; set; }
        public clsLicense()
        {
            LicenseID = -1;
            ApplicationID = -1;
            DriverID = -1;
            LicenseClass = -1;
            IssueDate = DateTime.Now;
            ExpirationDate = DateTime.Now;
            Notes = string.Empty;
            PaidFess = 0;
            IsActive = false;
            IssueReason = 0;
            CreatedByUserID = -1;
            Mode = enMode.AddNew;
        }
        private clsLicense(
            int licenseID,
            int applicationID, 
            int driverID, 
            int licenseClass,
            DateTime issueDate,
            DateTime expirationDate,
            string notes,
            float paidFess, 
            bool isActive,
            byte issueReason,
            int createdByUserID)
        {
            LicenseID = licenseID;
            ApplicationID = applicationID;
            ApplicationInfo = clsApplication.FindBaseApplication(applicationID);
            DriverID = driverID;
            DriverInfo=clsDriver.FindByDriverId(driverID);
            LicenseClass = licenseClass;
            LicenseClassInfo = clsLicenseClass.Find(licenseClass);
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            Notes = notes;
            PaidFess = paidFess;
            IsActive = isActive;
            IssueReason = issueReason;
            CreatedByUserID = createdByUserID;
            IsDetained = clsDetainedLicense.IsDetained(licenseID);
            Mode = enMode.Update;
        }

        private bool _AddNewLicense()
        {

            this.LicenseID = clsLicenseData.AddNewLicense(
                this.ApplicationID,
                this.DriverID,
                this.LicenseClass,
                this.IssueDate,
                this.ExpirationDate,
                this.Notes,
                this.PaidFess,
                this.IsActive,
                this.IssueReason,
                this.CreatedByUserID);

            return (this.LicenseID != 0);
        }
        private bool _UpdateLicense()
        {
            return clsLicenseData.UpdateLicense(
                this.LicenseID,
                this.ApplicationID,
                this.DriverID,
                this.LicenseClass,
                this.IssueDate,
                this.ExpirationDate,
                this.Notes,
                this.PaidFess,
                this.IsActive,
                this.IssueReason,
                this.CreatedByUserID);
        }
        public bool Save()
        {
            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNewLicense())
                    {

                        Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateLicense();


            }

            return false;
        }
        public static bool HisLicense(int applicantPersonId,int licenseClass)
        {
            return clsLicenseData.HisLicense(applicantPersonId, licenseClass);
        }
        public static clsLicense FindLicenseInfoByApplicationId(int applicationId)
        {

            int licenseID = -1, createdByUserID = -1, driverID = -1, licenseClass = -1;
            DateTime issueDate = DateTime.Now, expirationDate = DateTime.Now;
            string notes = string.Empty;
            float paidFess = 0;
            bool isActive = false;
            byte issueReason = 0;

            if (clsLicenseData.GetLicenseInfoByApplicationId(
                applicationId,
               ref licenseID,
               ref driverID,
               ref licenseClass,
               ref issueDate,
               ref expirationDate,
               ref notes,
               ref paidFess,
               ref isActive,
               ref issueReason,
               ref createdByUserID))
            {
                return new clsLicense(
                    licenseID,
                    applicationId,
                    driverID,
                    licenseClass,
                    issueDate,
                    expirationDate,
                    notes,
                    paidFess,
                    isActive,
                    issueReason,
                    createdByUserID);
            }


            return null;
        }
        public static clsLicense FindLicenseInfoByLicenseId(int licenseId)
        {
            int applicationId = -1, createdByUserID = -1, driverID = -1, licenseClass = -1;
            DateTime issueDate = DateTime.Now, expirationDate = DateTime.Now;
            string notes = string.Empty;
            float paidFess = 0;
            bool isActive = false;
            byte issueReason = 0;

            if (clsLicenseData.GetLicenseInfoByLicenseId(
                licenseId,
               ref applicationId,
               ref driverID,
               ref licenseClass,
               ref issueDate,
               ref expirationDate,
               ref notes,
               ref paidFess,
               ref isActive,
               ref issueReason,
               ref createdByUserID))
            {
                return new clsLicense(
                    licenseId,
                    applicationId,
                    driverID,
                    licenseClass,
                    issueDate,
                    expirationDate,
                    notes,
                    paidFess,
                    isActive,
                    issueReason,
                    createdByUserID);
            }


            return null;
        }
        public bool IsExpired()
        {
            return ExpirationDate < DateTime.Now;
        }

        public bool Lock()
        {
            if (!clsLicenseData.LockLicense(this.LicenseID))
                return false;

            this.IsActive = false;
            return true;
        }
        public int Detain(decimal fineFees,int createdByUserId)
        {
            if (IsDetained)
                return -1;

            clsDetainedLicense detainedLicense = new clsDetainedLicense();
            detainedLicense.LicenseID = this.LicenseID;
            detainedLicense.DetainDate = DateTime.Now;
            detainedLicense.FineFees = fineFees;
            detainedLicense.CreatedByUserID = createdByUserId;
            detainedLicense.IsReleased = false;

            if (!detainedLicense.Save())
                return -1;

            return detainedLicense.DetainID;
        }

        public int Release(int licenseId, int releasedByUserId)
        {
            var detainedLicense = clsDetainedLicense.FindByLicenseId(licenseId);

            if( detainedLicense == null ) return -1;


            var releasedLicenseApplication = new clsApplication();

            releasedLicenseApplication.ApplicantPersonID = this.DriverInfo.PersonID;
            releasedLicenseApplication.ApplicationDate = DateTime.Now;
            releasedLicenseApplication.ApplicationTypeID
                = (int)clsApplicationType.enApplicationTypes.ReleaseDetainedDrivingLicsense;
            releasedLicenseApplication.ApplicationStatus
                = (int)clsApplication.enApplicationStatus.Completed;



            releasedLicenseApplication.LastStatusDate = DateTime.Now;

            releasedLicenseApplication.PaidFess
                = clsApplicationType.FindApplicationTypeByID(
                    (int)clsApplicationType.enApplicationTypes.ReleaseDetainedDrivingLicsense).Fees;


            releasedLicenseApplication.CreatedByUserID = releasedByUserId;


            if (!releasedLicenseApplication.Save()) return -1;

         
            detainedLicense.IsReleased = true;
            detainedLicense.ReleaseDate = DateTime.Now;
            detainedLicense.ReleasedByUserID = releasedByUserId;
            detainedLicense.ReleaseApplicationID = releasedLicenseApplication.ApplicationID;

            if(!detainedLicense.Save()) return -1;

            return releasedLicenseApplication.ApplicationID;

        }

    }
}
