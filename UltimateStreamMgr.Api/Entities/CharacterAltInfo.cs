using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Entities
{
    public class CharacterAltInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon_path")]
        public string IconPath { get; set; }
    }
}
