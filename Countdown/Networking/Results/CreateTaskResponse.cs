using Newtonsoft.Json;

namespace Countdown.Networking.Results
{
    public struct CreateTaskResponse
    {
        [JsonProperty("status")]
        public bool Status;

        [JsonProperty("taskid")]
        public int TaskId;
    }
}