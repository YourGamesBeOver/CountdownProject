using Countdown.Networking.Serialization;
using Newtonsoft.Json;

namespace Countdown.Networking.Results
{
    public class GetTasksResponse
    {
        [JsonProperty("status")]
        public bool Status;

        [JsonProperty("tasks")]
        public Task[] Tasks;
    }
}