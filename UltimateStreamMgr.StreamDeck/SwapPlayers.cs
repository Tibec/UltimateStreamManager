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
    [PluginActionId("com.ultimatestreammgr.swapplayers")]
    class SwapPlayers : PluginBase
    {
        public SwapPlayers(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (!USM.IsConnected)
            {
                Connection.ShowAlert();
                return;
            }
            USM.Send(new SwapPlayerMessage());
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
