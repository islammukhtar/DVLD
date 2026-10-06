using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccessLayer
{
    public class clsUserData
    {
        static public DataTable GetAllUsers()
        {
            DataTable UsersDataTable = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"SELECT Users.UserID,Users.PersonID,CONCAT( People.FirstName,' ', People.SecondName,' ',People.ThirdName,' ', People.LastName) as FullName,Users.UserName,Users.IsActive
                            FROM Users INNER JOIN People ON Users.PersonID=People.PersonID;";

            SqlCommand cmd = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    UsersDataTable.Load(reader);
                }
                reader.Close();

            }
            finally
            {
                connection.Close();
            }
            return UsersDataTable;


        }

        static public bool IsUserExist(int userId)
        {


            bool isExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT Found=1 FROM Users WHERE UserID =@UserID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);

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

        static public bool IsThePersonLinkedToAUser(int personID)
        {
            bool isExist = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT FOUND=1 FROM Users WHERE PersonID=@PersonID";

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

        static public int AddNewUser(int PersonID, string UserName, string passoword, bool isActive)
        {
            int userID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"INSERT INTO Users (PersonID,UserName,Password,IsActive)VALUES(@PersonID,@UserName,@Password,@IsActive);
                                  SELECT SCOPE_IDENTITY();";


            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@Password", passoword);
            command.Parameters.AddWithValue("@IsActive", isActive);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int inserted))
                {
                    userID = inserted;
                }

            }
            finally
            {
                connection.Close();
            }

            return userID;

        }

        static public bool UpdateUser(int userID, string userName, string password, bool isActive)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE Users
                            SET UserName=@UserName,
                            Password=@Password,
                            IsActive=@IsActive
                            WHERE UserID=@UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@IsActive", isActive);

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

        static public bool DeleteUser(int userID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "DELETE FROM Users WHERE UserID = @UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", userID);

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
        static public bool IsUserIsActive(string userName)
        {

            bool isActive = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT IsActive FROM Users WHERE UserName=@UserName";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserName", userName);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isActive = (bool)reader["IsActive"];
                }
            }
            finally
            {
                connection.Close();
            }
            return isActive;







        }
        static public bool GetUserInfoByUserName(string userName, ref int userID, ref int personID, ref string password, ref bool isActive)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Users WHERE UserName=@UserName";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserName", userName);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    userID = (int)reader["UserID"];
                    personID = (int)reader["PersonID"];
                    password = (string)reader["Password"];
                    isActive = (bool)reader["IsActive"];
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }
        static public bool GetUserInfoByID(int userID, ref int personID, ref string userName, ref string password, ref bool isActive)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Users WHERE UserID=@UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", userID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    personID = (int)reader["PersonID"];
                    userName = (string)reader["UserName"];
                    password = (string)reader["Password"];
                    isActive = (bool)reader["IsActive"];
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }
        static public bool GetUserInfoByUserNameAndPassword(string userName, string password,ref int userID, ref int personID,  ref bool isActive)
        {
            bool isfound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = "SELECT * FROM Users WHERE UserName=@UserName AND Password=@Password";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserName", userName);
            command.Parameters.AddWithValue("@Password", password);
            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isfound = true;
                    personID = (int)reader["PersonID"];
                    userID = (int)reader["UserID"];
                    isActive = (bool)reader["IsActive"];
                }
            }
            finally
            {
                connection.Close();
            }
            return isfound;
        }
        static public bool ChangePassword(int userId,string newPassword)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.connstr);

            string query = @"UPDATE Users
                            SET Password=@Password
                            WHERE UserID=@UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@Password", newPassword);
        

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
