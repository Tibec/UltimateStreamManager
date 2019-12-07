using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.Test
{
    [TestClass]
    public class TestSerialization
    {
        [TestMethod]
        public void TestSerialize()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var message = new IncrementPlayerScoreMessage {Player = 1};
            var json = JsonConvert.SerializeObject(message, settings);
            var messageReceived = JsonConvert.DeserializeObject<BaseMessage>(json, settings);
        }
    }
}
