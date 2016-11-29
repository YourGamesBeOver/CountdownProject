using Countdown.Networking.Serialization;
using Newtonsoft.Json;

namespace Countdown.Networking.Results {
    public class SingleTaskResponse {
        [JsonProperty("status")]
        public bool Status;

        [JsonProperty("task")]
        public Task Task;
    }
}