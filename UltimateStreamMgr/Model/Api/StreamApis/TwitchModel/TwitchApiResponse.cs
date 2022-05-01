using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model.Api.StreamApis.TwitchModel
{
    public class TwitchApiResponse<DataType>
    {
        [JsonProperty("data")]
        public DataType[] Data { get; set; }
    }
}
