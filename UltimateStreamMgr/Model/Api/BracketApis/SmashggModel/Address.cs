using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Address
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("countryId")] public int CountryId { get; set; }
    }
}