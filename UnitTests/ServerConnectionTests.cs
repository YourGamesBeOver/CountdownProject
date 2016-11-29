using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Countdown.Networking;
using Countdown.Networking.Results;
using Countdown.Networking.Serialization;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AsyncTask = System.Threading.Tasks.Task;
using Task = Countdown.Networking.Serialization.Task;

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

        private static readonly Task TestSubTask = new Task {
            TaskId = -1,
            Name = "Test SubTask",
            Description = "Test SubTask Description",
            DueDate = DateTime.Now.Add(TimeSpan.FromDays(15)),
            CreationDate = DateTime.Now,
            LastModifiedTime = DateTime.Now,
            BackgroundColor = new Color { R = 0x23, G = 0x45, B = 0x67 },
            ForegroundColor = new Color { R = 0x89, G = 0xAB, B = 0xCD },
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
            Connect(con);
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
                Connect(con);
                await LogIn(con);
            }
        }

        [TestMethod]
        public async AsyncTask TestLoginWithBadCredentials()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
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
        public async AsyncTask CannotCreateSubTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.CreateSubTask(TestSubTask.DeepClone(), TestTask));
        }

        [TestMethod]
        public async AsyncTask CannotEditTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.EditTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotDeleteTasksIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.DeleteTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotSetTaskAsCompletedIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.MarkTaskAsCompleted(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotSetTaskAsNotCompletedIfNotConnected() {
            var con = new ServerConnection();
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.MarkTaskAsNotCompleted(TestTask.DeepClone()));
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
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetTasksForUser());
        }

        [TestMethod]
        public async AsyncTask CannotGetInactiveTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetInactiveTasksForUser());
        }

        [TestMethod]
        public async AsyncTask CannotCreateTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.CreateTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotCreateSubTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.CreateSubTask(TestSubTask.DeepClone(), TestTask));
        }

        [TestMethod]
        public async AsyncTask CannotEditTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.EditTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotDeleteTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.DeleteTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotSetTaskAsCompleteIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.MarkTaskAsCompleted(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotSetTaskAsNotCompleteIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.MarkTaskAsNotCompleted(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotGetSubTasksIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetSubTasksForTask(TestTask.DeepClone()));
        }

        [TestMethod]
        public async AsyncTask CannotGetNextTaskIfNotLoggedIn() {
            var con = new ServerConnection();
            Connect(con);
            await AssertThrowsAsync<ServerConnection.ConnectionException>(async () => await con.GetNextCountdown());
        }

        [TestMethod]
        public async AsyncTask TestCreateGetAndDeleteTask()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
                await LogIn(con);

                var sentTask = TestTask.DeepClone();
                var newTaskId = await CreateNewTask(con, sentTask);


                var usersTasks = await con.GetTasksForUser();
                var responseTask = usersTasks.First(t => t.TaskId == newTaskId);
                Assert.IsNotNull(responseTask, "Could not find newly created task");
                Assert.IsNotNull(responseTask.Subtasks, "Response task subtask list is null");
                //the creation and modification timestamps are not valid on the sent task, so we force them to be equal
                sentTask.CreationDate = responseTask.CreationDate;
                sentTask.LastModifiedTime = responseTask.LastModifiedTime;
                Assert.AreEqual(sentTask, responseTask, "Sent task and response task are not equal");


                var deleteResult = await con.DeleteTask(sentTask);
                Assert.AreEqual(ModifyTaskResult.Success, deleteResult, "Delete task did not return success");


                usersTasks = await con.GetTasksForUser();
                Assert.IsFalse(usersTasks.Any(t => t.TaskId == newTaskId), "deleted task was still returned from server");

                deleteResult = await con.DeleteTask(sentTask);
                Assert.AreEqual(ModifyTaskResult.Failure, deleteResult, "Repeated delete task did not return Failure");
            }
        }

        [TestMethod]
        public async AsyncTask TestSettingCompletion()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
                await LogIn(con);

                var sentTask = TestTask.DeepClone();
                var newTaskId = await CreateNewTask(con, sentTask);

                var activeTasks = await con.GetTasksForUser();
                var responseTask = activeTasks.First(t => t.TaskId == newTaskId);
                Assert.IsNotNull(responseTask, "Could not find newly created task");
                Assert.IsNotNull(responseTask.Subtasks, "Response task subtask list is null");
                //the creation and modification timestamps are not valid on the sent task, so we force them to be equal
                sentTask.CreationDate = responseTask.CreationDate;
                sentTask.LastModifiedTime = responseTask.LastModifiedTime;
                Assert.AreEqual(sentTask, responseTask, "Sent task and response task are not equal");

                Assert.IsFalse(sentTask.IsCompleted, "Sent task is marked as complete");
                Assert.IsFalse(responseTask.IsCompleted, "response task is marked as complete");

                var inactiveTasks = await con.GetInactiveTasksForUser();
                Assert.IsFalse(inactiveTasks.Any(t => t.TaskId == newTaskId), "Sent task came back in the inactive task list");

                //mark the task as complete

                var setCompleteResult = await con.MarkTaskAsCompleted(sentTask);
                Assert.AreEqual(ModifyTaskResult.Success, setCompleteResult, "Set task as completed did not return ModifyTaskResult.Success");
                Assert.IsTrue(sentTask.IsCompleted, "Original task did not have its isCompleted flag set after calling MarkTaskAsCompleted");

                inactiveTasks = await con.GetInactiveTasksForUser();
                Assert.IsTrue(inactiveTasks.Any(t => t.TaskId == newTaskId), "Sent task did not come back in the inactive task list");

                activeTasks = await con.GetTasksForUser();
                Assert.IsFalse(activeTasks.Any(t => t.TaskId == newTaskId), "Sent task came back in the active task list");

                //now we set the task back to not completed and verify it again
                var setIncompleteResult = await con.MarkTaskAsNotCompleted(sentTask);
                Assert.AreEqual(ModifyTaskResult.Success, setCompleteResult, "Set task as completed did not return ModifyTaskResult.Success");
                Assert.IsFalse(sentTask.IsCompleted, "Original task did not have its isCompleted flag reset after calling MarkTaskAsCompleted");

                inactiveTasks = await con.GetInactiveTasksForUser();
                Assert.IsFalse(inactiveTasks.Any(t => t.TaskId == newTaskId), "Sent task came back in the inactive task list");

                activeTasks = await con.GetTasksForUser();
                Assert.IsTrue(activeTasks.Any(t => t.TaskId == newTaskId), "Sent task did not come back in the active task list");

            }
        }

        [TestMethod]
        public async AsyncTask TestEditTask()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
                await LogIn(con);

                //create the task

                var sentTask = TestTask.DeepClone();
                var newTaskId = await CreateNewTask(con, sentTask);

                var activeTasks = await con.GetTasksForUser();
                var responseTask = activeTasks.First(t => t.TaskId == newTaskId);
                Assert.IsNotNull(responseTask, "Could not find newly created task");
                Assert.IsNotNull(responseTask.Subtasks, "Response task subtask list is null");
                //the creation and modification timestamps are not valid on the sent task, so we force them to be equal
                sentTask.CreationDate = responseTask.CreationDate;
                sentTask.LastModifiedTime = responseTask.LastModifiedTime;
                Assert.AreEqual(sentTask, responseTask, "Sent task and response task are not equal");

                //modify the task and verify again
                var modifiedTask = sentTask.DeepClone();
                modifiedTask.Name = "Changed name!";
                modifiedTask.Description = "Changed Description";

                var editResponse = await con.EditTask(modifiedTask);
                Assert.AreEqual(ModifyTaskResult.Success, editResponse, "Edit call did not respond with Success");

                //now we get the task from the server to compare how it changed
                activeTasks = await con.GetTasksForUser();
                responseTask = activeTasks.First(t => t.TaskId == newTaskId);
                Assert.IsNotNull(responseTask, "Could not find edited task");
                Assert.IsNotNull(responseTask.Subtasks, "Response task subtask list is null");
                //creation date should still be the same
                Assert.AreNotEqual(modifiedTask.LastModifiedTime, responseTask.LastModifiedTime, "Modified task and modified response task have the same last modified date");
                modifiedTask.LastModifiedTime = responseTask.LastModifiedTime;
                Assert.AreEqual(modifiedTask, responseTask, "Modified task and response task are not equal");
                
            }
        }

        [TestMethod]
        public async AsyncTask TestCreateSubTask()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
                await LogIn(con);

                var parentTask = TestTask.DeepClone();
                var newTaskId = await CreateNewTask(con, parentTask);


                var usersTasks = await con.GetTasksForUser();
                var responseTask = usersTasks.First(t => t.TaskId == newTaskId);
                Assert.IsTrue(responseTask.Subtasks.Length == 0, "Newly created task did not start off with no subtasks");

                var subTask = TestSubTask.DeepClone();
                var subTaskId = await con.CreateSubTask(subTask, parentTask);
                Assert.AreEqual(subTaskId, subTask.TaskId, "create subtask did not automatically set the input task's task id");

                usersTasks = await con.GetTasksForUser();
                responseTask = usersTasks.First(t => t.TaskId == newTaskId);

                Assert.AreEqual(1, responseTask.Subtasks.Length, "Response task does not have exactly one subtask");
                //the creation and modification timestamps are not valid on the sent task, so we force them to be equal
                var responseSubTask = responseTask.Subtasks[0];
                subTask.CreationDate = responseSubTask.CreationDate;
                subTask.LastModifiedTime = responseSubTask.LastModifiedTime;
                Assert.AreEqual(subTask, responseSubTask, "Sent subtask and response subtask are not equal");
            }
        }

        [TestMethod]
        public async AsyncTask TestGetNextTask()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
                await LogIn(con);

                var responseTask = await con.GetNextCountdown();
                Assert.IsNotNull(responseTask, "GetNextCountdown returned null");
            }
        }

        [TestMethod]
        public async AsyncTask AllActiveTasksAreMarkedAsIncomplete()
        {
            using (var con = new ServerConnection())
            {
                Connect(con);
                await LogIn(con);

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
                Connect(con);
                await LogIn(con);

                var usersTasks = await con.GetInactiveTasksForUser();
                Assert.IsNotNull(usersTasks, "get inactive tasks returned null");

                foreach (var t in usersTasks) {
                    Assert.IsTrue(t.IsCompleted, "task returned by get inactive tasks was not marked as completed");
                }
            }
        }

        private static async AsyncTask LogIn(ServerConnection con)
        {
            var loginResponse = await con.LogIn(new UserAuth {Username = Username, Password = Password});
            Assert.AreEqual(LoginResult.Success, loginResponse, "LogIn did not return LoginResult.Success");
        }

        private static void Connect(ServerConnection con) {
            Assert.IsFalse(con.IsConnected, "ServerConnection not initially disconnected");
            con.Connect(ServerUrl);
            Assert.IsTrue(con.IsConnected, "ServerConnection not not connected after calling Connect");
        }

        private static async Task<int?> CreateNewTask(ServerConnection con, Task sentTask) {
            var newTaskId = await con.CreateTask(sentTask);
            Assert.IsNotNull(newTaskId, "CreateTask did not return a task id");
            Assert.AreEqual(newTaskId, sentTask.TaskId, "Task id was not updated in input Task instance");
            Assert.AreNotEqual(TestTask.TaskId, newTaskId, "returned task id matches sent task id");
            Assert.AreNotEqual(TestTask.TaskId, newTaskId,
                "task id of sent task matches sent task id after CreateTask call");
            return newTaskId;
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
