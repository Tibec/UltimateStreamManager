using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Media;

namespace UltimateStreamMgr.Model.Api.StreamApis
{
    public class Twitch : StreamApi
    {
        private TwitchSettings _settings;

        public Twitch(StreamSettings settings)
            : base(settings)
        {
            ApiName = "Twitch";
            // Logo = 
            // ControlColor = new Color()
            if (settings is TwitchSettings)
            {
                _settings = settings as TwitchSettings;
                if (!string.IsNullOrEmpty(_settings.OAuthToken))
                    SetAuthorizationKey(_settings.OAuthToken);
            }

            _headers.Add("Client-ID", "ca48u330owoiyhvnmmwl7rkai3i6vqs");
            _headers.Add("Accept", "application/vnd.twitchtv.v5+json");

            _baseUrl = "https://api.twitch.tv/kraken/";

            _allowedActions[StreamCapabilities.UpdateChannel] = true;
            _allowedActions[StreamCapabilities.DisplayChat] = true;
        }

        public void SetAuthorizationKey(string key)
        {
            _headers["Authorization"] =  "OAuth " + key;
        }

        static public string GetAuthentificationUrl()
        {
            string clientId = "ca48u330owoiyhvnmmwl7rkai3i6vqs";
            string redirectUri = "http://51.254.99.66";
            string rights = "chat_login channel_read channel_editor";

            return "https://api.twitch.tv/kraken/oauth2/authorize" +
                   "?client_id=" + clientId +
                   "&redirect_uri=" + redirectUri +
                   "&response_type=token" +
                   "&scope="+rights;
        }

        public bool ChannelExist(string name)
        {
            string response = Request("users?login="+name);
            if (string.IsNullOrEmpty(response))
                return false;
            dynamic a = JsonConvert.DeserializeObject(response);

            return !(a._total == 0);
        }

        private int GetChannelId(string channelName)
        {
            string response = Request("users?login=" + channelName);
            if (string.IsNullOrEmpty(response))
                return -1;
            dynamic a = JsonConvert.DeserializeObject(response);

            return a.users[0]._id;
        }

        public override bool IsCorrectlySetup()
        {
            // Check if : 
            //  - Use a valid channel
            // optional : 
            //  - has a valid Authentification token 
            return ChannelExist(_settings.ChannelName);
        }

        public override ChannelInfo GetChannelInfo()
        {
            ChannelInfo info = new ChannelInfo();
            if(_settings.ChannelId == -1)
            {
                _settings.ChannelId = GetChannelId(_settings.ChannelName);
            }
            string rawStreamData = Request("streams/" + _settings.ChannelId);
            string rawChannelData = Request("channels/" + _settings.ChannelId);
            if (string.IsNullOrEmpty(rawStreamData) || string.IsNullOrEmpty(rawChannelData))
                return info;

            dynamic streamData = JsonConvert.DeserializeObject(rawStreamData);
            dynamic channelData = JsonConvert.DeserializeObject(rawChannelData);
            info.Game = new Game { Name = channelData.game };
            info.Name = channelData.name;
            info.Title = channelData.status;
            info.Status = streamData.stream == null ? ChannelStatus.Offline : ChannelStatus.Online;
            if(info.Status == ChannelStatus.Online)
                info.Viewers = streamData.stream.viewers;
            return info;
        }

        public override string GetChatUrl()
        {
            return "https://www.twitch.tv/" + _settings.ChannelName + "/chat";
        }

        public override void UpdateChannelInfo(string title, Game game)
        {
            if (_settings.ChannelId == -1)
            {
                _settings.ChannelId = GetChannelId(_settings.ChannelName);
            }
            string uri = "";
            if (string.IsNullOrEmpty(title) && game == null)
                return;
            else if (game == null && !string.IsNullOrEmpty(title))
                uri = "?channel[status]=" + Uri.EscapeDataString(title);
            else if (game == null && !string.IsNullOrEmpty(title))
                uri = "?channel[game]=" + Uri.EscapeDataString(game.Name);
            else // both ain't null
                uri = "?channel[status]=" + Uri.EscapeDataString(title) + "&channel[game]=" + Uri.EscapeDataString(game.Name);
            Request("channels/" + _settings.ChannelId + uri, HttpMethod.Put);
        }
    }

    public class TwitchSettings : StreamSettings
    {
        public TwitchSettings()
        {
            Api = typeof(Twitch);
        }

        public string ChannelName { get; set; }
        public int ChannelId { get; set; } = -1;
        public string OAuthToken { get; set; }
    }
}
