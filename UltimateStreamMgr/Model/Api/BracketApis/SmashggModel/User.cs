using System.Collections.Generic;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class User
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("authorizations")] public List<ExternalAccounts> ExternalAccounts { get; set; }
        [JsonProperty("location")] public Address Location { get; set; }
    }
}