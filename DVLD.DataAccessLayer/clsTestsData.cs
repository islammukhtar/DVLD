using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.SqlServer.Server;

namespace DVLD.DataAccessLayer
{
    public class clsTestsData
    {

        public static int AddNewTest(
            int testAppointmentId,
            byte testResult,
            string notes,
            int createdByUserId)
        {

            const string query = @"
                        INSERT INTO Tests
                (
                    TestAppointmentID,
                    TestResult,
                    Notes,
                    CreatedByUserID
                )
                VALUES
                (
                   @TestAppointmentID,
                   @TestResult,
                   @Notes,
                   @CreatedByUserID
                );
                
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = testAppointmentId;
                command.Parameters.Add("@TestResult", SqlDbType.Bit).Value = testResult;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar)
                    .Value = notes == string.Empty ? (object)DBNull.Value : notes;
                command.Parameters.Add("@CreatedByUserID",SqlDbType.Int).Value = createdByUserId;

                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;

            }

        }

        public static bool UpdateTest(
            int testId,
            int testAppointmentId,
            byte testResult,
            string notes,
            int createdByUserId)
        {
            const string query = @"UPDATE Tests
                          SET TestAppointmentID=@TestAppointmentID,
                          TestResult=@TestResult,
                          Notes=@Notes,
                          CreatedByUserID=@CreatedByUserID,
                          WHERE TestID=@TestID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@TestID", SqlDbType.Int).Value = testId;
                command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = testAppointmentId;
                command.Parameters.Add("@TestResult", SqlDbType.Bit).Value = testResult;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = notes;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;

                connection.Open();

                return command.ExecuteNonQuery() == 1;
            }
        }


        public static int GetPassedTestsCount(int localDrivingLicenseApplicationID)
        {
            const string query = @"
        SELECT COUNT(*)
        FROM Tests T
        INNER JOIN TestAppointments TA
            ON T.TestAppointmentID = TA.TestAppointmentID
        WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
          AND T.TestResult = 1;";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value =
                    localDrivingLicenseApplicationID;

                try
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Log the exception if needed.
                    throw;
                }
            }
        }

        public static bool PassedAllTests(int localDrivingLicenseApplicationID) {

            const string query = @"
                           SELECT CASE
                       WHEN NOT EXISTS
                       (
                           SELECT 1
                           FROM TestTypes TT
                           WHERE NOT EXISTS
                           (
                               SELECT 1
                               FROM Tests T
                               INNER JOIN TestAppointments TA
                                   ON T.TestAppointmentID = TA.TestAppointmentID
                               WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                                 AND TA.TestTypeID = TT.TestTypeID
                                 AND T.TestResult = 1
                           )
                       )
                       THEN 1
                       ELSE 0
                   END AS PassedAllTests;";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value =
                    localDrivingLicenseApplicationID;

                try
                {
                    connection.Open();
                    return Convert.ToBoolean(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Log the exception if needed.
                    throw;
                }
            }


        }
    }
}
