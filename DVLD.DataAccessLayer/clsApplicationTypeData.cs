using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsApplicationTypeData
    {
        static public DataTable GetAllApplicationTypes()
        {

            DataTable ApplicationTypesDataTable = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT * FROM ApplicationTypes;";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    ApplicationTypesDataTable.Load(reader);
                }
                reader.Close();

            }
            finally
            {
                connection.Close();
            }
            return ApplicationTypesDataTable;
        }

        static public bool GetApplicationTypeInfoByID(int id, ref string title, ref decimal fees)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID=@ApplicationTypeID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationTypeID", id);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    title = (string)reader["ApplicationTypeTitle"];
                    fees = (decimal)reader["ApplicationFees"];

                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }

        static public bool UpdateApplicationType(int id, string title, decimal fees)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE ApplicationTypes
                            SET ApplicationTypeTitle=@ApplicationTypeTitle,
                            ApplicationFees=@ApplicationFees
                            WHERE ApplicationTypeID=@ApplicationTypeID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationTypeID", id);
            command.Parameters.AddWithValue("@ApplicationTypeTitle", title);
            command.Parameters.AddWithValue("@ApplicationFees", fees);

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
    }
}
