using Countdown.Networking.Serialization;
using Newtonsoft.Json;

namespace Countdown.Networking.Parameters
{
    internal struct SingleTaskParams
    {
        [JsonProperty("task")]
        public Task Task;
    }
}