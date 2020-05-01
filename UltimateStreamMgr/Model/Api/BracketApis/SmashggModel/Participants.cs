using System.Collections.Generic;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Participants
    {
        [JsonProperty("nodes")] public List<Participant> List { get; set; }
    }
}