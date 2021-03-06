﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using UltimateStreamMgr.Api.Messages;
using UltimateStreamMgr.Api.Messages.Client;

namespace UltimateStreamMgr.StreamDeck
{
    [PluginActionId("com.ultimatestreammgr.decrementscore")]
    class DecrementScore : PluginBase
    {
        private int playerId = 1;

        public DecrementScore(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings.ContainsKey("playerId"))
            {
                string playerIdString = payload.Settings["playerId"].ToString();
                playerId = int.Parse(playerIdString);
            }
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
            USM.Send(new DecrementPlayerScoreMessage { Player = playerId });

        }

        public override void KeyReleased(KeyPayload payload)
        {

        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            string playerIdString = payload.Settings["playerId"].ToString();
            playerId = int.Parse(playerIdString);
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
