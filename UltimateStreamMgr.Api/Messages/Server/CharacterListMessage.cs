using System.Collections.Generic;
using Newtonsoft.Json;
using UltimateStreamMgr.Api.Entities;

namespace UltimateStreamMgr.Api.Messages.Server
{
    public class CharacterListMessage : BaseMessage
    {
        [JsonProperty("characters")]
        public List<CharacterInfo> Characters { get; set; } = new List<CharacterInfo>();
    }
}
