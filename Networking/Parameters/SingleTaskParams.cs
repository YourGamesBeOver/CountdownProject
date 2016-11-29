using Countdown.Networking.Serialization;
using Newtonsoft.Json;

namespace Countdown.Networking.Parameters
{
    public struct SingleTaskParams
    {
        [JsonProperty("task")]
        public Task Task;
    }
}