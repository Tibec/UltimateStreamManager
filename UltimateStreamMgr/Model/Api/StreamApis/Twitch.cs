using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using UltimateStreamMgr.Model.Api.StreamApis.TwitchModel;

namespace UltimateStreamMgr.Model.Api.StreamApis
{
    public class Twitch : StreamApi
    {
        private TwitchSettings _settings;

        public Twitch(StreamSettings settings)
            : base(settings)
        {
            ApiName = "Twitch";
            ApiInfo = new StreamApiInfo()
            {
                isFontLogo = false,
                logo = "Twitch",
                color = new SolidColorBrush(Color.FromRgb(0x69, 0x3D, 0xA5))
            };

            if (settings is TwitchSettings)
            {
                _settings = settings as TwitchSettings;
                if (!string.IsNullOrEmpty(_settings.OAuthToken))
                    SetAuthorizationKey(_settings.OAuthToken);
            }

            _headers.Add("Client-ID", "ca48u330owoiyhvnmmwl7rkai3i6vqs");
            _headers.Add("Accept", "application/vnd.twitchtv.v5+json");

            _baseUrl = "https://api.twitch.tv/helix/";

            _allowedActions[StreamCapabilities.UpdateChannel] = true;
            _allowedActions[StreamCapabilities.DisplayChat] = true;
        }

        public void SetAuthorizationKey(string key)
        {
            _headers["Authorization"] =  "Bearer " + key;
        }

        static public string GetAuthentificationUrl()
        {
            string clientId = "ca48u330owoiyhvnmmwl7rkai3i6vqs";
            string redirectUri = "http://gtfo.noe";
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
            if (string.IsNullOrEmpty(response) || ResponseContainError(response))
                return false;
            dynamic a = JsonConvert.DeserializeObject(response);

            return a?.data?.Count == 1;
        }

        private int GetChannelId(string channelName)
        {
            string response = Request("users?login=" + channelName);
            if (string.IsNullOrEmpty(response) || ResponseContainError(response))
                return -1;
            dynamic a = JsonConvert.DeserializeObject(response);

            return a.data[0].id;
        }
        private string GetGameId(Game game)
        {
            string response = Request("games?name=" + game.Name);
            if (string.IsNullOrEmpty(response) || ResponseContainError(response))
                return "";
            dynamic a = JsonConvert.DeserializeObject(response);

            return a.data[0].id;
        }

        public override bool IsCorrectlySetup()
        {
            // Check if : 
            //  - Use a valid channel (ok)
            // optional : 
            //  - has a valid Authentification token (todo)
            return ChannelExist(_settings.ChannelName);
        }

        public override ChannelInfo GetChannelInfo()
        {
            ChannelInfo info = new ChannelInfo();
            if(_settings.ChannelId == -1)
            {
                _settings.ChannelId = GetChannelId(_settings.ChannelName);
            }
            string rawStreamData = Request("streams?user_id=" + _settings.ChannelId);
            string rawChannelData = Request("channels?broadcaster_id=" + _settings.ChannelId);
            if (string.IsNullOrEmpty(rawStreamData))
                return info;

            TwitchApiResponse<GetStreamResponse> stream = JsonConvert.DeserializeObject<TwitchApiResponse<GetStreamResponse>>(rawStreamData);
            TwitchApiResponse<GetChannelResponse> channel = JsonConvert.DeserializeObject<TwitchApiResponse<GetChannelResponse>>(rawChannelData);
            if (channel.Data?.Length == 0)
            {
                return info;
            }

            info.Game = new Game { Name = channel.Data[0].GameName };
            info.Name = channel.Data[0].BroadcasterName;
            info.Title = channel.Data[0].Title;
            info.Status = stream.Data.Length == 0 ? ChannelStatus.Offline : ChannelStatus.Online;
            if(info.Status == ChannelStatus.Online)
                info.Viewers = stream.Data[0].ViewerCounter;
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

            var parameters = new UpdateChannelParameters
            {
                Title = string.IsNullOrEmpty(title) ? null : title,
                GameId = GetGameId(game)
            };
            
            Request("channels/?broadcaster_id=" + _settings.ChannelId, new HttpMethod("PATCH"), JsonConvert.SerializeObject(parameters));
        }

        private bool ResponseContainError(string response)
        {
            JObject data = (JObject)JsonConvert.DeserializeObject(response);
            if (data.ContainsKey("error"))
                return true;
            return false;
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

        public override string ToString()
        {
            return "Twitch - " + (string.IsNullOrEmpty(ChannelName) ? "Aucune chaine" : ChannelName);
        }
    }
}
