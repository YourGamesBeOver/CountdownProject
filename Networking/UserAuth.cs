using Newtonsoft.Json;

namespace Countdown.Networking {
    public struct UserAuth
    {
        [JsonProperty("username")]
        public string Username;
        [JsonProperty("password")]
        public string Password;
    }
}
