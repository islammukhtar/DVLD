using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD.DataAccessLayer
{
    public class clsPersonData
    {
        static public bool GetPersonInfoByID(
            int ID,
            ref string nationalNo,
            ref string firstName,
            ref string secondName,
            ref string thirdName,
            ref string lastName,
            ref DateTime dateOfBirth,
            ref short Gender,
            ref string address,
            ref string phone,
            ref string email,
            ref int nationalityCountryID,
            ref string imagePath)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM People WHERE PersonID=@PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", ID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    nationalNo = (string)reader["NationalNo"];
                    firstName = (string)reader["FirstName"];
                    secondName = (string)reader["SecondName"];
                    thirdName = (string)reader["ThirdName"];
                    Gender = Convert.ToInt16(reader["Gender"]);
                    dateOfBirth = (DateTime)reader["DateOfBirth"];
                    address = (string)reader["Address"];
                    phone = (string)reader["Phone"];
                    nationalityCountryID = (int)reader["NationalityCountryID"];
                    email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : "";
                    imagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
                    lastName = reader["LastName"] != DBNull.Value ? (string)reader["LastName"] : "";
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }

        static public bool GetPersonInfoByNationalNo(
            ref int ID, 
            string nationalNo,
            ref string firstName, 
            ref string secondName, 
            ref string thirdName,
            ref string lastName,
            ref DateTime dateOfBirth,
            ref short Gender,
            ref string address, 
            ref string phone, 
            ref string email, 
            ref int nationalityCountryID,
            ref string imagePath)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM People WHERE NationalNo=@NationalNo";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@NationalNo", nationalNo);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    ID = (int)reader["PersonID"];
                    firstName = (string)reader["FirstName"];
                    secondName = (string)reader["SecondName"];
                    thirdName = (string)reader["ThirdName"];
                    Gender = Convert.ToInt16(reader["Gender"]);
                    dateOfBirth = (DateTime)reader["DateOfBirth"];
                    address = (string)reader["Address"];
                    phone = (string)reader["Phone"];
                    nationalityCountryID = (int)reader["NationalityCountryID"];
                    email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : "";
                    imagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
                    lastName = reader["LastName"] != DBNull.Value ? (string)reader["LastName"] : "";
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }


        public static DataTable GetAllPeople()
        {
            DataTable peopleTable = new DataTable();

            const string query = @"
        SELECT
            PersonID,
            NationalNo,
            FirstName,
            SecondName,
            ThirdName,
            LastName,
            CASE
                WHEN Gender = 0 THEN 'Male'
                WHEN Gender = 1 THEN 'Female'
            END AS Gender,
            DateOfBirth,
            Countries.CountryName AS Nationality,
            Phone,
            Email
        FROM People
        INNER JOIN Countries
            ON People.NationalityCountryID = Countries.CountryID;";

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.connstr))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    peopleTable.Load(reader);
                }
            }

            return peopleTable;
        }

        static public int AddNewPerson(
            string nationalNo,
            string firstName, 
            string secondName, 
            string thirdName,
            string lastName,
            DateTime DateOfBirth,
            short Gender,
            string address,
            string phone,
            string email, 
            int nationalityCountryID,
            string imagePath)
        {
            int personID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"INSERT INTO People (NationalNo,FirstName,SecondName,ThirdName,LastName,DateOfBirth,Gender,Address,Phone,Email,NationalityCountryID,ImagePath)
                          VALUES (@NationalNo,@FirstName,@SecondName,@ThirdName, @LastName,@DateOfBirth,@Gender,@Address, @Phone,@Email, @NationalityCountryID,@ImagePath);
                                  SELECT SCOPE_IDENTITY();";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NationalNo", nationalNo);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", thirdName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", address);
            command.Parameters.AddWithValue("@Phone", phone);

            command.Parameters.AddWithValue("@NationalityCountryID", nationalityCountryID);
            if (lastName != "")
            {
                command.Parameters.AddWithValue("@LastName", lastName);
            }
            else
            {
                command.Parameters.AddWithValue("@LastName", DBNull.Value);
            }

            if (email != "")
            {
                command.Parameters.AddWithValue("@Email", email);
            }
            else
            {
                command.Parameters.AddWithValue("@Email", DBNull.Value);
            }

            if (imagePath != "")
            {
                command.Parameters.AddWithValue("@ImagePath", imagePath);

            }
            else
            {
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
            }

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int inserted))
                {
                    personID = inserted;
                }

            }
            finally
            {
                connection.Close();
            }

            return personID;

        }


        static public bool UpdatePerson(int personID, string nationalNo, string firstName, string secondName, string thirdName, string lastName, DateTime DateOfBirth, short Gender,
            string address, string phone, string email, int nationalityCountryID, string imagePath)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE People
                          SET NationalNo=@NationalNo,
                              FirstName = @FirstName,
                              SecondName=@SecondName,
                              ThirdName=@ThirdName,
                              LastName=@LastName,
                              DateOfBirth=@DateOfBirth,
                              Gender=@Gender,
                              Address=@Address,
                              Phone=@Phone,
                              Email=@Email,
                              NationalityCountryID=@NationalityCountryID,
                              ImagePath=@ImagePath
                              WHERE PersonID=@PersonID";




            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", personID);
            command.Parameters.AddWithValue("@NationalNo", nationalNo);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@SecondName", secondName);
            command.Parameters.AddWithValue("@ThirdName", thirdName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", address);
            command.Parameters.AddWithValue("@Phone", phone);
            command.Parameters.AddWithValue("@NationalityCountryID", nationalityCountryID);

            if (lastName != "")
            {
                command.Parameters.AddWithValue("@LastName", lastName);
            }
            else
            {
                command.Parameters.AddWithValue("@LastName", DBNull.Value);
            }
            if (email != "")
            {
                command.Parameters.AddWithValue("@Email", email);
            }
            else
            {
                command.Parameters.AddWithValue("@Email", DBNull.Value);
            }

            if (imagePath != "")
            {
                command.Parameters.AddWithValue("@ImagePath", imagePath);

            }
            else
            {
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
            }

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

        static public bool DeletePerson(int personID)
        {
            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "DELETE FROM People WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", personID);

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
        static public bool IsPersonExist(string nationalNo)
        {

            bool isExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT Found=1 FROM People WHERE NationalNo =@NationalNo";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NationalNo", nationalNo);

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

        static public bool IsPersonExist(int personID)
        {
            bool isExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT Found=1 FROM People WHERE PersonID =@PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", personID);

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

    }
}
