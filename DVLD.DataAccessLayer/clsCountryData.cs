using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsCountryData
    {
        static public bool GetCountryInfoByID(int ID, ref string CountryName)
        {

            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Countries WHERE CountryID =@CountryID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CountryID", ID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    CountryName = (string)reader["CountryName"];
                }

            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }
        static public bool GetCountryInfoByName(string CountryName, ref int ID)
        {

            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Countries WHERE CountryName =@CountryName";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CountryName", CountryName);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    ID = (int)reader["CountryID"];
                }

            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }
        static public DataTable GetAllCountries()
        {
            DataTable CountriesDataTable = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Countries";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    CountriesDataTable.Load(reader);
                }
                reader.Close();

            }
            finally
            {

                connection.Close();
            }
            return CountriesDataTable;
        }
    }
}
