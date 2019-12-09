using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages.Server
{
    public class CurrentScoreMessage : BaseMessage
    {
        [JsonProperty("score_p1")]
        public int ScoreP1 { get; set; }
        [JsonProperty("score_p2")]
        public int ScoreP2 { get; set; }
    }
}
