using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages
{
    public class MessageResponse : BaseMessage
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
