using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using ProjectAPI;


namespace ProjectAPI
{
    public partial class ProjectFunctions
    {
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]

        [Microsoft.SqlServer.Server.SqlProcedure]
        private static int createClobObjectFromString(string document, string name, string login, string connectionString)
        {
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand com = new SqlCommand("INSERT INTO[dbo].[Document] ([user_id], [name], [document]) VALUES (@user_id, @name, @document)", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;

                com.Parameters.Add("@name", SqlDbType.NChar);
                com.Parameters["@name"].Value = name;

                com.Parameters.Add("@document", SqlDbType.Text);
                com.Parameters["@document"].Value = document;

                int rowsAffected = com.ExecuteNonQuery();
                connection.Close();
                return rowsAffected;
            }
        }

        public int createClobObjectFromString(string document, string name)
        {
            isUserLoggedIn();
            return createClobObjectFromString(document, name, this.login, this.connectionString);
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        private static int createClobObjectFromFile(string fileName, string name, string login, string connectionString)
        {
            int user_id = getUserId(login, connectionString);
            string documentText = readClobFromFile(fileName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("INSERT INTO[dbo].[Document] ([user_id], [name], [document]) VALUES (@user_id, @name, @document)", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;

                com.Parameters.Add("@name", SqlDbType.NChar);
                com.Parameters["@name"].Value = name.Trim();

                com.Parameters.Add("@document", SqlDbType.Text);
                com.Parameters["@document"].Value = documentText;

                connection.Open();
                int rowsAffected = com.ExecuteNonQuery();
                connection.Close();
                return rowsAffected;
            }
        }

        public int createClobObjectFromFile(string filename, string name)
        {
            isUserLoggedIn();
            return createClobObjectFromFile(filename, name, this.login, this.connectionString);
        }

        private static string readClobFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        private List<PUserDocuments> searchDocumentsByText(string textToSearch, string login, string connectionString)
        {

            var docs = new System.Collections.Generic.List<PUserDocuments>();
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("SELECT * FROM [ProjectTest].[dbo].[Document] WHERE user_id=@user_id AND document LIKE @textToSearch;", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;
                com.Parameters.Add("@textToSearch", SqlDbType.Text);
                com.Parameters["@textToSearch"].Value = '%' + textToSearch + '%';

                connection.Open();
                var res = com.ExecuteReader();
                while (res.Read())
                {
                    docs.Add(new PUserDocuments
                    {
                        id = ((int)res["id"]),
                        name = ((string)res["name"]).Trim(),
                        document = (string)res["document"]
                    });
                }

                connection.Close();
            }
            return docs;
        }

        public List<PUserDocuments> searchDocumentsByText(string textToSearch)
        {
            isUserLoggedIn();
            return searchDocumentsByText(textToSearch, this.login, this.connectionString);
        }

        private int[] searchDocumentIdsByText(string textToSearch, string login, string connectionString)
        {

            var docs = new System.Collections.Generic.List<int>();
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("SELECT id FROM [ProjectTest].[dbo].[Document] WHERE user_id=@user_id AND document LIKE @textToSearch;", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;
                com.Parameters.Add("@textToSearch", SqlDbType.Text);
                com.Parameters["@textToSearch"].Value = '%' + textToSearch + '%';

                connection.Open();
                var res = com.ExecuteReader();
                while (res.Read())
                {
                    docs.Add((int)res["id"]);
                }

                connection.Close();
            }
            return docs.ToArray();
        }

        public int[] searchDocumentIdsByText(string textToSearch)
        {
            isUserLoggedIn();
            return searchDocumentIdsByText(textToSearch, this.login, this.connectionString);
        }

        private string[] searchDocumentNamesByText(string textToSearch, string login, string connectionString)
        {
            var names = new System.Collections.Generic.List<string>();
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("SELECT name FROM [ProjectTest].[dbo].[Document] WHERE user_id=@user_id;", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;

                connection.Open();
                var res = com.ExecuteReader();
                while (res.Read())
                {
                    names.Add(res.GetString(0).Trim());
                }
                connection.Close();
            }
            return names.ToArray();
        }

        public string[] searchDocumentNamesByText(string textToSearch)
        {
            isUserLoggedIn();
            return searchDocumentNamesByText(textToSearch, this.login, this.connectionString);
        }

        public class PUserDocuments
        {
            public int id { get; set; }
            public string name { get; set; }
            public string document { get; set; }
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "FillDocumentRow",
            TableDefinition = "name nchar(50), document nvarchar(MAX)"
            )]
        private static List<PUserDocuments> getUserDocuments(string login, string connectionString)
        {
            int user_id = getUserId(login, connectionString);
            var results = new List<PUserDocuments>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("SELECT [id], [name], [document] FROM [dbo].[Document] WHERE [Document].user_id = @user_id", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;

                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new PUserDocuments
                    {
                        id = ((int)reader["id"]),
                        name = ((string)reader["name"]).Trim(),
                        document = (string)reader["document"]
                    });
                }
                reader.Close();
            }
            return results;
        }

        public List<PUserDocuments> getUserDocuments()
        {
            isUserLoggedIn();
            return getUserDocuments(this.login, this.connectionString);
        }

        private int updateNameOfDocument(int documentId, string newName, string login, string connectionString)
        {
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("UPDATE [ProjectTest].[dbo].[Document] SET [name]=@newName WHERE [ProjectTest].[dbo].[Document].[id]=@doc_id AND [ProjectTest].[dbo].[Document].[user_id]=@userId;", connection);

                com.Parameters.Add("@newName", SqlDbType.NChar);
                com.Parameters["@newName"].Value = newName;
                com.Parameters.Add("@doc_id", SqlDbType.Int);
                com.Parameters["@doc_id"].Value = documentId;
                com.Parameters.Add("@userId", SqlDbType.Int);
                com.Parameters["@userId"].Value = user_id;

                connection.Open();
                int res = com.ExecuteNonQuery();
                connection.Close();
                if (res == 0)
                {
                    throw new Exception("Wrong doument ID");
                }
                return res;
            }
        }

        public int updateNameOfDocument(int documentId, string newName)
        {
            isUserLoggedIn();
            if (newName.Length == 0)
            {
                throw new Exception("Wrong document name");
            }
            return updateNameOfDocument(documentId, newName, this.login, this.connectionString);
        }

        private int deleteDocumentWithId(int documentId, string login, string connectionString)
        {
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("DELETE FROM [ProjectTest].[dbo].[Document] WHERE [ProjectTest].[dbo].[Document].[id]=@doc_id AND [ProjectTest].[dbo].[Document].[user_id]=@userId;", connection);

                com.Parameters.Add("@doc_id", SqlDbType.Int);
                com.Parameters["@doc_id"].Value = documentId;
                com.Parameters.Add("@userId", SqlDbType.Int);
                com.Parameters["@userId"].Value = user_id;

                connection.Open();
                int res = com.ExecuteNonQuery();
                connection.Close();
                if (res == 0)
                {
                    throw new Exception("Wrong doument ID");
                }
                return res;
            }
        }

        public int deleteDocumentWithId(int documentId)
        {
            isUserLoggedIn();
            return deleteDocumentWithId(documentId, this.login, this.connectionString);
        }

        private int[] searchDocumentIdsByName(string documentName, string login, string connectionStrin)
        {
            var ids = new System.Collections.Generic.List<int>();
            int user_id = getUserId(login, connectionString);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("SELECT id FROM [ProjectTest].[dbo].[Document] WHERE user_id=@user_id AND name LIKE @documentName;", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;
                com.Parameters.Add("@documentName", SqlDbType.NChar);
                com.Parameters["@documentName"].Value = '%' + documentName + '%';

                connection.Open();
                var res = com.ExecuteReader();
                while (res.Read())
                {
                    ids.Add(res.GetInt32(0));
                }
                connection.Close();
            }
            return ids.ToArray();
        }

        public int[] searchDocumentIdsByName(string documentName)
        {
            isUserLoggedIn();
            return searchDocumentIdsByName(documentName, this.login, this.connectionString);
        }


        private List<PUserDocuments> searchDocumentByName(string documentName, string login, string connectionString)
        {
            int user_id = getUserId(login, connectionString);
            var results = new List<PUserDocuments>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand com = new SqlCommand("SELECT [id], [name], [document] FROM [dbo].[Document] WHERE [Document].user_id = @user_id AND [Document].name LIKE @documentName", connection);

                com.Parameters.Add("@user_id", SqlDbType.Int);
                com.Parameters["@user_id"].Value = user_id;
                com.Parameters.Add("@documentName", SqlDbType.NChar);
                com.Parameters["@documentName"].Value = '%' + documentName + '%';

                connection.Open();
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new PUserDocuments
                    {
                        id = ((int)reader["id"]),
                        name = ((string)reader["name"]).Trim(),
                        document = (string)reader["document"]
                    });
                }
                reader.Close();
            }
            return results;
        }

        public List<PUserDocuments> searchDocumentByName(string documentName)
        {
            isUserLoggedIn();
            return searchDocumentByName(documentName, this.login, this.connectionString);
        }

        private static void FillDocumentRow(object obj, out string name, out string document)
        {
            var reader = (PUserDocuments)obj;
            name = reader.name;
            document = reader.document;
        }

    }
}