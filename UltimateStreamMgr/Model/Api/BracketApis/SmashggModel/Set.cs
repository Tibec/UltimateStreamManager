using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class Set
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("fullRoundText")] public string RoundName { get; set; }
        [JsonProperty("stream")] public Stream Stream { get; set; }
        [JsonProperty("slots")] public List<SetPlayer> Players { get; set; }

    }

    public class SetPlayer
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("entrant")] public Entrant Entrant { get; set; }
    }
}
