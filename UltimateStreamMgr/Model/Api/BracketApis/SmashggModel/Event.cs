using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Event
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("gamerTag")] public string GamerTag { get; set; }

    }
}