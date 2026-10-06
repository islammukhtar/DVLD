using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsDriverData
    {
        public static int AddNewDriver(
          int personId,
          int createdByUserId,
          DateTime createdDate)
        {

            const string query = @"
                        INSERT INTO Drivers
                (
                    PersonID,
                    CreatedByUserID,
                    CreatedDate
                )
                VALUES
                (
                    @PersonID,
                    @CreatedByUserID,
                    @CreatedDate
                );
                
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;
                command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = createdDate;

                connection.Open();

                object result = command.ExecuteScalar();

                return result != null
                    ? Convert.ToInt32(result)
                    : -1;

            }

        }

        public static bool UpdateDriver(
          int driverId,
          int personId,
          int createdByUserId,
          DateTime createdDate)
        {
            

            const string query = @"UPDATE Drivers
                        SET  PersonID = @PersonID
                        	 CreatedByUserID = @CreatedByUserID
                             CreatedDate = @CreatedDate
                             WHERE	DriverID = @DriverID";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
                command.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = createdByUserId;
                command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = createdDate;
                command.Parameters.Add("@DriverID", SqlDbType.Int).Value = driverId;
                connection.Open();

                return command.ExecuteNonQuery() == 1;
            }
        }

        public static bool IsPersonADriver(int personId)
        {
            using (SqlConnection connection =
                new SqlConnection(clsDataAccessSettings.connstr))
            {
                string query = @"
            SELECT CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM Drivers
                    WHERE PersonID = @PersonID
                )
                THEN 1
                ELSE 0
            END;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personId);

                    try
                    {
                        connection.Open();

                        return Convert.ToBoolean(command.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                       
                        return false;
                    }
                }
            }
        }


        public static bool GetDriverInfoByPersonId(
                int personId,
            ref int driverId,
            ref int createdByUserId,
            ref DateTime createdDate)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Drivers WHERE PersonID=@PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", personId);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    driverId = (int)reader["DriverID"];
                    createdByUserId = (int)reader["CreatedByUserID"];
                    createdDate = (DateTime)reader["CreatedDate"];
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }
        public static bool GetDriverInfoById(
              int driverId,
          ref int personId,
          ref int createdByUserId,
          ref DateTime createdDate)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Drivers WHERE DriverID = @DriverID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DriverID", driverId);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    personId = (int)reader["PersonID"];
                    createdByUserId = (int)reader["CreatedByUserID"];
                    createdDate = (DateTime)reader["CreatedDate"];
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }
        static public DataTable GetAllDrivers()
        {
            DataTable dtDrivers = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

           const string query = @"SELECT * FROM Drivers_View";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    dtDrivers.Load(reader);
                }
                reader.Close();

            }
            finally
            {
                connection.Close();
            }
            return dtDrivers;


        }

    }
}
