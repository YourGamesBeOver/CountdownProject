using Countdown.Networking.Serialization;
using Newtonsoft.Json;

namespace Countdown.Networking.Parameters
{
    public struct CreateTaskParams
    {
        [JsonProperty("task")]
        public Task Task;
    }
}