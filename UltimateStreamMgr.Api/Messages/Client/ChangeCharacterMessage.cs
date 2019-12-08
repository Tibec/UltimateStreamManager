using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages.Client
{
    public class ChangeCharacterMessage : BaseMessage
    {
        [JsonProperty("player_id")]
        public int PlayerId { get; set; }
        [JsonProperty("character_name")]
        public string CharacterName { get; set; }
    }
}
