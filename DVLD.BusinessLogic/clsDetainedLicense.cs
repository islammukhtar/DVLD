using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsDetainedLicense
    {
        private enum enMode
        {
            AddNew,
            Update
        }
        private enMode Mode;
        public int DetainID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public decimal FineFees { get; set; }

        public int CreatedByUserID { get; set; }

        public bool IsReleased { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int? ReleasedByUserID { get; set; }

        public int? ReleaseApplicationID { get; set; }
        public clsDetainedLicense()
        {
            DetainID = -1;
            LicenseID = -1;
            DetainDate = DateTime.Now;
            FineFees = 0;
            CreatedByUserID = -1;
            IsReleased = false;
            ReleaseDate = null;
            ReleasedByUserID = null;
            ReleaseApplicationID = null;
            Mode = enMode.AddNew;
        }
        private clsDetainedLicense(
            int detainID,
            int licenseID,
            DateTime detainDate,
            decimal fineFees,
            int createdByUserID,
            bool isReleased,
            DateTime? releaseDate,
            int? releasedByUserID,
            int? releaseApplicationID)
        {
            DetainID = detainID;
            LicenseID = licenseID;
            DetainDate = detainDate;
            FineFees = fineFees;
            CreatedByUserID = createdByUserID;
            IsReleased = isReleased;
            ReleaseDate = releaseDate;
            ReleasedByUserID = releasedByUserID;
            ReleaseApplicationID = releaseApplicationID;
            Mode = enMode.Update;
        }

        private bool _AddNewDetainedLicense()
        {
            this.DetainID = clsDetainedLicenseData.AddNewDetainedLicense(
                this.LicenseID,
                this.DetainDate,
                this.FineFees,
                this.CreatedByUserID,
                this.IsReleased,
                this.ReleaseDate,
                this.ReleasedByUserID,
                this.ReleaseApplicationID);

            return (this.DetainID != -1);

        }

        private bool _UpdateDetainedLicense()
        {
            return clsDetainedLicenseData.UpdateDetainedLicense(
                this.DetainID,
                this.LicenseID,
                this.DetainDate,
                this.FineFees,
                this.CreatedByUserID,
                this.IsReleased,
                this.ReleaseDate,
                this.ReleasedByUserID,
                this.ReleaseApplicationID);
        }

        public bool Save()
        {

            switch (Mode) 
            {
                case enMode.AddNew:
                    if (_AddNewDetainedLicense()) 
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                  return _UpdateDetainedLicense();
            
            
            }
            return false;

        }

        public static DataTable  GetAllDetainedLicense() 
        { 
            return clsDetainedLicenseData.GetAllDetainedLicenses();
        }

        public static clsDetainedLicense Find(int detainId)
        {
            int licenseID = -1, createdByUserID = -1;
            DateTime detainDate = DateTime.Now;
            decimal fineFees = 0;
            bool isReleased = false;
            DateTime? releaseDate = DateTime.Now;
            int? releasedByUserID = -1, releaseApplicationID = -1;


            if(clsDetainedLicenseData.GetDetinedLicenseInfoById(
                detainId,
               ref licenseID,
               ref detainDate,
               ref fineFees,
               ref createdByUserID,
               ref isReleased,
               ref releaseDate,
               ref releasedByUserID,
               ref releaseApplicationID))
            {
                return new clsDetainedLicense(
                    detainId,
                    licenseID,
                    detainDate,
                    fineFees,
                    createdByUserID,
                    isReleased,
                    releaseDate,
                    releasedByUserID,
                    releaseApplicationID
                    );
            }
            return null;
            
        }
        public static clsDetainedLicense FindByLicenseId(int licenseId)
        {
            int detainId = -1, createdByUserID = -1;
            DateTime detainDate = DateTime.Now;
            decimal fineFees = 0;
            bool isReleased = false;
            DateTime? releaseDate = DateTime.Now;
            int? releasedByUserID = -1, releaseApplicationID = -1;


            if (clsDetainedLicenseData.GetDetinedLicenseInfoByLicenseId(
                 licenseId,
               ref detainId,
               ref detainDate,
               ref fineFees,
               ref createdByUserID,
               ref isReleased,
               ref releaseDate,
               ref releasedByUserID,
               ref releaseApplicationID))
            {
                return new clsDetainedLicense(
                    detainId,
                    licenseId,
                    detainDate,
                    fineFees,
                    createdByUserID,
                    isReleased,
                    releaseDate,
                    releasedByUserID,
                    releaseApplicationID
                    );
            }
            return null;

        }

        public static bool IsDetained(int license)
        {
            return clsDetainedLicenseData.IsDetained(license);
        }
    }
}
