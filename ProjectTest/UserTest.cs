using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProjectAPI;
using System.Linq;

namespace ProjectTest
{
    [TestClass]
    public class UserTest
    {

        private string connectionString = "Data Source=WINSERV01;Initial Catalog=ProjectTest;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False";

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TestWrongSqlConnectionString()
        {
            new ProjectFunctions("Wrong sql connection string");
        }

        [TestMethod]
        public void TestCorrectSqlConnectionString()
        {
            var pf = new ProjectFunctions(this.connectionString);
            Assert.IsInstanceOfType(pf, typeof(ProjectFunctions));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestDeleteUserWithoutLogin()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.deleteUser();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestUpdatePasswordWithoutLogin()
        {
            var pf = new ProjectFunctions(this.connectionString);
            pf.updatePassword("123", "321");
        }

        [TestMethod]
        public void TestCreateNewUser()
        {
            var pf = new ProjectFunctions(this.connectionString);
            int count = pf.createUser(generateRandomString(10), generateRandomString(12));
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void TestLoginUser()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            Assert.AreEqual(true, pf.loginUser(login, password));
        }

        [TestMethod]
        public void TestChangePasswordWrong()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            Assert.AreEqual(0, pf.updatePassword("WrongPassowrd12312321", "NewPassword"));
        }

        [TestMethod]
        public void TestChangePassword()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            Assert.AreEqual(1, pf.updatePassword(password, "NewPassword"));
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            var pf = new ProjectFunctions(this.connectionString);
            string login = generateRandomString(10);
            string password = generateRandomString(12);
            pf.createUser(login, password);
            pf.loginUser(login, password);
            Assert.AreEqual(1, pf.deleteUser());
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
