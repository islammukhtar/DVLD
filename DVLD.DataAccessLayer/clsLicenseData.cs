using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace DVLD.DataAccessLayer
{
    public class clsLicenseData
    {

        public static int AddNewLicense(
            int applicationId,
            int driverId,
            int licenseClass,
            DateTime issueDate,
            DateTime expirationDate,
            string notes,
            float paidFees,
            bool isActive,
            byte issueReason,
            int createdByUserId)
        {

            const string query = @"
                        INSERT INTO Licenses
                (
                    ApplicationID,
                    DriverID,
                    LicenseClass,
                    IssueDate,
                    ExpirationDate,
                    Notes,
                    PaidFees,
                    IsActive,
                    IssueReason,
                    CreatedByUserID
                )
                VALUES
                (
                   @ApplicationID,
                   @DriverID,
                   @LicenseClass,
                   @IssueDate,
                   @ExpirationDate,
                   @Notes,
                   @PaidFees,
                   @IsActive,
                   @IssueReason,
                   @CreatedByUserID
                );
                
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;
                command.Parameters.Add("@DriverID", SqlDbType.Int).Value = driverId;
                command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = licenseClass;
                command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = issueDate;
                command.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = expirationDate;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar)
                    .Value = notes != string.Empty ? notes : (object)DBNull.Value;
                command.Parameters.Add("@PaidFees", SqlDbType.SmallMoney).Value = paidFees;
                command.Parameters.Add("@IsActive",SqlDbType.Bit).Value = isActive;
                command.Parameters.Add("@IssueReason", SqlDbType.TinyInt).Value = issueReason;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;

                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;

            }

        }

        public static bool UpdateLicense(
            int LicenseId,
            int applicationId,
            int driverId,
            int licenseClass,
            DateTime issueDate,
            DateTime expirationDate,
            string notes,
            float paidFees,
            bool isActive,
            byte issueReason,
            int createdByUserId)
        {

            const string query = @"UPDATE Licenses
                        SET  ApplicationID = @ApplicationID 
                        	 DriverID = @DriverID
                        	 LicenseClass = @LicenseClass
                        	 IssueDate = @IssueDate
                        	 ExpirationDate = @ExpirationDate
                        	 Notes = @Notes
                        	 PaidFees = @PaidFees
                        	 IssueReason = @IssueReason
                        	 CreatedByUserID = @CreatedByUserID
                             WHERE	LicenseID = @LicenseID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = LicenseId;
                command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;
                command.Parameters.Add("@DriverID", SqlDbType.Int).Value = driverId;
                command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = licenseClass;
                command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = issueDate;
                command.Parameters.Add("@ExpirationDate", SqlDbType.DateTime).Value = expirationDate;
                command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = notes;
                command.Parameters.Add("@PaidFees", SqlDbType.SmallMoney).Value = paidFees;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;
                command.Parameters.Add("@IssueReason", SqlDbType.TinyInt).Value = issueReason;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;

                connection.Open();

                return command.ExecuteNonQuery() == 1;
            }
        }


        public static bool HisLicense(int applicantPersonId,int licenseClassId)
        {
            const string query = @"SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1
            FROM Licenses L
            INNER JOIN Applications A
                ON L.ApplicationID = A.ApplicationID
            WHERE A.ApplicantPersonID = @ApplicantPersonID
              AND L.LicenseClass = @LicenseClass
        )
        THEN 1
        ELSE 0
    END AS HasLicense;";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ApplicantPersonID", SqlDbType.Int).Value = applicantPersonId;
                command.Parameters.Add("@LicenseClass", SqlDbType.Int).Value = licenseClassId;
                

                connection.Open();

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }

        public static bool GetLicenseInfoByApplicationId(
              int applicationId,
          ref int LicenseId,
          ref int driverId,
          ref int licenseClass,
          ref DateTime issueDate,
          ref DateTime expirationDate,
          ref string notes,
          ref float paidFees,
          ref bool isActive,
          ref byte issueReason,
          ref int createdByUserId)
        {

            bool isFound = false;

            const string query = @"
                     SELECT * 
             FROM Licenses
             WHERE ApplicationID = @ApplicationID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;

                            LicenseId = (int)reader["LicenseID"];
                            driverId = (int)reader["DriverID"];
                            licenseClass = (int)reader["LicenseClass"];
                            issueDate = (DateTime)reader["IssueDate"];
                            expirationDate = (DateTime)reader["ExpirationDate"];
                            notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : string.Empty;
                            paidFees = Convert.ToSingle(reader["PaidFees"]);
                            isActive = Convert.ToBoolean(reader["IsActive"]);
                            issueReason = (byte)reader["IssueReason"];
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


        public static bool GetLicenseInfoByLicenseId(

              int licenseId,
          ref int applicationId,
          ref int driverId,
          ref int licenseClass,
          ref DateTime issueDate,
          ref DateTime expirationDate,
          ref string notes,
          ref float paidFees,
          ref bool isActive,
          ref byte issueReason,
          ref int createdByUserId)
        {

            bool isFound = false;

            const string query = @"
                     SELECT * 
             FROM Licenses
             WHERE LicenseID = @LicenseID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = licenseId;

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
                            licenseClass = (int)reader["LicenseClass"];
                            issueDate = (DateTime)reader["IssueDate"];
                            expirationDate = (DateTime)reader["ExpirationDate"];
                            notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : string.Empty;
                            paidFees = Convert.ToSingle(reader["PaidFees"]);
                            isActive = Convert.ToBoolean(reader["IsActive"]);
                            issueReason = (byte)reader["IssueReason"];
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

        private static DataTable ExecuteDataTable(string query, int driverId)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@DriverID", SqlDbType.Int).Value = driverId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }
        public static DataTable GetDriverLicense(int driverId)
        {
            const string query = @"
        SELECT
    LicenseID AS LicID,
    ApplicationID AS AppID,
    LC.ClassName,
    IssueDate,
    ExpirationDate,
    IsActive
FROM Licenses L
INNER JOIN LicenseClasses LC
ON L.LicenseClass = LC.LicenseClassID
WHERE DriverID = @DriverID";

            return ExecuteDataTable(query, driverId);
        }
        public static DataTable GetDriverInternationalLicense(int driverId)
        {
            const string query = @"
        SELECT
            InternationalLicenseID AS IntLicID,
            ApplicationID,
            IssuedUsingLocalLicenseID AS LLicenseID,
            IssueDate,
            ExpirationDate,
            IsActive
        FROM InternationalLicenses
        WHERE DriverID = @DriverID";

            return ExecuteDataTable(query, driverId);
        }
        public static bool LockLicense(int licenseId)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr))
            {
                string query = @"
            UPDATE Licenses
            SET IsActive = 0
            WHERE LicenseID = @LicenseID;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", licenseId);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return rowsAffected > 0;
        }
    }
}
