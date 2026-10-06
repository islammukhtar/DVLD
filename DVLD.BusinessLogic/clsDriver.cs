using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public  class clsDriver
    {
        enum enMode
        {
            AddNew,
            Update
        }
        enMode Mode;
        public int DriverID {  get; set; }
        public int PersonID { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public clsDriver()
        {
            DriverID = -1;
            PersonID = -1;
            CreatedByUserID = -1;
            CreatedDate = DateTime.Now;
            Mode = enMode.AddNew;
        }

        private clsDriver(
            int driverID, 
            int personID,
            int createdByUserID,
            DateTime createdDate)
        {
            DriverID = driverID;
            PersonID = personID;
            CreatedByUserID = createdByUserID;
            CreatedDate = createdDate;
            Mode = enMode.Update;
        }


        public static DataTable GetAllDrivers()
        {
            return clsDriverData.GetAllDrivers();
        }
        private bool _AddNewDriver()
        {

            this.DriverID = clsDriverData.AddNewDriver(
                this.PersonID,
                this.CreatedByUserID,
                this.CreatedDate);

            return (this.DriverID != -1);
        }
        private bool _UpdateDriver()
        {
            return clsDriverData.UpdateDriver(
                this.DriverID,
                this.PersonID,
                this.CreatedByUserID,
                this.CreatedDate);
        }
        public bool Save()
        {
            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNewDriver())
                    {

                        Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateDriver();


            }

            return false;
        }
        public static clsDriver FindByPersonID(int personId)
        {
            int driverId = -1, createdByUserID = -1;
            DateTime createdDate = DateTime.Now;

            if(clsDriverData.GetDriverInfoByPersonId(personId,ref driverId,ref createdByUserID,ref createdDate))
            {
                return new clsDriver(driverId,personId,createdByUserID,createdDate);   
            }

            return null;
        }
        public static clsDriver FindByDriverId(int driverId)
        {
            int personId = -1, createdByUserID = -1;
            DateTime createdDate = DateTime.Now;

            if (clsDriverData.GetDriverInfoById(driverId,ref personId, ref createdByUserID, ref createdDate))
            {
                return new clsDriver(driverId, personId, createdByUserID, createdDate);
            }

            return null;
        }

        public static bool IsPersonADriver(int personId)
        {
            return clsDriverData.IsPersonADriver(personId);
        }

        public static DataTable GetDriverLicense(int driverId)
        {
            return clsLicenseData.GetDriverLicense(driverId);
        }

        public static DataTable GetDriverInternationalLicense(int driverId)
        {
            return clsLicenseData.GetDriverInternationalLicense(driverId);
        }
    }
}
