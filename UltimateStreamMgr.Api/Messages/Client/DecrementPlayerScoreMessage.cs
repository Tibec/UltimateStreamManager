using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages.Client
{
    public class DecrementPlayerScoreMessage : BaseMessage
    {
        [JsonProperty("player")]
        public int Player { get; set; }
    }
}
