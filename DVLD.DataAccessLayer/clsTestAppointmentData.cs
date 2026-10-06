using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsTestAppointmentData
    {


        public static bool GetTestAppointmentById(
          int testAppointmentId,
          ref int testTypeId,
          ref int localDrivingLicenseApplicationId,
          ref DateTime appointmentDate,
          ref float paidFees,
          ref int createdByUserId,
          ref bool isLocked,
          ref int retakeTestApplicationId)
        {
            bool isFound = false;

            const string query = @"
        SELECT *
        FROM TestAppointments
        WHERE TestAppointmentID = @TestAppointmentID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TestAppointmentID", testAppointmentId);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            testTypeId = (int)reader["TestTypeID"];
                            localDrivingLicenseApplicationId = (int)reader["LocalDrivingLicenseApplicationID"];
                            appointmentDate = (DateTime)reader["AppointmentDate"];
                            paidFees = Convert.ToSingle(reader["PaidFees"]);
                            createdByUserId = (int)reader["CreatedByUserID"];
                            isLocked = (bool)reader["IsLocked"];

                            retakeTestApplicationId =
                                reader["RetakeTestApplicationID"] == DBNull.Value
                                ? -1
                                : (int)reader["RetakeTestApplicationID"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log Exception if needed
                    isFound = false;
                }
            }

            return isFound;
        }


        public static DataTable GetApplicationTestAppointmentPerTestType(
           int localDrivingLicenseApplicationId,
           int testTypeId)
        {

            DataTable dtAppointment = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT T.TestAppointmentID as AppointmentID,T.AppointmentDate,T.PaidFees,T.IsLocked
                             FROM TestAppointments T
                             WHERE T.LocalDrivingLicenseApplicationID=@LocalDrivingLicenseApplicationID	AND T.TestTypeID=@TestTypeID
                             ORDER BY T.TestAppointmentID DESC";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplicationId);
            cmd.Parameters.AddWithValue("@TestTypeID", testTypeId);
            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    dtAppointment.Load(reader);
                }
                reader.Close();

            }
            finally
            {
                connection.Close();
            }
            return dtAppointment;
        }


        public static int AddNewTestAppointment(
           int testTypeId,
           int localDrivingLicenseApplicationId,
           DateTime appointmentDate,
           float paidFees,
           int createdByUserId,
           bool isLocked,
           int? retakeTestApplicationId)
        {
            const string query = @"
        INSERT INTO TestAppointments
        (
            TestTypeID,
            LocalDrivingLicenseApplicationID,
            AppointmentDate,
            PaidFees,
            CreatedByUserID,
            IsLocked,
            RetakeTestApplicationID
        )
        VALUES
        (
            @TestTypeID,
            @LocalDrivingLicenseApplicationID,
            @AppointmentDate,
            @PaidFees,
            @CreatedByUserID,
            @IsLocked,
            @RetakeTestApplicationID
        );

        SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = testTypeId;
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = localDrivingLicenseApplicationId;
                command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = appointmentDate;
                command.Parameters.Add("@PaidFees", SqlDbType.Decimal).Value = paidFees;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;
                command.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = isLocked;
                command.Parameters.Add("@RetakeTestApplicationID", SqlDbType.Int)
                       .Value = retakeTestApplicationId ?? (object)DBNull.Value;

                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;
            }

        }

        public static bool UpdateTestAppointment(
            int testAppointmentId,
            DateTime newAppointmentDate)
        {

            const string query = @"UPDATE TestAppointments
                                  SET AppointmentDate =@AppointmentDate
                                  WHERE TestAppointmentID=@TestAppointmentID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = newAppointmentDate;
                command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = testAppointmentId;

                connection.Open();

                return command.ExecuteNonQuery() == 1;
            }
        }
        public static bool IsThereAnActiveAppointment(int localDrivingLicenseApplicationId, int testTypeId)
        {
            const string query = @"
        SELECT CASE
                 WHEN EXISTS
                 (
                     SELECT 1
                     FROM TestAppointments
                     WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                           AND TestTypeID = @TestTypeID
                           AND IsLocked = 0
                 )
                 THEN 1
                 ELSE 0
               END;";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int)
                       .Value = localDrivingLicenseApplicationId;

                command.Parameters.Add("@TestTypeID", SqlDbType.Int)
                      .Value = testTypeId;

                connection.Open();

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }

        public static int GetTotalTrials(int localDrivingLicenseApplicationId, int testTypeId)
        {


            const string query = @"SELECT TotalTrials= COUNT(*)  FROM TestAppointments INNER JOIN Tests
                 ON TestAppointments.TestAppointmentID=Tests.TestAppointmentID
                 WHERE TestAppointments.LocalDrivingLicenseApplicationID=@LocalDrivingLicenseApplicationID AND TestAppointments.TestTypeID=@TestTypeID";


            using (SqlConnection connection =
                      new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = testTypeId;
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int).Value = localDrivingLicenseApplicationId;

                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;
            }


        }

        public static bool DidPassedTest(int localDrivingLicenseApplicationId, int testTypeId)
        {

            const string query = @"
                       SELECT 
                       CASE WHEN EXISTS
                       (
                       SELECT 1 FROM TestAppointments INNER JOIN Tests
                       ON TestAppointments.TestAppointmentID=Tests.TestAppointmentID
                       WHERE TestAppointments.LocalDrivingLicenseApplicationID=@LocalDrivingLicenseApplicationID
                       AND TestAppointments.TestTypeID=@TestTypeID
                       AND Tests.TestResult=1
                       )
                       THEN 1
                       ELSE 0
                       END";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int)
                       .Value = localDrivingLicenseApplicationId;

                command.Parameters.Add("@TestTypeID", SqlDbType.Int)
                      .Value = testTypeId;

                connection.Open();

                return Convert.ToBoolean(command.ExecuteScalar());
            }

        }

        public static bool DidFailedTest(int localDrivingLicenseApplicationId,int testTypeId)
        {
            const string query = @"
                       SELECT 
                       CASE WHEN EXISTS
                       (
                       SELECT TOP 1 1
                         FROM Tests T
                         INNER JOIN TestAppointments TA
                             ON T.TestAppointmentID = TA.TestAppointmentID
                         WHERE TA.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                           AND TA.TestTypeID = @TestTypeID
                           AND T.TestResult = 0
                       )
                       THEN 1
                       ELSE 0
                       END";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LocalDrivingLicenseApplicationID", SqlDbType.Int)
                       .Value = localDrivingLicenseApplicationId;

                command.Parameters.Add("@TestTypeID", SqlDbType.Int)
                      .Value = testTypeId;

                connection.Open();

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }
        public static bool LockTestAppointment(int testAppointmentId)
        {
            const string query = @"UPDATE TestAppointments
                                  SET IsLocked = 1
                                  WHERE TestAppointmentID=@TestAppointmentID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {

                command.Parameters.Add("@TestAppointmentID", SqlDbType.Int).Value = testAppointmentId;

                connection.Open();

                return command.ExecuteNonQuery() == 1;
            }
        }
    }
}
