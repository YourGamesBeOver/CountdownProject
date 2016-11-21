using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
                var status = JsonConvert.DeserializeObject<LoginResponse>(await content.ReadAsStringAsync());
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
                return !response.Status ? null : response.Tasks;
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
                return !response.Status ? null : response.Tasks;
            }
        }


        private void SetUpClient(string url)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(url) };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
        }

        #region exceptions
        internal class ConnectionException : Exception
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
