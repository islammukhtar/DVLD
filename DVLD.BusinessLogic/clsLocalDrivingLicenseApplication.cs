using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsLocalDrivingLicenseApplication :clsApplication
    {

        enum enMode { AddNew ,Update}
        enMode Mode;
        public int LocalDrivingLicenseApplicationID { get; set; }
        public int LicenseClassID { get; set; }

        public clsLicenseClass LicenseClassInfo { get; }   
        public clsLocalDrivingLicenseApplication()
        {
            LocalDrivingLicenseApplicationID = -1;
            LicenseClassID = -1;
            Mode = enMode.AddNew;
        }

        private clsLocalDrivingLicenseApplication(
        int localDrivingLicenseApplicationId,
        int licenseClassId,
        int applicationId,
        int applicatPersonId,
        DateTime applicationDate,
        int applicatonTypeId,
        int applicationStatus,
        DateTime lastStatusDate,
        decimal paidFess,
        int createdByUserId)

          : base(
              applicationId,
              applicatPersonId,
              applicationDate,
         applicatonTypeId,
         applicationStatus,
         lastStatusDate,
         paidFess,
         createdByUserId)
        {
            LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationId;
            LicenseClassID = licenseClassId;
            LicenseClassInfo = clsLicenseClass.Find(licenseClassId);
            Mode = enMode.Update;
        }

        private bool AddNewLocalDrivingLicenseApplication()
        {
            this.LocalDrivingLicenseApplicationID
                = clsLocalDrivingLicenseApplicationData.AddNewLocalDrivingLicenseApplication(
                    this.ApplicationID,
                    this.LicenseClassID);

            return (this.LocalDrivingLicenseApplicationID != -1);
        }
        private bool UpdateLocalDrivingLicenseApplication()
        {
            return clsLocalDrivingLicenseApplicationData.UpdateLocalDrivingLicenseApplication(
                this.LocalDrivingLicenseApplicationID, this.ApplicationID, this.LicenseClassID
                );
        }

        public bool Save()
        {
            base.Mode = (clsApplication.enMode)Mode;
            if (!base.Save())
                return false;


            //After we save the main application now we save the sub application.
            switch (Mode)
            {
                case enMode.AddNew:
                    if (AddNewLocalDrivingLicenseApplication())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return UpdateLocalDrivingLicenseApplication();

            }

            return false;

        }

        public static DataTable GetAllLocalDrivingLicensesApplications()
        {
            return clsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicensesApplications();
        }

        public static clsLocalDrivingLicenseApplication Find(int localDrivingLicenseApplicationId)
        {
            int applicationId = -1, licenseClassId = -1;

            bool isFound = clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationById(
                localDrivingLicenseApplicationId, ref applicationId, ref licenseClassId);


            if (isFound) {

                clsApplication application = clsApplication.FindBaseApplication(applicationId);

                return new clsLocalDrivingLicenseApplication(localDrivingLicenseApplicationId, licenseClassId,
                    application.ApplicationID, application.ApplicantPersonID, application.ApplicationDate,
                    application.ApplicationTypeID, application.ApplicationStatus, application.LastStatusDate,
                    application.PaidFess, application.CreatedByUserID);
            
            }
            return null;
        }

        public static bool Delete(int localDrivingLicenseApplicationId)
        {

            bool isLocalDrivingLicenseApplicationDeleted = false, isBaseApplicationDeleted = false;

            var application = clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationId);

            if (application == null)
                return false;
           
            if (!clsLocalDrivingLicenseApplicationData.DeleteLocalDrivingLicenseApplication(application.LocalDrivingLicenseApplicationID))
                return false;
            isLocalDrivingLicenseApplicationDeleted = true;

            if(!clsApplication.DeleteApplication(application.ApplicationID))
                return false;
            isBaseApplicationDeleted = true;

            return (isLocalDrivingLicenseApplicationDeleted && isBaseApplicationDeleted);
        }

        public static bool Cancel(int localDrivingLicenseApplicationId) {

            var application = clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationId);
            
            if(application == null) return false;

            return clsApplication.CancelApplication(application.ApplicationID);
        
        }

        public int GetPassedTestsCount()
        {
            return clsTestsData.GetPassedTestsCount(this.LocalDrivingLicenseApplicationID);
        }


    }
}
