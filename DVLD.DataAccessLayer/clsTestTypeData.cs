using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsTestTypeData
    {
        static public DataTable GetListTestTypes()
        {

            DataTable TestTypesDataTable = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT * FROM TestTypes";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    TestTypesDataTable.Load(reader);
                }
                reader.Close();

            }
            finally
            {
                connection.Close();
            }
            return TestTypesDataTable;

        }

        static public bool GetTestTypeInfoByID(int id, ref string title, ref string Description, ref decimal fees)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM TestTypes WHERE TestTypeID=@TestTypeID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TestTypeID", id);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    title = (string)reader["TestTypeTitle"];
                    Description = (string)reader["TestTypeDescription"];
                    fees = (decimal)reader["TestTypeFees"];

                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }

        static public bool UpdateTestType(int id, string title, string description, decimal fees)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE TestTypes
                            SET TestTypeTitle=@TestTypeTitle,
                               TestTypeDescription=@TestTypeDescription,
                              TestTypeFees=@TestTypeFees
                              WHERE TestTypeID=@TestTypeID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TestTypeID", id);
            command.Parameters.AddWithValue("@TestTypeTitle", title);
            command.Parameters.AddWithValue("@TestTypeDescription", description);
            command.Parameters.AddWithValue("@TestTypeFees", fees);

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
