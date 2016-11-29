using System;
using System.Collections.Generic;
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
        public string Tag { get; set; } = "None";

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("completed")]
        public bool IsCompleted { get; set; }

        [JsonIgnore]
        public Task[] Subtasks { get; set; } = {};

        [JsonIgnore]
        public TimeSpan RemainingTime { get; set; }

        public Task DeepClone()
        {
            return new Task
            {
                TaskId = TaskId,
                OwnerId = OwnerId,
                Name = Name,
                Description = Description,
                DueDate = DueDate,
                CreationDate = CreationDate,
                LastModifiedTime = LastModifiedTime,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Tag = Tag,
                Priority = Priority,
                IsCompleted = IsCompleted,
                Subtasks = Subtasks,
                RemainingTime = RemainingTime
            };
        }

        #region ReSharper generated Equals() code

        protected bool Equals(Task other)
        {
            return TaskId == other.TaskId && OwnerId == other.OwnerId && string.Equals(Name, other.Name) &&
                   string.Equals(Description, other.Description) && DueDate.Equals(other.DueDate) &&
                   CreationDate.Equals(other.CreationDate) && LastModifiedTime.Equals(other.LastModifiedTime) &&
                   BackgroundColor.Equals(other.BackgroundColor) && ForegroundColor.Equals(other.ForegroundColor) &&
                   string.Equals(Tag, other.Tag) && Priority == other.Priority && IsCompleted == other.IsCompleted;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Task) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TaskId;
                hashCode = (hashCode*397) ^ OwnerId;
                hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Description?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ DueDate.GetHashCode();
                hashCode = (hashCode*397) ^ CreationDate.GetHashCode();
                hashCode = (hashCode*397) ^ LastModifiedTime.GetHashCode();
                hashCode = (hashCode*397) ^ BackgroundColor.GetHashCode();
                hashCode = (hashCode*397) ^ ForegroundColor.GetHashCode();
                hashCode = (hashCode*397) ^ (Tag?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ Priority;
                hashCode = (hashCode*397) ^ IsCompleted.GetHashCode();
                return hashCode;
            }
        }

        #endregion ReSharper generated Equals() code
    }
}
