using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UltimateStreamMgr.Model.Api
{
    abstract public class StreamApi : WebApi
    {
        protected Dictionary<StreamCapabilities, bool> _allowedActions = new Dictionary<StreamCapabilities, bool>();
        protected string _channel;

        public Color ControlColor { get; protected set; }
        public StreamLogo Logo { get; protected set; }

        public StreamApi(StreamSettings settings) : base()
        {
            _allowedActions[StreamCapabilities.UpdateChannel] = false;
            _allowedActions[StreamCapabilities.DisplayChat] = false;
        }

        public List<StreamCapabilities> GetSupportedActions()
        {
            List<StreamCapabilities> list = new List<StreamCapabilities>();
            foreach (var action in _allowedActions)
                if(action.Value)
                    list.Add(action.Key);
            return list;
        }

        public abstract void UpdateChannelInfo(string title, Game game);

        public void UpdateChannelInfo(string title)
        {
            UpdateChannelInfo(title, null);
        }

        public void UpdateChannelInfo(Game game)
        {
            UpdateChannelInfo(null, game);
        }

        public abstract ChannelInfo GetChannelInfo();

        public abstract string GetChatUrl();

        public abstract bool IsCorrectlySetup();



        // public virtual
    }

    public enum StreamCapabilities
    {
        UpdateChannel, 
        DisplayChat
    }

    public class StreamSettings
    {
        [XmlIgnore]
        public Type Api { get; set; } = null;
    }

    public struct StreamLogo
    {
        bool isFontLogo;
        string logo; // ressourcepath if !isFontLogo
    }
}
