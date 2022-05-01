using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.StreamApis.TwitchModel
{
    public class GetChannelResponse
    {
        [JsonProperty("game_name")]
        public string GameName { get; set; }
        
        [JsonProperty("broadcaster_name")]
        public string BroadcasterName { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
