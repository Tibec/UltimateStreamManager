using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UltimateStreamMgr.Api.Entities;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.Api.Messages
{
    public class CharacterListMessage : BaseMessage
    {
        [JsonProperty("characters")] public List<CharacterInfo> Characters { get; set; } = new List<CharacterInfo>();
    }
}
