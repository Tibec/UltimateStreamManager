using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.StreamApis.TwitchModel
{
    internal class UpdateChannelParameters
    {

        [JsonProperty("game_id", NullValueHandling = NullValueHandling.Ignore)]
        public string GameId { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("broadcaster_language", NullValueHandling = NullValueHandling.Ignore)]
        public string BroadCasterLanguage { get; set; }
        
        [JsonProperty("delay", NullValueHandling = NullValueHandling.Ignore)] 
        public int? Delay { get; set; }
    }
}
