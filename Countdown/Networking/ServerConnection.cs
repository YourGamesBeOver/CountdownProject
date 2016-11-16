using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Countdown.Networking.Results;
using Newtonsoft.Json;

namespace Countdown.Networking {
    internal class ServerConnection : IDisposable
    {
        #region static
        private static ServerConnection _instance;

        public static void Connect(string url)
        {
            if (_instance != null)
            {
                throw new ConnectionException("Already connected; call Disconnect() first");
            }
            _instance = new ServerConnection(url);
        }

        public static void Disconnect()
        {
            if (_instance == null) {
                throw new ConnectionException("No connection was established");
            }
            _instance.Dispose();
            _instance = null;
        }

        public static async Task<LoginResult> LogIn(UserAuth auth) {

            if (_instance == null) {
                throw new ConnectionException("No connection was established");
            }
            return await _instance.InternalLogIn(auth);
        }

        #endregion static

        #region non-static

        private HttpClient _httpClient;
        private bool _loggedIn;

        private ServerConnection(string url) {
            SetUpClient(url);
            _loggedIn = false;
        }

        private void SetUpClient(string url)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(url) };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
        }

        private async Task<LoginResult> InternalLogIn(UserAuth auth) {
            if (_loggedIn)
            {
                throw new ConnectionException("Already logged in");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{auth.Username}:{auth.Password}")));
            var response = await _httpClient.GetAsync(@"/login");
            if (!response.IsSuccessStatusCode) return LoginResult.Error;
            using (var content = response.Content)
            {
                var status = JsonConvert.DeserializeObject<LoginResponse>(await content.ReadAsStringAsync());
                if (status.Status)
                {
                    _loggedIn = true;
                    return LoginResult.Success;
                }
                _loggedIn = false;
                return LoginResult.BadParams;
            }
        }

        #endregion non-static

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
