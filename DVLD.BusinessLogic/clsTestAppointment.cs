using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsTestAppointment
    {

        enum enMode { AddNew, Update}

        enMode _mode;

        public int TestAppointmentID {  get; set; } 
        public clsTestType.enTestTypes TestTypeID { get; set; }
        public int LocalDrivingLicenseApplicationID {  get; set; }  
        public clsLocalDrivingLicenseApplication LocalDrivingLicenseApplicationInfo { get; set; }
        public DateTime AppointmentDate { get; set; }
        public float PaidFees {  get; set; }    
        public int CreatedByUserID {  get; set; }
        public bool IsLocked {  get; set; }
        public int? RetakeTestApplicationID {  get; set; }

        private clsTestAppointment(
            int testAppointmentID, 
            clsTestType.enTestTypes testTypeID,
            int localDrivingLicenseApplicationID,
            DateTime appointmentDate,
            float paidFees, 
            int createdByUserID,
            bool isLocked, 
            int? retakeTestApplicationID
            )
        {
           
            TestAppointmentID = testAppointmentID;
            TestTypeID = testTypeID;
            LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            LocalDrivingLicenseApplicationInfo = clsLocalDrivingLicenseApplication.Find(localDrivingLicenseApplicationID);
            AppointmentDate = appointmentDate;
            PaidFees = paidFees;
            CreatedByUserID = createdByUserID;
            IsLocked = isLocked;
            RetakeTestApplicationID = retakeTestApplicationID;
            _mode = enMode.Update;
        }

        public clsTestAppointment()
        {
            this.TestAppointmentID = -1;
            this.TestTypeID = clsTestType.enTestTypes.VisionTest;
            this.LocalDrivingLicenseApplicationID = -1;
            this.AppointmentDate = DateTime.Now;
            this.PaidFees = 0;
            this.CreatedByUserID = -1;
            this.IsLocked = false;
            this.RetakeTestApplicationID = default;
            _mode = enMode.AddNew;

        }
        public static DataTable GetApplicationTestAppointmentPerTestType(int localDrivingLicenseApplicationId,int testTypeId)
        {
            return clsTestAppointmentData.GetApplicationTestAppointmentPerTestType(localDrivingLicenseApplicationId, testTypeId);
        }

        public static clsTestAppointment Find(int testAppointmentId)
        {
         
            int localDrivingLicenseApplicationId = -1, createdByUserId = -1, retakeTestApplicationId = -1, testTypeId=-1;
            DateTime appointmentDate = DateTime.Now;
            float paidFees = 0;
            bool isLocked = false;

            if(clsTestAppointmentData.GetTestAppointmentById(
                testAppointmentId,
                ref testTypeId,
                ref localDrivingLicenseApplicationId,
                ref appointmentDate,
                ref paidFees,
                ref createdByUserId,
                ref isLocked,
                ref retakeTestApplicationId
                ))
            {
                return new clsTestAppointment(
                    testAppointmentId,
                    (clsTestType.enTestTypes)testTypeId,
                    localDrivingLicenseApplicationId,
                    appointmentDate,
                    paidFees,
                    createdByUserId,
                    isLocked,
                    retakeTestApplicationId);

            }
            return null;
        }

        private bool _AddNewAppointment()
        {

            this.TestAppointmentID = clsTestAppointmentData.AddNewTestAppointment(
              (int)this.TestTypeID,
              this.LocalDrivingLicenseApplicationID,
              this.AppointmentDate,
              this.PaidFees,
              this.CreatedByUserID,
              this.IsLocked,
              this.RetakeTestApplicationID
               );

            return (this.TestAppointmentID != -1);
        }
        private bool _UpdateAppointment()
        {
            return clsTestAppointmentData.UpdateTestAppointment(this.TestAppointmentID, this.AppointmentDate);
        }
        public bool Save()
        {
            switch (_mode)
            {

                case enMode.AddNew:
                    if (_AddNewAppointment())
                    {

                        _mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateAppointment();


            }

            return false;
        }

        public static bool IsThereAnActiveAppointment(
            int localDrivingLicenseApplicationId,
            int testTypeId)
        {
          return clsTestAppointmentData.IsThereAnActiveAppointment(localDrivingLicenseApplicationId, testTypeId);
        }


        public static int TotalTrials(int localDrivingLicenseApplicationId,int testTypeId)
        {
            return clsTestAppointmentData.GetTotalTrials(localDrivingLicenseApplicationId, testTypeId);   
        }
        
        public static bool ApplicantPassedTest(int localDrivingLicenseApplicationId, int testTypeId)
        {
            return clsTestAppointmentData.DidPassedTest(localDrivingLicenseApplicationId, testTypeId);
        }

        public static bool LockTestAppointment(int testAppointmentId)
        {
            return clsTestAppointmentData.LockTestAppointment(testAppointmentId);
        }

        public bool LockTestAppointment()
        {
            return clsTestAppointmentData.LockTestAppointment(this.TestAppointmentID);
        }

        public static bool ApplicantFailedTest(int localDrivingLicenseApplicationId,int testTypeId)
        {
            return clsTestAppointmentData.DidFailedTest(localDrivingLicenseApplicationId, testTypeId);
        }
    }
}
