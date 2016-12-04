using Newtonsoft.Json;

namespace Countdown.Networking.Parameters {
    internal struct CreateUserParams
    {
        [JsonProperty("user_info")]
        public UserAuth User;
    }
}
