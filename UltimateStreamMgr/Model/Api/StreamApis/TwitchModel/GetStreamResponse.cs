using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.StreamApis.TwitchModel
{
    public class GetStreamResponse
    {
        [JsonProperty("user_name")]
        public string Username { get; set; }
     
        [JsonProperty("game_name")]
        public string GameName { get; set; }
        
        [JsonProperty("viewer_count")]
        public int ViewerCounter { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }

    }
}
