using Newtonsoft.Json;

namespace Countdown.Networking.Parameters {
    internal struct SingleTaskIdParams {
        [JsonProperty("taskid")]
        public int TaskId { get; set; }
    }
}
