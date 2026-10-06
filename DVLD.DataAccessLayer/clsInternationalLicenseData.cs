using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.DataAccessLayer
{
    public class clsInternationalLicenseData
    {

        public static bool GetInternationalLicenseById(
              int internationalLicenseId,
          ref int applicationId,
          ref int driverId,
          ref int IssuedUsingLocalLicenseId,
          ref DateTime issueDate,
          ref DateTime expirationDate,
          ref bool isActive,
          ref int createdByUserId)
        {

            bool isFound = false;

            const string query = @"
                     SELECT *
FROM InternationalLicenses
WHERE InternationalLicenseID = @InternationalLicenseID
";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@InternationalLicenseID", SqlDbType.Int).Value = internationalLicenseId;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            applicationId = (int)reader["ApplicationID"];
                            driverId = (int)reader["DriverID"];
                            IssuedUsingLocalLicenseId = (int)reader["IssuedUsingLocalLicenseID"];
                            issueDate = (DateTime)reader["IssueDate"];
                            expirationDate = (DateTime)reader["ExpirationDate"];
                            isActive = Convert.ToBoolean(reader["IsActive"]);
                            createdByUserId = (int)reader["CreatedByUserID"];
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

        public static bool GetInternationalLicenseByLocalLicenseId(
              int issuedUsingLocalLicenseId,
          ref int internationalLicenseId,
          ref int applicationId,
          ref int driverId,
          ref DateTime issueDate,
          ref DateTime expirationDate,
          ref bool isActive,
          ref int createdByUserId)
        {

            bool isFound = false;

            const string query = @"
                     SELECT *
           FROM InternationalLicenses
           WHERE IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID
           ";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@IssuedUsingLocalLicenseID", SqlDbType.Int).Value
                    = issuedUsingLocalLicenseId;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            internationalLicenseId = (int)reader["InternationalLicenseID"];
                            applicationId = (int)reader["ApplicationID"];
                            driverId = (int)reader["DriverID"];
                            issueDate = (DateTime)reader["IssueDate"];
                            expirationDate = (DateTime)reader["ExpirationDate"];
                            isActive = Convert.ToBoolean(reader["IsActive"]);
                            createdByUserId = (int)reader["CreatedByUserID"];
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

        public static int AddNewInternationalLicense(
           int applicationId,
           int driverId,
           int IssuedUsingLocalLicense,
           DateTime issueDate,
           DateTime expirationDate,
           bool isActive,
           int createdByUserId)
        {


            const string query = @"
                        INSERT INTO InternationalLicenses
                (
                    ApplicationID,
                    DriverID,
                    IssuedUsingLocalLicenseID,
                    IssueDate,
                    ExpirationDate,
                    IsActive,
                    CreatedByUserID
                )
                VALUES
                (
                   @ApplicationID,
                   @DriverID,
                   @IssuedUsingLocalLicenseID,
                   @IssueDate,
                   @ExpirationDate,
                   @IsActive,
                   @CreatedByUserID
                );
                
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;
                command.Parameters.Add("@DriverID", SqlDbType.Int).Value = driverId;
                command.Parameters.Add("@IssuedUsingLocalLicenseID", SqlDbType.Int).Value = IssuedUsingLocalLicense;
                command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = issueDate;
                command.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = expirationDate;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;

                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;

            }

        }

        public static bool UpdateInternationalLicense(
           int internationalLicenseId,
           int applicationId,
           int driverId,
           int IssuedUsingLocalLicenseId,
           DateTime issueDate,
           DateTime expirationDate,
           bool isActive,
           int createdByUserId)
        {

            const string query = @"UPDATE InternationalLicenses
                        SET  ApplicationID = @ApplicationID 
                        	 DriverID = @DriverID
                        	 IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID
                        	 IssueDate = @IssueDate
                        	 ExpirationDate = @ExpirationDate
                             IsActive = @IsActive
                        	 CreatedByUserID = @CreatedByUserID
                             WHERE	 InternationalLicenseID = @InternationalLicenseID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@InternationalLicenseID", SqlDbType.Int).Value = internationalLicenseId;
                command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;
                command.Parameters.Add("@DriverID", SqlDbType.Int).Value = driverId;
                command.Parameters.Add("@IssuedUsingLocalLicenseID", SqlDbType.Int).Value = IssuedUsingLocalLicenseId;
                command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = issueDate;
                command.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = expirationDate;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;
               
                connection.Open();
                return command.ExecuteNonQuery() == 1;
            }
        }
        public static bool UpdateLocalLicenseId(
          int internationalLicenseId,
          int newLocalLicenseId)
        {

            const string query = @"UPDATE InternationalLicenses
                        SET  IssuedUsingLocalLicenseID = @IssuedUsingLocalLicenseID
                             WHERE	 InternationalLicenseID = @InternationalLicenseID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@InternationalLicenseID", SqlDbType.Int).Value = internationalLicenseId;
                command.Parameters.Add("@IssuedUsingLocalLicenseID", SqlDbType.Int).Value = newLocalLicenseId;

                connection.Open();
                return command.ExecuteNonQuery() == 1;
            }
        }

        public static bool HasInternationalLicense(int licenseId)
        {

            const string query = @"SELECT
    CASE
        WHEN EXISTS (
            SELECT 1
            FROM InternationalLicenses
            WHERE IssuedUsingLocalLicenseID = @LicenseID
        )
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS HasInternationalLicense;";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = licenseId;
               
                connection.Open();

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }
    }
}
