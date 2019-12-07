using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.StreamDeck
{
    [PluginActionId("com.ultimatestreammgr.incrementscore")]
    class IncrementScore : PluginBase
    {
        private int playerId = 1;

        public IncrementScore(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            USM.OnMessageReceived += OnMessage;
        }

        private void OnMessage(BaseMessage mess)
        {
            
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (!USM.IsConnected)
            {
                Connection.ShowAlert();
                return;
            }
            USM.Send(new IncrementPlayerScoreMessage {Player = 1});
            
        }

        public override void KeyReleased(KeyPayload payload)
        {

        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
        }

        public override void OnTick()
        {
        }

        public override void Dispose()
        {
        }
    }
}
