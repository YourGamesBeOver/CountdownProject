using System;
using Windows.UI;
using Newtonsoft.Json;

namespace Countdown.Networking.Serialization {
    public class Task {
        [JsonProperty("id")]
        public int TaskId { get; set; }

        [JsonIgnore]
        public int OwnerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("duedate")]
        public DateTime DueDate { get; set; }

        [JsonProperty("datecreated")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("lastmodified")]
        public DateTime LastModifiedTime { get; set; }

        [JsonProperty("backgroundhex")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color BackgroundColor { get; set; }

        [JsonProperty("foregroundhex")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color ForegroundColor { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("completed")]
        public bool IsCompleted { get; set; }
    }
}
