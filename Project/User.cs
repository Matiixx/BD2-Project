using System;
using System.Data;
using System.Data.SqlClient;

namespace ProjectAPI
{
    public partial class ProjectFunctions
    {
        private string connectionString;

        private string login;

        public ProjectFunctions(string connString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                new SqlCommand("SELECT 1", connection).ExecuteScalar();
                connection.Close();
            }
            this.connectionString = connString;
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        private static int createUser(string login, string password, string connectionString)
        {
            if (isLoginExist(login, connectionString))
            {
                throw new Exception("User with given login exists");
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("INSERT INTO[dbo].[User] ([login],[password]) VALUES (@login, @password)", connection);
                com.Parameters.Add("@login", SqlDbType.NChar);
                com.Parameters["@login"].Value = login;
                com.Parameters.Add("@password", SqlDbType.NChar);
                com.Parameters["@password"].Value = password;
                int rowsAffected = com.ExecuteNonQuery();
                connection.Close();
                return rowsAffected;
            }
        }

        public int createUser(string login, string password)
        {
            return createUser(login, password, this.connectionString);
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        private static int updatePassword(string login, string password, string connectionString)
        {
            if (!isLoginExist(login, connectionString))
            {
                throw new Exception("User does not exist");
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("UPDATE [dbo].[User] SET [password]=@password WHERE [login]=@login", connection);
                com.Parameters.Add("@login", SqlDbType.NChar);
                com.Parameters["@login"].Value = login;
                com.Parameters.Add("@password", SqlDbType.NChar);
                com.Parameters["@password"].Value = password;
                int rowsAffected = com.ExecuteNonQuery();
                connection.Close();
                return rowsAffected;
            }
        }

        private bool checkOldPasword(string oldPassword, string login, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand com = new SqlCommand("SELECT COUNT(*) FROM [dbo].[User] WHERE [login]=@login AND password=@oldPassword", connection);
                com.Parameters.Add("@login", SqlDbType.NChar);
                com.Parameters["@login"].Value = login;
                com.Parameters.Add("@oldPassword", SqlDbType.NChar);
                com.Parameters["@oldPassword"].Value = oldPassword;


                int count = (int)com.ExecuteScalar();
                connection.Close();
                return count > 0;
            }
        }

        public int updatePassword(string oldPassword, string newPassword)
        {
            isUserLoggedIn();
            if (checkOldPasword(oldPassword, this.login, this.connectionString))
            {
                return updatePassword(this.login, newPassword, this.connectionString);
            }
            return 0;
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        private static int deleteUser(string login, string connectionString)
        {
            if (!isLoginExist(login, connectionString))
            {
                throw new Exception("User does not exist");
            }
            int userId = getUserId(login, connectionString);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com1 = new SqlCommand("DELETE FROM [dbo].[Document] WHERE [user_id]=@userId", connection);
                com1.Parameters.Add("@userId", SqlDbType.Int);
                com1.Parameters["@userId"].Value = userId;
                int rowsAffected = com1.ExecuteNonQuery();

                SqlCommand com2 = new SqlCommand("DELETE FROM [dbo].[User] WHERE [login]=@login", connection);
                com2.Parameters.Add("@login", SqlDbType.NChar);
                com2.Parameters["@login"].Value = login;
                rowsAffected += com2.ExecuteNonQuery();
                connection.Close();
                return rowsAffected;
            }
        }

        public int deleteUser()
        {
            isUserLoggedIn();
            return deleteUser(this.login, this.connectionString);
        }


        private static bool isLoginExist(string login, string connectionString)
        {
            bool isExist = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("SELECT * FROM [dbo].[User] WHERE login=@login", connection);
                com.Parameters.Add("@login", SqlDbType.NChar);
                com.Parameters["@login"].Value = login;

                using (SqlDataReader reader = com.ExecuteReader())
                {
                    if (reader.HasRows) isExist = true;
                }
                connection.Close();
            }
            return isExist;
        }

        private static int getUserId(string login, string connectionString)
        {
            if (!isLoginExist(login, connectionString))
            {
                throw new Exception("User does not exist");
            }
            int userId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("SELECT [id] FROM [dbo].[User] WHERE login=@login", connection);
                com.Parameters.Add("@login", SqlDbType.NChar);
                com.Parameters["@login"].Value = login;

                using (SqlDataReader reader = com.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userId = (int)reader["id"];
                    }
                }
                connection.Close();
            }
            return userId;
        }

        public bool loginUser(string login, string password)
        {
            if (!isLoginExist(login, this.connectionString))
            {
                throw new Exception("User does not exist");
            }
            int count = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("SELECT Count(*) FROM [dbo].[User] WHERE login=@login AND password=@password", connection);
                com.Parameters.Add("@login", SqlDbType.NChar);
                com.Parameters["@login"].Value = login;
                com.Parameters.Add("@password", SqlDbType.NChar);
                com.Parameters["@password"].Value = password;

                count = (int)com.ExecuteScalar();
                connection.Close();
            }
            if(count == 0)
            {
                throw new Exception("Wrong password");
            }
            this.login = login;
            return count > 0;
        }

        private void isUserLoggedIn()
        {
            if (login == null)
            {
                throw new Exception("User not logged in");
            }
        }
    }
}