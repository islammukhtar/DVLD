using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;
using System.Data.SqlTypes;

namespace DVLD.DataAccessLayer
{
    public class clsLocalDrivingLicenseApplicationData
    {

        static public bool GetLocalDrivingLicenseApplicationById(int localDrivingLicenseApplicationId,
            ref int applicationID,ref int licenseClass)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT *FROM LocalDrivingLicenseApplications WHERE LocalDrivingLicenseApplicationID=
                              @LocalDrivingLicenseApplicationID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplicationId);
            
            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    applicationID = (int)reader["ApplicationID"];
                    licenseClass = (int)reader["LicenseClassID"];
                    
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;

        }

        static public DataTable GetAllLocalDrivingLicensesApplications()
        {

            DataTable LocalDrivingLicensesApplicationsDataTable = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"
             SELECT
             L.LocalDrivingLicenseApplicationID AS LDLAppID,
             LC.ClassName,
             P.NationalNo,
             CONCAT(P.FirstName, ' ', P.SecondName, ' ', P.ThirdName, ' ', P.LastName) AS FullName,
             A.ApplicationDate,
             ISNULL(PT.PassedTests, 0) AS PassedTests,
             CASE A.ApplicationStatus
                 WHEN 1 THEN 'New'
                 WHEN 2 THEN 'Cancelled'
                 ELSE 'Completed'
             END AS Status
         FROM LocalDrivingLicenseApplications L
         INNER JOIN Applications A
             ON L.ApplicationID = A.ApplicationID
         INNER JOIN LicenseClasses LC
             ON L.LicenseClassID = LC.LicenseClassID
         INNER JOIN People P
             ON A.ApplicantPersonID = P.PersonID
         OUTER APPLY
         (
             SELECT COUNT(T.TestID) AS PassedTests
             FROM TestAppointments TA
             INNER JOIN Tests T
                 ON TA.TestAppointmentID = T.TestAppointmentID
             WHERE TA.LocalDrivingLicenseApplicationID = L.LocalDrivingLicenseApplicationID
               AND T.TestResult = 1
         ) PT
		 ORDER BY LDLAppID DESC;";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    LocalDrivingLicensesApplicationsDataTable.Load(reader);
                }
                reader.Close();

            }
            finally
            {
                connection.Close();
            }
            return LocalDrivingLicensesApplicationsDataTable;
        }
        static public int AddNewLocalDrivingLicenseApplication(int ApplicationID, int LicenseClass)
        {
            int LocalDrivingLicenseApplication = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"INSERT INTO LocalDrivingLicenseApplications (ApplicationID,LicenseClassID)
                            VALUES(@ApplicationID,@LicenseClassID)
                            SELECT SCOPE_IDENTITY();";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@LicenseClassID", LicenseClass);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int inserted))
                {
                    LocalDrivingLicenseApplication = inserted;
                }

            }
            finally
            {
                connection.Close();
            }

            return LocalDrivingLicenseApplication;

        }
        static public bool UpdateLocalDrivingLicenseApplication(int localDrivingLicenseApplication,int applicationID, int licenseClass)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE LocalDrivingLicenseApplications
                            SET ApplicationID=@ApplicationID,
                                LicenseClassID=@LicenseClassID
                                WHERE LocalDrivingLicenseApplicationID=@LocalDrivingLicenseApplicationID";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplication);
            command.Parameters.AddWithValue("@ApplicationID", applicationID);
            command.Parameters.AddWithValue("@LicenseClassID", licenseClass);

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
        static public bool DeleteLocalDrivingLicenseApplication(int localDrivingLicenseApplication)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"DELETE FROM LocalDrivingLicenseApplications 
                       WHERE LocalDrivingLicenseApplicationID =@LocalDrivingLicenseApplicationID";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", localDrivingLicenseApplication);
           
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                connection.Close();
            }

            return rowsAffected > 0;

        }


    }
}
