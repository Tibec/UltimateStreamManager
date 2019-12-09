using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages.Client
{
    public class IncrementPlayerScoreMessage : BaseMessage
    {
        [JsonProperty("player")]
        public int Player { get; set; }
    }
}
