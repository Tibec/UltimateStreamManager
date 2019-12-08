using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using UltimateStreamMgr.Api.Messages;
using UltimateStreamMgr.Api.Messages.Client;
using UltimateStreamMgr.Api.Messages.Server;

namespace UltimateStreamMgr.StreamDeck
{
    [PluginActionId("com.ultimatestreammgr.displayscore")]
    class DisplayScore : PluginBase
    {
        public DisplayScore(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            USM.OnMessageReceived += OnMessage;
            USM.Send(new GetCurrentScoreMessage());
        }

        private void OnMessage(BaseMessage mess)
        {
            if (mess is CurrentScoreMessage scoreMessage)
            {
                Connection.SetTitleAsync($"{scoreMessage.ScoreP1} - {scoreMessage.ScoreP2}");
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {

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
