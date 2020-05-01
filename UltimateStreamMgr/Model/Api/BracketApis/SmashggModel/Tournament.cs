using System.Collections.Generic;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Tournament
    {
        [JsonProperty("name")] public string Name { get; set; }
        // [JsonProperty("streamQueue")] public string StreamQueue { get; set; }
        [JsonProperty("participants")] public Participants Participants { get; set; }
        [JsonProperty("streamQueue")] public List<StreamQueue> StreamQueue { get; set; }
        [JsonProperty("events")] public Events Events { get; set; }

    }
}
