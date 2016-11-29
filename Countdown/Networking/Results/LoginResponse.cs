using Newtonsoft.Json;

namespace Countdown.Networking.Results {
    public struct LoginResponse
    {
        [JsonProperty("status")]
        public bool Status;
    }
}
