using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.BusinessLogic;

namespace DVLD.Presentation.Tests
{
    public partial class frmScheduleTest : Form
    {
        enum enMode
        {
            AddNew,
            Update
        }

        enMode _mode;
        int _localDrivingLicenseApplicationId;
        int _testAppointmentId;
        clsTestType.enTestTypes _testType;
        bool _isRetakeTest;

        public frmScheduleTest(
            int localDrivingLicenseApplicationId,
            clsTestType.enTestTypes testType,
            bool isRetakeTest = false)
        {
            InitializeComponent();

            _mode = enMode.AddNew;
            _localDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
            _testType = testType;
            _isRetakeTest = isRetakeTest;

        }
        public frmScheduleTest(int testAppointmentId)
        {
            InitializeComponent();
            _mode = enMode.Update;
            _testAppointmentId = testAppointmentId;
        }
        private void frmScheduleTest_Load(object sender, EventArgs e)
        {
            switch (_mode)
            {
                case enMode.AddNew:
                    ctrlScheduleTest1.LoadForAddNew(
                        _localDrivingLicenseApplicationId, 
                        _testType,
                        _isRetakeTest);

                    break;
                case enMode.Update:
                    ctrlScheduleTest1.LoadForUpdate(_testAppointmentId);
                    break;
            }
           
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
