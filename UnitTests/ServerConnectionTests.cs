using System;
using System.Linq;
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
        public async AsyncTask TestLogIn()
        {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
            var response = await con.LogIn(new UserAuth {Username = Username, Password = Password});
            Assert.AreEqual(LoginResult.Success, response, "LogIn did not return LoginResult.Success");
        }

        [TestMethod]
        public async AsyncTask TestCreateGetAndDeleteTask()
        {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
            var loginResponse = await con.LogIn(new UserAuth { Username = Username, Password = Password });
            Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");

            var sentTask = TestTask.DeepClone();
            var newTaskId = await con.CreateTask(sentTask);
            Assert.IsNotNull(newTaskId, "CreateTask did not return a task id");
            Assert.AreEqual(newTaskId, sentTask.TaskId, "Task id was not updated in input Task instance");
            Assert.AreNotEqual(TestTask.TaskId, newTaskId, "returned task id matches sent task id");
            Assert.AreNotEqual(TestTask.TaskId, newTaskId, "task id of sent task matches sent task id after CreateTask call");


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

        [TestMethod]
        public async AsyncTask TestGetNextTask()
        {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
            var loginResponse = await con.LogIn(new UserAuth { Username = Username, Password = Password });
            Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");

            var responseTask = await con.GetNextCountdown();
            Assert.IsNotNull(responseTask, "GetNextCountdown returned null");
        }
    }
}
