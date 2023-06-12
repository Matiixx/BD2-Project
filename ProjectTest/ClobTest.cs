using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProjectAPI;
using System.Linq;

namespace ProjectTest
{
    [TestClass]
    public class ClobTest
    {
        private string connectionString = "Data Source=WINSERV01;Initial Catalog=ProjectTest;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False";

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestAddClobFromStringWithoutLoggedIn()
        {
            string stringToAdd = "StringToAdd";
            var pf = new ProjectFunctions(this.connectionString);
            pf.createClobObjectFromString(stringToAdd, "NameStringToAdd");
        }

        [TestMethod]
        public void TestAddClobFromString()
        {
            string stringToAdd = "StringToAdd";
            var pf = new ProjectFunctions(this.connectionString);
            pf.loginUser("Mateusz", "password");
            Assert.AreEqual(1, pf.createClobObjectFromString(stringToAdd, "NameStringToAdd"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestAddClobFromFileWithoutLoggedIn()
        {
            string filepath = "C:\\Users\\Administrator\\Desktop\\BD2\\ProjectApp\\Data.txt";
            var pf = new ProjectFunctions(this.connectionString);
            Assert.AreEqual(1, pf.createClobObjectFromFile(filepath, "NameStringToAdd"));
        }

        [TestMethod]
        public void TestAddClobFromFile()
        {
            string stringToAdd = "C:\\Users\\Administrator\\Desktop\\BD2\\ProjectApp\\Data.txt";
            var pf = new ProjectFunctions(this.connectionString);
            pf.loginUser("Mateusz", "password");
            Assert.AreEqual(1, pf.createClobObjectFromFile(stringToAdd, "NameStringToAdd"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void TestAddClobFromFileThatNotExists()
        {
            string stringToAdd = "C:\\dataasd.txt";
            var pf = new ProjectFunctions(this.connectionString);
            pf.loginUser("Mateusz", "password");
            pf.createClobObjectFromFile(stringToAdd, "NameStringToAdd");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void TestGetUserDocumentsWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.getUserDocuments();
        }

        [TestMethod]
        public void TestGetUserDocuments()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            Assert.AreEqual(0, pf.getUserDocuments().Count);
            string document = "String";
            string name = "Doc1";
            pf.createClobObjectFromString(document, name);
            var res = pf.getUserDocuments();
            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(true, string.Equals(document, res[0].document));
            Assert.AreEqual(true, string.Equals(name, res[0].name));
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void TestSearchDocumentByTextWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.searchDocumentIdByText("Text to find");
        }

        [TestMethod]
        public void TestSearchDocumentByText()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc1";
            pf.createClobObjectFromString(document, name);
            pf.createClobObjectFromString(document + "1", name);
            pf.createClobObjectFromString(document + "2", name);
            pf.createClobObjectFromString("DifferentDoc", name);
            Assert.AreEqual(3, pf.searchDocumentIdByText(document).Length);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]

        public void TestSearchDocumentNamesByTextWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.searchDocumentNamesByText("TextToFind");
        }

        [TestMethod]
        public void TestSearchDocumentNamesByText()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc";
            pf.createClobObjectFromString(document, name);
            pf.createClobObjectFromString(document + "1", name + "1");
            pf.createClobObjectFromString(document + "2", name + "2");
            var res = pf.searchDocumentNamesByText(document);
            Assert.AreEqual(3, res.Length);
            Assert.AreEqual(0, String.Compare(name, res[0]));
            Assert.AreEqual(0, String.Compare(name + "1", res[1]));
            Assert.AreEqual(0, String.Compare(name + "2", res[2]));
        }

        [TestMethod]
        public void TestChangeNameOfDocument()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc";
            pf.createClobObjectFromString(document, name);
            var res = pf.searchDocumentNamesByText(document);
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(0, String.Compare(name, res[0]));
            var docId = pf.getUserDocuments()[0].id;
            string newName = "NewName";
            pf.updateNameOfDocument(docId, newName);
            string docName = pf.getUserDocuments()[0].name;
            Assert.AreEqual(0, String.Compare(newName, docName));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestWrongNewNameOfDocument()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc";
            pf.createClobObjectFromString(document, name);
            var docId = pf.getUserDocuments()[0].id;
            string newName = "";
            pf.updateNameOfDocument(docId, newName);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestChangeDocumentNameWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.updateNameOfDocument(1, "NewName");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestChangeDocumentWithWrongDocumentId()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            pf.updateNameOfDocument(106, "Newname");
        }

        [TestMethod]
        public void TestDeleteDocument()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc";
            pf.createClobObjectFromString(document, name);
            pf.createClobObjectFromString(document + "1", name + "1");
            pf.createClobObjectFromString(document + "2", name + "2");
            var res = pf.getUserDocuments();
            Assert.AreEqual(3, res.Count);
            pf.deleteDocumentWithId(res[0].id);
            res = pf.getUserDocuments();
            Assert.AreEqual(2, res.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeleteDocumentWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.deleteDocumentWithId(1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeleteDocumentWithWrongId()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            pf.deleteDocumentWithId(123);
        }

        [TestMethod]
        public void TestSearchDocumentIdsByName()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc1";
            pf.createClobObjectFromString(document, name);
            pf.createClobObjectFromString(document + "1", name);
            pf.createClobObjectFromString(document + "2", name);
            pf.createClobObjectFromString(document + "3", generateRandomString(10));
            Assert.AreEqual(3, pf.searchDocumentIdsByName(name).Length);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestSearchDocumentIdsByNameWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.searchDocumentIdsByName("name");
        }

        [TestMethod]
        public void TestSearchDocumentsByName()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            string document = "String";
            string name = "Doc1";
            pf.createClobObjectFromString(document, name);
            pf.createClobObjectFromString(document + "1", name);
            pf.createClobObjectFromString(document + "2", name);
            pf.createClobObjectFromString(document + "3", "DifferentName");
            Assert.AreEqual(3, pf.searchDocumentByName(name).Count);
            Assert.AreEqual(0, pf.searchDocumentByName(generateRandomString(10)).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestSearchDocumentByNameWithoutLoggedIn()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.searchDocumentByName("name");
        }


        private static string generateRandomString(int len)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
