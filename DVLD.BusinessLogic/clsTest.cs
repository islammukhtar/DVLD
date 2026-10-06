using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DVLD.DataAccessLayer;

namespace DVLD.BusinessLogic
{
    public class clsTest
    {

        enum enMode
        {
            AddNew,
            Update
        }

        public enum enTestResult
        {
            Fail,
            Pass
        }

        enMode Mode;
        public int TestId { get; set; }
        public int TestAppointmentId { get; set; }

        public clsTestAppointment TestAppointmentInfo { get; set; }
        public byte TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserId { get; set; }

        private clsTest(
            int testId, 
            int testAppointmentId, 
            byte testResult,
            string notes,
            int createdByUserId)
        {
         
            TestId = testId;
            TestAppointmentId = testAppointmentId;
            TestAppointmentInfo = clsTestAppointment.Find(testAppointmentId);
            TestResult = testResult;
            Notes = notes;
            CreatedByUserId = createdByUserId;
            Mode = enMode.Update;
        }


        public clsTest()
        {

            TestId = -1;
            TestAppointmentId = -1;
            TestResult = default;
            Notes = string.Empty;
            CreatedByUserId = -1;
            Mode = enMode.AddNew;
        }

        bool _AddNewTest()
        {
            this.TestId = clsTestsData.AddNewTest(
                this.TestAppointmentId,
                this.TestResult,
                this.Notes,
                this.CreatedByUserId);

            return (this.TestId != -1);
        }

        bool _UpdateTest()
        {
            return clsTestsData.UpdateTest(this.TestId,this.TestAppointmentId,this.TestResult, this.Notes, this.CreatedByUserId);  
        }

        public bool Save()
        {
            switch (Mode) {

                case enMode.AddNew:
                    if (_AddNewTest()) {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateTest();
            
            
            }
            return false;

        }

        public static bool PassedAllTests(int localDrvingLicenseApplicationId)
        {
            return clsTestsData.PassedAllTests(localDrvingLicenseApplicationId);
        }

    }

}
