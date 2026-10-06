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
    public partial class frmTakeTest : Form
    {
        int _testAppointmentId;

        clsTestAppointment _testAppointment;

        public frmTakeTest(int testAppointmentId)
        {
            InitializeComponent();
            _testAppointmentId = testAppointmentId;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmTakeTest_Load(object sender, EventArgs e)
        {
            ctrlScheduledTest1.TestAppointmentLoaded += CtrlScheduledTest1_TestAppointmentLoaded;
            ctrlScheduledTest1.LoadTestInfo(_testAppointmentId);
        }

        private void CtrlScheduledTest1_TestAppointmentLoaded(object sender, EventArgs e)
        {

            clsTestAppointment testAppointment = (clsTestAppointment)sender;

            if (testAppointment.IsLocked)
            {
                btnSave.Enabled = false;
                MessageBox.Show(
                  "This test appointment is locked. The applicant cannot take this test.",
                  "Test Appointment Locked",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Warning
                  );



            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            _testAppointment = clsTestAppointment.Find(_testAppointmentId);

            if (_testAppointment == null)
            {
                MessageBox.Show(
                    "Test appointment was not found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                return;
            }

            if (_testAppointment.AppointmentDate != DateTime.Today)
            {
                MessageBox.Show(
                      "You cannot take this test because the appointment is not scheduled for today.",
                      "Test Cannot Be Taken",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning
                  );
                return;
            }

            var test = new clsTest();
            test.TestAppointmentId = _testAppointment.TestAppointmentID;

            clsTest.enTestResult testResult =
                (rbPass.Checked) ? clsTest.enTestResult.Pass : clsTest.enTestResult.Fail;

            test.TestResult = (byte)testResult;
            test.Notes = txtNotes.Text.Trim();
            test.CreatedByUserId = clsGlobal.CurrentUser.UserID;

            if (MessageBox.Show(
               "Are you sure you want to save?\n\n" +
               "After saving, you cannot change the pass/fail result.",
               "Confirm Save",
               MessageBoxButtons.OKCancel,
               MessageBoxIcon.Warning,
               MessageBoxDefaultButton.Button2
               ) == DialogResult.OK)
              {
                  if (!test.Save())
                  {
                      MessageBox.Show(
                      "The test result was not saved.",
                      "Save Failed",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error
                      );
   
                     return;
                  }
    
                  if (!_testAppointment.LockTestAppointment())
                  {
                      MessageBox.Show(
                          "The test result was saved, but the appointment could not be locked.",
                          "Appointment Lock Failed",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error
                      );
    
                      return;
                  }
    
                  MessageBox.Show(
                      "The test result has been saved successfully, and the appointment is now locked.",
                      "Saved Successfully",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Information
                  );

              
    
            }

        }
    }
}
