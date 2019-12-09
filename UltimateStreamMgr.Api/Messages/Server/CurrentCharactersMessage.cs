using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages.Server
{
    public class CurrentCharactersMessage : BaseMessage
    {
        [JsonProperty("player1_character_icon_path")]
        public string Player1CharacterIconPath { get; set; }

        [JsonProperty("player2_character_icon_path")]
        public string Player2CharacterIconPath { get; set; }
    }
}
