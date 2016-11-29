using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Countdown.Networking.Parameters;
using Countdown.Networking.Results;
using Newtonsoft.Json;
using Countdown.Networking.Serialization;

namespace Countdown.Networking {
    public class ServerConnection : IDisposable
    {
        public bool IsConnected { get; private set; }
        private HttpClient _httpClient;
        private bool _loggedIn;

        public ServerConnection() {
            _loggedIn = false;
            IsConnected = false;
        }

        public void Connect(string url)
        {
            if (IsConnected)
            {
                throw new ConnectionException("Already connected; call Disconnect() first");
            }
            SetUpClient(url);
            IsConnected = true;
        }

        public async System.Threading.Tasks.Task<LoginResult> LogIn(UserAuth auth) {

            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (_loggedIn) {
                throw new ConnectionException("Already logged in");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{auth.Username}:{auth.Password}")));
            var response = await _httpClient.GetAsync(@"/login");
            if (!response.IsSuccessStatusCode) return LoginResult.Error;
            using (var content = response.Content) {
                var status = JsonConvert.DeserializeObject<StatusOnlyResponse>(await content.ReadAsStringAsync());
                if (status.Status) {
                    _loggedIn = true;
                    return LoginResult.Success;
                }
                _loggedIn = false;
                return LoginResult.BadParams;
            }
        }

        public async System.Threading.Tasks.Task<Task[]> GetTasksForUser()
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var rawresponse = await _httpClient.GetAsync(@"/get_active_tasks");
            if (!rawresponse.IsSuccessStatusCode) return null;
            using (var content = rawresponse.Content)
            {
                var response = JsonConvert.DeserializeObject<GetTasksResponse>(await content.ReadAsStringAsync());
                if(!response.Status) return null;
                if (response.Tasks != null && response.Tasks.Length > 0)
                {
                    foreach (var responseTask in response.Tasks)
                    {
                        responseTask.Subtasks = await GetSubTasksForTask(responseTask);
                        responseTask.IsCompleted = false;
                    }
                }
                return response.Tasks;
            }
        }

        public async System.Threading.Tasks.Task<Task[]> GetInactiveTasksForUser() {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var rawresponse = await _httpClient.GetAsync(@"/get_inactive_tasks");
            if (!rawresponse.IsSuccessStatusCode) return null;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<GetTasksResponse>(await content.ReadAsStringAsync());
                if (!response.Status) return null;
                if (response.Tasks != null && response.Tasks.Length > 0) {
                    foreach (var responseTask in response.Tasks) {
                        responseTask.Subtasks = await GetSubTasksForTask(responseTask);
                        responseTask.IsCompleted = true;
                    }
                }
                return response.Tasks;
            }
        }

        public async System.Threading.Tasks.Task<int?> CreateTask(Task newTask)
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }

            var param = new SingleTaskParams {Task = newTask};
            var rawresponse =
                await _httpClient.PostAsync(@"/create_task", new StringContent(JsonConvert.SerializeObject(param)));
            if (!rawresponse.IsSuccessStatusCode) return null;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<CreateTaskResponse>(await content.ReadAsStringAsync());
                if (!response.Status) return null;
                newTask.TaskId = response.TaskId;
                return response.TaskId;
            }
        }

        public async System.Threading.Tasks.Task<Task[]> GetSubTasksForTask(Task parentTask)
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var rawresponse = await _httpClient.GetAsync($"/get_subtasks/{parentTask.TaskId}");
            if (!rawresponse.IsSuccessStatusCode) return null;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<GetTasksResponse>(await content.ReadAsStringAsync());
                return !response.Status ? null : response.Tasks;
            }
        }

        public async System.Threading.Tasks.Task<Task> GetNextCountdown()
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var rawresponse = await _httpClient.GetAsync(@"/get_next_countdown");
            if (!rawresponse.IsSuccessStatusCode) return null;
            using (var content = rawresponse.Content)
            {
                var response = JsonConvert.DeserializeObject<SingleTaskResponse>(await content.ReadAsStringAsync());
                return !response.Status ? null : response.Task;
            }
        }

        public async System.Threading.Tasks.Task<ModifyTaskResult> DeleteTask(Task task)
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var param = new SingleTaskIdParams {TaskId = task.TaskId};
            var rawresponse =
                await _httpClient.PostAsync(@"/delete_task", new StringContent(JsonConvert.SerializeObject(param)));
            if (!rawresponse.IsSuccessStatusCode) return ModifyTaskResult.Error;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<StatusOnlyResponse>(await content.ReadAsStringAsync());
                return response.Status ? ModifyTaskResult.Success : ModifyTaskResult.Failure;
            }
        }

        public async System.Threading.Tasks.Task<ModifyTaskResult> MarkTaskAsCompleted(Task task)
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var param = new SingleTaskIdParams { TaskId = task.TaskId };
            var rawresponse =
                await _httpClient.PostAsync(@"/complete_task", new StringContent(JsonConvert.SerializeObject(param)));
            if (!rawresponse.IsSuccessStatusCode) return ModifyTaskResult.Error;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<StatusOnlyResponse>(await content.ReadAsStringAsync());
                if (!response.Status) return ModifyTaskResult.Failure;
                task.IsCompleted = true;
                return ModifyTaskResult.Success;
            }
        }

        public async System.Threading.Tasks.Task<ModifyTaskResult> MarkTaskAsNotCompleted(Task task) {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            var param = new SingleTaskIdParams { TaskId = task.TaskId };
            var rawresponse =
                await _httpClient.PostAsync(@"/unarchive_task", new StringContent(JsonConvert.SerializeObject(param)));
            if (!rawresponse.IsSuccessStatusCode) return ModifyTaskResult.Error;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<StatusOnlyResponse>(await content.ReadAsStringAsync());
                if (!response.Status) return ModifyTaskResult.Failure;
                task.IsCompleted = false;
                return ModifyTaskResult.Success;
            }
        }

        public async System.Threading.Tasks.Task<ModifyTaskResult> EditTask(Task task)
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }
            task.LastModifiedTime = DateTime.Now;
            var param = new SingleTaskParams {Task = task};
            var rawresponse =
                await _httpClient.PostAsync(@"/edit_task", new StringContent(JsonConvert.SerializeObject(param)));
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<StatusOnlyResponse>(await content.ReadAsStringAsync());
                if (!response.Status) return ModifyTaskResult.Failure;
                task.IsCompleted = false;
                return ModifyTaskResult.Success;
            }
        }

        public async System.Threading.Tasks.Task<int?> CreateSubTask(Task subtask, Task parentTask)
        {
            if (!IsConnected) {
                throw new ConnectionException("No connection was established");
            }

            if (!_loggedIn) {
                throw new ConnectionException("No user logged in");
            }

            var param = new TaskAndIdParams {Task = subtask, TaskId = parentTask.TaskId};
            var rawresponse =
                await _httpClient.PostAsync(@"/create_subtask", new StringContent(JsonConvert.SerializeObject(param)));
            if (!rawresponse.IsSuccessStatusCode) return null;
            using (var content = rawresponse.Content) {
                var response = JsonConvert.DeserializeObject<CreateTaskResponse>(await content.ReadAsStringAsync());
                if (!response.Status) return null;
                subtask.TaskId = response.TaskId;
                return response.TaskId;
            }
        }

        private void SetUpClient(string url)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(url) };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
        }

        #region exceptions
        public class ConnectionException : Exception
        {
            public ConnectionException(string message) : base(message) { }
        }
        #endregion exceptions

        #region IDisposable
        public void Dispose()
        {
            _httpClient.CancelPendingRequests();
            _httpClient.Dispose();
        }
        #endregion IDisposable
    }
}
