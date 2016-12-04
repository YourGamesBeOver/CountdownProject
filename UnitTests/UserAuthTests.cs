using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Countdown.Networking;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests {
    [TestClass]
    public class UserAuthTests {

        [TestInitialize]
        public void InitalizeTests()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            settings.Remove("username");
            settings.Remove("password");
            settings.Remove("loggedin");
        }

        [TestMethod]
        public void TestLoggedInIsFalseWithNoData()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            settings.Remove("loggedin");
            Assert.IsNull(settings["loggedin"]);
            Assert.IsFalse(AuthStorage.LoggedIn());
            Assert.IsNotNull(settings["loggedin"]);
            Assert.IsFalse((bool) settings["loggedin"]);
        }

        [TestMethod]
        public void TestProperlyRecallsAuth()
        {
            var auth = new UserAuth {Username = "USERNAME", Password = "PASSWORD"};
            AuthStorage.LogIn(auth);
            var response = AuthStorage.GetAuth();
            Assert.IsNotNull(response, "AuthStorage.GetAuth returned null");
            Assert.AreEqual(auth.Username, response.Value.Username, "Stored username does not match returned username");
            Assert.AreEqual(auth.Password, response.Value.Password, "Stored password does not match returned password");
            Assert.IsTrue(AuthStorage.LoggedIn());
        }

        [TestMethod]
        public void TestLoggingOutClearsData()
        {
            AuthStorage.LogOut();
            Assert.IsFalse(AuthStorage.LoggedIn());
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            Assert.IsNull(settings["username"]);
            Assert.IsNull(settings["password"]);
        }

        [TestMethod]
        public void TestLoggingOutTwiceCausesNoError()
        {
            AuthStorage.LogOut();
            AuthStorage.LogOut();
        }

        [TestMethod]
        public void TestLoggingInOverwritesLastValue()
        {
            var auth1 = new UserAuth { Username = "USERNAME", Password = "PASSWORD" };
            AuthStorage.LogIn(auth1);

            var auth2 = new UserAuth { Username = "USERNAME2", Password = "PASSWORD2" };
            AuthStorage.LogIn(auth2);

            var response = AuthStorage.GetAuth();
            Assert.IsNotNull(response, "AuthStorage.GetAuth returned null");
            Assert.AreEqual(auth2.Username, response.Value.Username, "Stored username does not match returned username");
            Assert.AreEqual(auth2.Password, response.Value.Password, "Stored password does not match returned password");
        }

        [TestMethod]
        public void GetAuthIsNullIfNotLoggedIn()
        {
            AuthStorage.LogOut();
            Assert.IsNull(AuthStorage.GetAuth());
        }
    }
}
