using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.StreamDeck
{
    [PluginActionId("com.ultimatestreammgr.programmable")]
    public class ProgrammableButton : PluginBase
    {
        private int playerId = 1;

        public Action OnClick;

        public InitialPayload ButtonInfo { get; }
        public SDConnection Connection { get; }

        public ProgrammableButton(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            ButtonInfo = payload;
            Connection = connection;
            CharacterSelector.AddKey(this);
        }

        private void OnMessage(BaseMessage mess)
        {
            
        }

        public override void KeyPressed(KeyPayload payload)
        {
            OnClick?.Invoke();
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

        public void ResetImage()
        {
            Connection.SetImageAsync("");
        }

        public void SetImage(string imagePath)
        {
            Connection.SetImageAsync(Image.FromFile(imagePath), true);
        }
    }
}
