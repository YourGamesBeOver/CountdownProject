using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Countdown.Networking;
using Countdown.Networking.Results;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests
{
    [TestClass]
    public class ServerConnectionTests {
        private const string ServerUrl = "http://129.22.62.229:5000";
        private const string Username = "user_1";
        private const string Password = "password_1";

        [TestMethod]
        public async System.Threading.Tasks.Task TestLogIn()
        {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
            var response = await con.LogIn(new UserAuth {Username = Username, Password = Password});
            Assert.AreEqual(LoginResult.Success, response, "LogIn did not return LoginResult.Success");
        }
    }
}
