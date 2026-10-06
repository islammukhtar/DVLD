using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsApplicationType
    {
        public enum enApplicationTypes
        {
            NewLocalDrivingLicenseService = 1,
            RenewDrivingLicenseService,
            ReplacementforaLostDrivingLicense,
            ReplacementforaDamagedDrivingLicense,
            ReleaseDetainedDrivingLicsense,
            NewInternationalLicense,
            RetakeTest
        }
        public int ID { get; set; }
        public string Title { get; set; }

        public decimal Fees { get; set; }

        clsApplicationType(int id, string title, decimal fees)
        {

            this.ID = id;
            this.Title = title;
            this.Fees = fees;
        }
        public static DataTable GetAllApplicationTypes()
        {
            return clsApplicationTypeData.GetAllApplicationTypes();
        }

        public static clsApplicationType FindApplicationTypeByID(int id)
        {

            string title = string.Empty;
            decimal fees = 0;

            if (clsApplicationTypeData.GetApplicationTypeInfoByID(id, ref title, ref fees))
            {
                return new clsApplicationType(id, title, fees);
            }
            else
            {
                return null;
            }

        }

        public bool UpdateApplicationType()
        {
            return clsApplicationTypeData.UpdateApplicationType(this.ID, this.Title, this.Fees);
        }


    }
}
