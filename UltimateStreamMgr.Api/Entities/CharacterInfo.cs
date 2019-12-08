using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Entities
{
    public class CharacterInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon_path")]
        public string IconPath { get; set; }

        [JsonProperty("alts")]
        public List<CharacterAltInfo> Alts { get; set; } = new List<CharacterAltInfo>();

    }
}
