using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Collections;

namespace DVLD.DataAccessLayer
{
    public class clsDetainedLicenseData
    {
        public static int AddNewDetainedLicense(
               int licenseId,
               DateTime detainDate,
               decimal fineFees,
               int createdByUserId,
               bool isReleased,
               DateTime? releaseDate,
               int? releasedByUserId,
               int? releaseApplicationId)
        {

            const string query = @"
                        INSERT INTO DetainedLicenses
                (
                   LicenseID,
                   DetainDate,
                   FineFees,
                   CreatedByUserID,
                   IsReleased,
                   ReleaseDate,
                   ReleasedByUserID,
                   ReleaseApplicationID
                )
                VALUES
                (
                  @LicenseID,
                  @DetainDate,
                  @FineFees,
                  @CreatedByUserID,
                  @IsReleased,
                  @ReleaseDate,
                  @ReleasedByUserID,
                  @ReleaseApplicationID
                );
                
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = licenseId;
                command.Parameters.Add("@DetainDate", SqlDbType.DateTime).Value = detainDate;
                command.Parameters.Add("@FineFees", SqlDbType.SmallMoney).Value = fineFees;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;
                command.Parameters.Add("@IsReleased", SqlDbType.Bit).Value = isReleased;

                if (releaseDate != null)
                {
                    command.Parameters.Add("@ReleaseDate", SqlDbType.DateTime).Value = releaseDate;
                }
                else
                {
                    command.Parameters.Add("@ReleaseDate", SqlDbType.DateTime).Value = DBNull.Value;
                }

                if (releasedByUserId != null)
                {
                    command.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = releasedByUserId;
                }
                else
                {
                    command.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = DBNull.Value;
                }

                if (releaseApplicationId != null)
                {
                    command.Parameters.Add("@ReleaseApplicationID", SqlDbType.Int).Value = releaseApplicationId;
                }
                else
                {
                    command.Parameters.Add("@ReleaseApplicationID", SqlDbType.Int).Value = DBNull.Value;
                }


                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;

            }

        }

        public static bool UpdateDetainedLicense(
            int detainId,
            int licenseId,
            DateTime detainDate,
            decimal fineFees,
            int createdByUserId,
            bool isReleased,
            DateTime? releaseDate,
            int? releasedByUserId,
            int? releaseApplicationId)
        {

            const string query = @"UPDATE DetainedLicenses
                        SET  LicenseID = @LicenseID,
                        	 DetainDate = @DetainDate,
                        	 FineFees = @FineFees,
                        	 CreatedByUserID = @CreatedByUserID,
                        	 IsReleased = @IsReleased,
                             ReleaseDate = @ReleaseDate,
                        	 ReleasedByUserID = @ReleasedByUserID,
                        	 ReleaseApplicationID = @ReleaseApplicationID
                             WHERE	DetainID = @DetainID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@DetainID", SqlDbType.Int).Value = detainId;
                command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = licenseId;
                command.Parameters.Add("@DetainDate", SqlDbType.DateTime).Value = detainDate;
                command.Parameters.Add("@FineFees", SqlDbType.SmallMoney).Value = fineFees;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;
                command.Parameters.Add("@IsReleased", SqlDbType.Bit).Value = isReleased;
               
                if (releaseDate != null)
                {
                    command.Parameters.Add("@ReleaseDate", SqlDbType.DateTime).Value = releaseDate;
                }
                else
                {
                    command.Parameters.Add("@ReleaseDate", SqlDbType.DateTime).Value = DBNull.Value;
                }

                if (releasedByUserId != null)
                {
                    command.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = releasedByUserId;
                }
                else
                {
                    command.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = DBNull.Value;
                }

                if (releaseApplicationId != null)
                {
                    command.Parameters.Add("@ReleaseApplicationID", SqlDbType.Int).Value = releaseApplicationId;
                }
                else
                {
                    command.Parameters.Add("@ReleaseApplicationID", SqlDbType.Int).Value = DBNull.Value;
                }
               
              

                connection.Open();

                return command.ExecuteNonQuery() == 1;
            }
        }

        public static bool GetDetinedLicenseInfoById(
            int detainId,
            ref int licenseId,
            ref DateTime detainDate,
            ref decimal fineFees,
            ref int createdByUserId,
            ref bool isReleased,
            ref DateTime? releaseDate,
            ref int? releasedByUserId,
            ref int? releaseApplicationId)
        {


            const string query = @"
                              SELECT * 
             FROM DetainedLicenses
             WHERE DetainID = @DetainID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@DetainID", SqlDbType.Int).Value = detainId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (!reader.Read())
                        return false;

                    licenseId = reader.GetInt32(reader.GetOrdinal("LicenseID"));
                    detainDate = reader.GetDateTime(reader.GetOrdinal("DetainDate"));
                    fineFees = reader.GetDecimal(reader.GetOrdinal("FineFees"));
                    createdByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserID"));
                    isReleased = reader.GetBoolean(reader.GetOrdinal("IsReleased"));

                    int releaseDateIndex = reader.GetOrdinal("ReleaseDate");

                    releaseDate = reader.IsDBNull(releaseDateIndex)
                        ? (DateTime?)null
                        : reader.GetDateTime(releaseDateIndex);

                    int releasedByUserIdIndex = reader.GetOrdinal("ReleasedByUserID");

                    releasedByUserId = reader.IsDBNull(releasedByUserIdIndex)
                        ? (int?)null
                        : reader.GetInt32(releaseDateIndex);

                    int releaseApplicationIdIndex = reader.GetOrdinal("ReleaseApplicationID");

                    releaseApplicationId = reader.IsDBNull(releaseApplicationIdIndex)
                        ? (int?)null
                        : reader.GetInt32(releaseDateIndex);
                    return true;
                }

            }


        }

        public static bool GetDetinedLicenseInfoByLicenseId(
           int licenseId,
           ref int detainId,
           ref DateTime detainDate,
           ref decimal fineFees,
           ref int createdByUserId,
           ref bool isReleased,
           ref DateTime? releaseDate,
           ref int? releasedByUserId,
           ref int? releaseApplicationId)
        {


            const string query = @"
                              SELECT * 
             FROM DetainedLicenses
             WHERE LicenseID = @LicenseID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@LicenseID", SqlDbType.Int).Value = licenseId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (!reader.Read())
                        return false;

                    detainId = reader.GetInt32(reader.GetOrdinal("DetainID"));
                    detainDate = reader.GetDateTime(reader.GetOrdinal("DetainDate"));
                    fineFees = reader.GetDecimal(reader.GetOrdinal("FineFees"));
                    createdByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserID"));
                    isReleased = reader.GetBoolean(reader.GetOrdinal("IsReleased"));

                    int releaseDateIndex = reader.GetOrdinal("ReleaseDate");

                    releaseDate = reader.IsDBNull(releaseDateIndex)
                        ? (DateTime?)null
                        : reader.GetDateTime(releaseDateIndex);

                    int releasedByUserIdIndex = reader.GetOrdinal("ReleasedByUserID");

                    releasedByUserId = reader.IsDBNull(releasedByUserIdIndex)
                        ? (int?)null
                        : reader.GetInt32(releasedByUserIdIndex);

                    int releaseApplicationIdIndex = reader.GetOrdinal("ReleaseApplicationID");

                    releaseApplicationId = reader.IsDBNull(releaseApplicationIdIndex)
                        ? (int?)null
                        : reader.GetInt32(releaseApplicationIdIndex);
                    return true;
                }

            }


        }
        public static DataTable GetAllDetainedLicenses()
        {
            DataTable table = new DataTable();


            const string query = @" SELECT
    DL.DetainID,
    DL.LicenseID,
    DL.DetainDate,
    DL.IsReleased,
    DL.FineFees,
    DL.ReleaseDate,
    P.NationalNo,
    CONCAT(P.FirstName, ' ', P.SecondName, ' ', P.ThirdName, ' ', P.LastName) AS FullName,
    DL.ReleaseApplicationID
FROM DetainedLicenses AS DL
INNER JOIN Licenses AS L
    ON DL.LicenseID = L.LicenseID
INNER JOIN Drivers AS D
    ON L.DriverID = D.DriverID
INNER JOIN People AS P
    ON D.PersonID = P.PersonID;";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;

        }

        public static bool IsDetained(int licenseId)
        {

            const string query = @"
               SELECT
                   CASE
                       WHEN EXISTS
                       (
                           SELECT 1
                           FROM DetainedLicenses
                           WHERE LicenseID = @LicenseID
                             AND IsReleased = 0
                       )
                       THEN CAST(1 AS BIT)
                       ELSE CAST(0 AS BIT)
                   END";

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
