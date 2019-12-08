using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Api.Messages
{
    public class DecrementPlayerScoreMessage : BaseMessage
    {
        [JsonProperty("player")]
        public int Player { get; set; }
    }
}
