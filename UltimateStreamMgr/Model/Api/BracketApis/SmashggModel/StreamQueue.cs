using System.Collections.Generic;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class StreamQueue
    {
        [JsonProperty("stream")] public Stream Stream { get; set; }
        [JsonProperty("sets")] public List<Set> Sets { get; set; }
    }
}