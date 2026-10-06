using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsInternationalLicense
    {
        enum enMode
        {
            AddNew ,
            Update
        }
        enMode Mode;
        public int InternationalLicenseId { get; set; }
        public int ApplicationId { get; set; }
        public clsApplication ApplicationInfo { get; set; }
        public int DriverId { get; set; }
        public clsDriver DriverInfo { get; set; }
        public int IssuedUsingLocalLicenseId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive {  get; set; } 
        public int CreatedByUserId { get; set; }
        public clsInternationalLicense()
        {
            InternationalLicenseId = -1;
            ApplicationId = -1;  
            DriverId = -1;
            IssuedUsingLocalLicenseId = -1;
            IssueDate = DateTime.Now;
            ExpirationDate = DateTime.Now;
            IsActive = false;
            CreatedByUserId = -1;
            Mode = enMode.AddNew;

        }

        private clsInternationalLicense(
            int internationalLicenseId,
            int applicationId,
            int driverId,
            int issuedUsingLocalLicenseId,
            DateTime issueDate,
            DateTime expirationDate,
            bool isActive,
            int createdByUserId)
        {
            InternationalLicenseId = internationalLicenseId;
            ApplicationId = applicationId;
            ApplicationInfo = clsApplication.FindBaseApplication(applicationId);
            DriverId = driverId;
            DriverInfo = clsDriver.FindByDriverId(driverId);
            IssuedUsingLocalLicenseId = issuedUsingLocalLicenseId;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            IsActive = isActive;
            CreatedByUserId = createdByUserId;
            Mode = enMode.Update;
        }


        public static clsInternationalLicense Find(int internationalLicenseId)
        {

            int applicationId = -1, driverId = -1,
           issuedUsingLocalLicenseId = -1, createdByUserId = -1;
            
            DateTime issueDate = DateTime.Now, expirationDate = DateTime.Now;
           
            bool isActive = false;

            if (clsInternationalLicenseData.GetInternationalLicenseById(
                internationalLicenseId,
               ref applicationId,
               ref driverId,
               ref issuedUsingLocalLicenseId,
               ref issueDate,
               ref expirationDate,
               ref isActive,
               ref createdByUserId)) {


                return new clsInternationalLicense(
                    internationalLicenseId,
                    applicationId,
                    driverId,
                    issuedUsingLocalLicenseId,
                    issueDate,
                    expirationDate,
                    isActive,
                    createdByUserId);
                  
            }
            return null;

        }

        public static clsInternationalLicense FindInternationalLicenseByLocalLicenseId(int localLicenseId)
        {

            int applicationId = -1, driverId = -1, internationalLicenseId = -1,
            createdByUserId = -1;

            DateTime issueDate = DateTime.Now, expirationDate = DateTime.Now;

            bool isActive = false;

            if (clsInternationalLicenseData.GetInternationalLicenseByLocalLicenseId(
               localLicenseId,
               ref internationalLicenseId,
               ref applicationId,
               ref driverId,
               ref issueDate,
               ref expirationDate,
               ref isActive,
               ref createdByUserId))
            {
                return new clsInternationalLicense(
                    internationalLicenseId,
                    applicationId,
                    driverId,
                    localLicenseId,
                    issueDate,
                    expirationDate,
                    isActive,
                    createdByUserId);
            }

            return null;

        }
        bool _AddNewInternationalLicense()
        {
            this.InternationalLicenseId = clsInternationalLicenseData.AddNewInternationalLicense(
                 this.ApplicationId,
                 this.DriverId,
                 this.IssuedUsingLocalLicenseId,
                 this.IssueDate,
                 this.ExpirationDate,
                 this.IsActive,
                 this.CreatedByUserId);

            return (this.InternationalLicenseId != -1);
        }

        bool _UpdateInternationalLicense()
        {
            return clsInternationalLicenseData.UpdateInternationalLicense(
                this.InternationalLicenseId,
                this.ApplicationId,
                this.DriverId,
                this.IssuedUsingLocalLicenseId,
                this.IssueDate,
                this.ExpirationDate,
                this.IsActive,
                this.CreatedByUserId);
        }

        public bool UpdateLocalLicense()
        {
            return clsInternationalLicenseData.UpdateLocalLicenseId
                (this.InternationalLicenseId, this.IssuedUsingLocalLicenseId);
        }
        public bool Save()
        {
            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNewInternationalLicense())
                    {

                        Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateInternationalLicense();


            }

            return false;
        }

        public static bool IsThereAnActiveInternationalLicense(int licenseId)
        {
            return clsInternationalLicenseData.HasInternationalLicense(licenseId);
        }

        

    }
}
