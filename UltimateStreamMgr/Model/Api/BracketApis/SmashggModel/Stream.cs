using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Stream
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("streamName")] public string Name { get; set; }

    }
}