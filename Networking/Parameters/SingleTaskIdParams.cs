using Newtonsoft.Json;

namespace Countdown.Networking.Parameters {
    public struct SingleTaskIdParams {
        [JsonProperty("taskid")]
        public int TaskId { get; set; }
    }
}
