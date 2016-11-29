using Newtonsoft.Json;

namespace Countdown.Networking.Results {
    public struct StatusOnlyResponse
    {
        [JsonProperty("status")]
        public bool Status;
    }
}
