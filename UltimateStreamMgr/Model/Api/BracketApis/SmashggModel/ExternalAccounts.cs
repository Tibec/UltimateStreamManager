using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class ExternalAccounts
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("externalUsername")] public string Username  { get; set; }
    }

    public enum AccountType
    {
        Twitter,
        Twitch,
        Discord,
        Steam,
        Mixer
    }
}