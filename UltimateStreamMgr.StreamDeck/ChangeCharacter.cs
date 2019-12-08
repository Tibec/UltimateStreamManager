﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using UltimateStreamMgr.Api.Messages;
using UltimateStreamMgr.Api.Messages.Server;

namespace UltimateStreamMgr.StreamDeck
{
    [PluginActionId("com.ultimatestreammgr.changecharacter")]
    class ChangeCharacter : PluginBase
    {
        private int playerId = 1;

        public ChangeCharacter(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            USM.OnMessageReceived += OnMessage;
            // USM.Send();
        }

        private void OnMessage(BaseMessage mess)
        {
            if(mess is CurrentCharactersMessage newCharactersMessage)
            {
                if (playerId == 1)
                {
                    Connection.SetImageAsync(Image.FromFile(newCharactersMessage.Player1CharacterIconPath));
                }
                else if (playerId == 2)
                {
                    Connection.SetImageAsync(Image.FromFile(newCharactersMessage.Player2CharacterIconPath));
                }
            }

        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (!USM.IsConnected)
            {
                Connection.ShowAlert();
                return;
            }

            CharacterSelector.Initialize();
            Connection.SwitchProfileAsync("CharacterGrid");
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
