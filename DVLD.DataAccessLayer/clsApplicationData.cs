using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsApplicationData
    {

        static public bool GetApplicationInfoByID(
            int applicationId,
            ref int ApplicantPersonID ,
            ref DateTime ApplicationDate,
            ref int ApplicationTypeID,
            ref int ApplicationStatus,
            ref  DateTime LastStatusDate,
            ref decimal PaidFees,
            ref  int CreatedByUserID)
        {

            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT * FROM Applications WHERE ApplicationID=@ApplicationID";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);
            

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read()) {

                    isFound = true;
                    ApplicantPersonID = (int)reader["ApplicantPersonID"];
                    ApplicationDate = (DateTime)reader["ApplicationDate"];
                    ApplicationTypeID = (int)reader["ApplicationTypeID"];
                    ApplicationStatus = Convert.ToInt32(reader["ApplicationStatus"]);
                    LastStatusDate = (DateTime)reader["LastStatusDate"];
                    PaidFees = (decimal)reader["PaidFees"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                
                }

            }
            finally
            {
                connection.Close();
            }

            return isFound;

        }

        static public DataTable GetAllApplications()
        {
            DataTable ApplicationDataTable = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT *FROM Applications";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    ApplicationDataTable.Load(reader);
                }
                reader.Close();

            }
            finally
            {

                connection.Close();
            }
            return ApplicationDataTable;
        }

        static public bool CancelApplication(int ApplicationID)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE Applications
                            SET ApplicationStatus=2
                         WHERE ApplicationID=@ApplicationID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();

                rowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
            return (rowsAffected > 0);

        }

        static public int GetActiveApplicationIDForPersonByStatus(int PersonID, int LicenseClassID)
        {
            int ActiveApplicationID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @" SELECT TOP 1 Applications.ApplicationID FROM LocalDrivingLicenseApplications INNER JOIN Applications
                              ON LocalDrivingLicenseApplications.ApplicationID=Applications.ApplicationID
                              WHERE Applications.ApplicantPersonID= @ApplicantPersonID AND LocalDrivingLicenseApplications.LicenseClassID= @LicenseClassID AND Applications.ApplicationStatus <>2";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int inserted))
                {
                    ActiveApplicationID = inserted;
                }

            }
            finally
            {

                connection.Close();
            }

            return ActiveApplicationID;
        }

        static public int AddNewApplication(
            int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,
            int ApplicationStatus, 
            DateTime LastStatusDate,
            decimal PaidFees,
            int CreatedByUserID)
        {
            int ApplicationID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"INSERT INTO Applications (ApplicantPersonID,ApplicationDate,ApplicationTypeID,ApplicationStatus,LastStatusDate,PaidFees,CreatedByUserID)
                          VALUES (@ApplicantPersonID,@ApplicationDate,@ApplicationTypeID,@ApplicationStatus, @LastStatusDate,@PaidFees,@CreatedByUserID);
                                  SELECT SCOPE_IDENTITY();";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
            command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int inserted))
                {
                    ApplicationID = inserted;
                }

            }
            finally
            {
                connection.Close();
            }

            return ApplicationID;

        }

        static public bool UpdateApplication(
            int applicationId,
            int ApplicantPersonID,
            DateTime ApplicationDate, 
            int ApplicationTypeID,
            int ApplicationStatus,
            DateTime LastStatusDate,
            decimal PaidFees, 
            int CreatedByUserID)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"Update  Applications  
                            set ApplicantPersonID = @ApplicantPersonID,
                                ApplicationDate = @ApplicationDate,
                                ApplicationTypeID = @ApplicationTypeID,
                                ApplicationStatus = @ApplicationStatus, 
                                LastStatusDate = @LastStatusDate,
                                PaidFees = @PaidFees,
                                CreatedByUserID=@CreatedByUserID
                            where ApplicationID=@ApplicationID";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);
            command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
            command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
            command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
              
            }
            finally
            {
                connection.Close();
            }

            return rowsAffected > 0;

        }
        static public bool DeleteApplication(int applicationId)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "DELETE FROM Applications WHERE ApplicationID = @ApplicationID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);

            try
            {
                connection.Open();

                rowsAffected = command.ExecuteNonQuery();

            }
            catch
            {
                rowsAffected = 0;
            }
            finally
            {
                connection.Close();
            }
            return rowsAffected > 0;

        }

        static public bool IsApplicationExist(int applicationId) {

            bool isExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT Found=1 FROM Applications WHERE ApplicationID =@ApplicationID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    isExist = true;
                }
                reader.Close();
            }
            finally
            {

                connection.Close();
            }
            return isExist;

        }

        static public int GetActiveApplicationIdForLicenseClass(int applicantPersonId,int applicationTypeId, int licenseClassID) {
            
            int activeApplicationId = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @" SELECT Applications.ApplicationID FROM LocalDrivingLicenseApplications INNER JOIN Applications
                              ON LocalDrivingLicenseApplications.ApplicationID=Applications.ApplicationID
                              WHERE 
                              Applications.ApplicantPersonID=@ApplicantPersonID AND
                              Applications.ApplicationTypeID=@ApplicationTypeID AND
                              LocalDrivingLicenseApplications.LicenseClassID=@LicenseClassID AND
                              Applications.ApplicationStatus=1";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicantPersonID", applicantPersonId);
            command.Parameters.AddWithValue("@ApplicationTypeID", applicationTypeId);
            command.Parameters.AddWithValue("@LicenseClassID", licenseClassID);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int inserted))
                {
                    activeApplicationId = inserted;
                }

            }
            finally
            {

                connection.Close();
            }

            return activeApplicationId;
        }

        static public bool UpdateStatus(int applicationId,int newStatus)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE Applications
                            SET Applications.ApplicationStatus=@ApplicationStatus
                            WHERE Applications.ApplicationID=@ApplicationID";
                            

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", applicationId);
            command.Parameters.AddWithValue("@ApplicationStatus", newStatus);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
             
            }
            finally
            {
                connection.Close();
            }

            return rowsAffected > 0;
        }


       
    }
}
