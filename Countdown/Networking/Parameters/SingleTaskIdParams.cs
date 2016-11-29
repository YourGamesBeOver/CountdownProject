using Newtonsoft.Json;

namespace Countdown.Networking.Parameters {
    public class SingleTaskIdParams {
        [JsonProperty("taskid")]
        public int TaskId { get; set; }
    }
}
