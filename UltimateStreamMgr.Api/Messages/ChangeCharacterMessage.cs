using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages
{
    public class ChangeCharacterMessage : BaseMessage
    {
        [JsonProperty("player_id")]
        public int PlayerId { get; set; }
        [JsonProperty("character_name")]
        public string CharacterName { get; set; }
    }
}
