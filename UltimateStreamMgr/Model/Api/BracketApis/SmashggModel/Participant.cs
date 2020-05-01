using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Participant
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("gamerTag")] public string GamerTag { get; set; }
        [JsonProperty("prefix")] public string Prefix { get; set; }
        [JsonProperty("user")] public User User { get; set; }
    }
}
