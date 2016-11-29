using Newtonsoft.Json;
using Task = Countdown.Networking.Serialization.Task;

namespace Countdown.Networking.Parameters {
    internal struct TaskAndIdParams
    {
        [JsonProperty("task")]
        public Task Task;
        [JsonProperty("parentid")]
        public int TaskId;
    }
}
