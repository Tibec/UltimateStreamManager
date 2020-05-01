using System.Collections.Generic;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Events
    {
        [JsonProperty("nodes")] public List<Event> List { get; set; }
    }
}