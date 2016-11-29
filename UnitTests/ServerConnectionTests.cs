using System;
using System.Linq;
using System.Threading;
using Windows.UI;
using Countdown.Networking;
using Countdown.Networking.Results;
using Countdown.Networking.Serialization;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AsyncTask = System.Threading.Tasks.Task;

namespace UnitTests
{
    [TestClass]
    public class ServerConnectionTests {
        private const string ServerUrl = "http://localhost:5000";
        private const string Username = "user_1";
        private const string Password = "password_1";

        private static readonly Task TestTask = new Task
        {
            TaskId = -1,
            Name = "Test Task",
            Description = "Test Task Description",
            DueDate = DateTime.Now.Add(TimeSpan.FromDays(20)),
            CreationDate = DateTime.Now,
            LastModifiedTime = DateTime.Now,
            BackgroundColor = new Color {R = 0x12, G = 0x34, B = 0x56},
            ForegroundColor = new Color {R = 0x78, G = 0x9A, B = 0xBC},
            Priority = 0,
            IsCompleted = false
        };

        [TestMethod]
        public void TestTaskEquals()
        {
            Assert.IsFalse(TestTask.Equals(null));
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(TestTask.Equals(TestTask));
            Assert.AreEqual(TestTask, TestTask.DeepClone(), "Deep clone is not equal");
            Assert.AreEqual(TestTask.GetHashCode(), TestTask.DeepClone().GetHashCode(), "deep clone hash code is not equal");
        }

        [TestMethod]
        public void TestColorConverter()
        {
            var instance = new ColorJsonConverter();
            Assert.IsTrue(instance.CanConvert(typeof(Color)));
            Assert.IsFalse(instance.CanConvert(typeof(string)));
        }

        [TestMethod]
        public void CannotConnectTwice()
        {
            var con = new ServerConnection();
            con.Connect(ServerUrl);
            Assert.ThrowsException<ServerConnection.ConnectionException>(() => con.Connect(ServerUrl));
        }

        [TestMethod]
        public async AsyncTask CannotLogInTwice()
        {
            var con = new ServerConnection();
            con.Connect(ServerUrl);
            var auth = new UserAuth {Username = Username, Password = Password};
            await con.LogIn(auth);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.LogIn(auth));
        }

        [TestMethod]
        public async AsyncTask CannotLoginIfNotConnected()
        {
            var con = new ServerConnection();
            await
                AssertThrowsAsync<ServerConnection.ConnectionException>(
                    async () => await con.LogIn(new UserAuth {Username = Username, Password = Password}));
        }

        [TestMethod]
        public async AsyncTask TestLogIn()
        {
            using (var con = new ServerConnection())
            {
                Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
                con.Connect(ServerUrl);
                Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
                var response = await con.LogIn(new UserAuth {Username = Username, Password = Password});
                Assert.AreEqual(LoginResult.Success, response, "LogIn did not return LoginResult.Success");
            }
        }

        [TestMethod]
        public async AsyncTask TestLoginWithBadCredentials()
        {
            using (var con = new ServerConnection())
            {
                Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
                con.Connect(ServerUrl);
                Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
                var response =
                    await con.LogIn(new UserAuth {Username = "NOT A VALID USERNAME!!!", Password = "INVALID PASSWORD"});
                Assert.AreEqual(LoginResult.BadParams, response, "LogIn did not return LoginResult.BadParams");
            }
        }

        [TestMethod]
        public async AsyncTask CannotGetTasksIfNotConnected()
        {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetTasksForUser());
        }

        [TestMethod]
        public async AsyncTask CannotGetInactiveTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetInactiveTasksForUser());
        }

        [TestMethod]
        public async AsyncTask CannotCreateTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.CreateTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotDeleteTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.DeleteTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotGetSubTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetSubTasksForTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotGetNextTaskIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetNextCountdown());
        }


        [TestMethod]
        public async AsyncTask CannotGetTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetTasksForUser());
        }

        [TestMethod]
        public async AsyncTask CannotGetInactiveTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetInactiveTasksForUser());
        }

        [TestMethod]
        public async AsyncTask CannotCreateTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.CreateTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotDeleteTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.DeleteTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotGetSubTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetSubTasksForTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotGetNextTaskIfNotLoggedIn() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetNextCountdown());
        }

        [TestMethod]
        public async AsyncTask TestCreateGetAndDeleteTask()
        {
            using (var con = new ServerConnection())
            {
                Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
                con.Connect(ServerUrl);
                Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
                var loginResponse = await con.LogIn(new UserAuth {Username = Username, Password = Password});
                Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");

                var sentTask = TestTask.DeepClone();
                var newTaskId = await con.CreateTask(sentTask);
                Assert.IsNotNull(newTaskId, "CreateTask did not return a task id");
                Assert.AreEqual(newTaskId, sentTask.TaskId, "Task id was not updated in input Task instance");
                Assert.AreNotEqual(TestTask.TaskId, newTaskId, "returned task id matches sent task id");
                Assert.AreNotEqual(TestTask.TaskId, newTaskId,
                    "task id of sent task matches sent task id after CreateTask call");


                var usersTasks = await con.GetTasksForUser();
                var responseTask = usersTasks.First(t => t.TaskId == newTaskId);
                Assert.IsNotNull(responseTask, "Could not find newly created task");
                Assert.IsNotNull(responseTask.Subtasks, "Response task subtask list is null");
                //the creation and modification timestamps are not valid on the sent task, so we force them to be equal
                sentTask.CreationDate = responseTask.CreationDate;
                sentTask.LastModifiedTime = responseTask.LastModifiedTime;
                Assert.AreEqual(sentTask, responseTask, "Sent task and response task are not equal");


                var deleteResult = await con.DeleteTask(sentTask);
                Assert.AreEqual(DeleteTaskResult.Success, deleteResult, "Delete task did not return success");


                usersTasks = await con.GetTasksForUser();
                Assert.IsFalse(usersTasks.Any(t => t.TaskId == newTaskId), "deleted task was still returned from server");

                deleteResult = await con.DeleteTask(sentTask);
                Assert.AreEqual(DeleteTaskResult.Failure, deleteResult, "Repeated delete task did not return Failure");
            }
        }

        [TestMethod]
        public async AsyncTask TestGetNextTask()
        {
            using (var con = new ServerConnection())
            {
                Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
                con.Connect(ServerUrl);
                Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
                var loginResponse = await con.LogIn(new UserAuth {Username = Username, Password = Password});
                Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");

                var responseTask = await con.GetNextCountdown();
                Assert.IsNotNull(responseTask, "GetNextCountdown returned null");
            }
        }

        [TestMethod]
        public async AsyncTask AllActiveTasksAreMarkedAsIncomplete()
        {
            using (var con = new ServerConnection())
            {
                Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
                con.Connect(ServerUrl);
                Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
                var loginResponse = await con.LogIn(new UserAuth {Username = Username, Password = Password});
                Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");

                var usersTasks = await con.GetTasksForUser();
                Assert.IsNotNull(usersTasks, "get tasks returned null");

                foreach (var t in usersTasks)
                {
                    Assert.IsFalse(t.IsCompleted, "task returned by get tasks was marked as completed");
                }
            }
        }

        [TestMethod]
        public async AsyncTask AllInactiveTasksAreMarkedAsIncomplete() {
            using (var con = new ServerConnection()) {
                Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
                con.Connect(ServerUrl);
                Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
                var loginResponse = await con.LogIn(new UserAuth { Username = Username, Password = Password });
                Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");

                var usersTasks = await con.GetInactiveTasksForUser();
                Assert.IsNotNull(usersTasks, "get inactive tasks returned null");

                foreach (var t in usersTasks) {
                    Assert.IsTrue(t.IsCompleted, "task returned by get inactive tasks was not marked as completed");
                }
            }
        }



        /// <summary>
        /// Utility function to assert that an async function throws the given exception.  
        /// This is needed as a workaround for a flaw in this testing framework's Assert.Throws implementation
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="allowDerivedTypes"></param>
        /// <returns></returns>
        public static async AsyncTask AssertThrowsAsync<TException>(Func<AsyncTask> action, bool allowDerivedTypes = true) {
            try {
                await action();
                Assert.Fail("Delegate did not throw expected exception " + typeof(TException).Name + ".");
            } catch (Exception ex) {
                if (allowDerivedTypes && !(ex is TException))
                    Assert.Fail("Delegate threw exception of type " + ex.GetType().Name + ", but " + typeof(TException).Name + " or a derived type was expected.");
                if (!allowDerivedTypes && ex.GetType() != typeof(TException))
                    Assert.Fail("Delegate threw exception of type " + ex.GetType().Name + ", but " + typeof(TException).Name + " was expected.");
            }
        }
    }
}
