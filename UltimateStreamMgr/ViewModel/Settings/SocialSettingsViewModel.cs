using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;
using UltimateStreamMgr.Model.Api.SocialApis;

namespace UltimateStreamMgr.ViewModel
{
    class SocialSettingsViewModel : ViewModelBase
    {
        public SocialSettingsViewModel()
        {
            if(Configuration.Instance.Social is TwitterSettings)
            {
                TwitterSettings s = Configuration.Instance.Social as TwitterSettings;
                ConsumerKey = s.ConsumerKey;
                ConsumerSecret = s.ConsumerSecret;
                AccessKey = s.AccessToken;
                AccessSecret = s.AccessTokenSecret;
            }
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));
        }

        ~SocialSettingsViewModel()
        {
            Messenger.Default.Unregister(this);
        }


        private string _consumerKey;
        public string ConsumerKey
        {
            get { return _consumerKey; }
            set { Set("ConsumerKey", ref _consumerKey, value); }
        }

        private string _consumerSecret;
        public string ConsumerSecret
        {
            get { return _consumerSecret; }
            set { Set("ConsumerSecret", ref _consumerSecret, value); }
        }

        private string _accessKey;
        public string AccessKey
        {
            get { return _accessKey; }
            set { Set("AccessKey", ref _accessKey, value); }
        }

        private string _accessSecret;
        public string AccessSecret
        {
            get { return _accessSecret; }
            set { Set("AccessSecret", ref _accessSecret, value); }
        }


        private void Save(NotificationMessage msg)
        {
            if (msg.Notification == "Save")
            {
                Configuration.Instance.Social = new TwitterSettings {
                    AccessToken = AccessKey,
                    AccessTokenSecret = AccessSecret,
                    ConsumerKey = ConsumerKey,
                    ConsumerSecret = ConsumerSecret
                }; 
            }
        }
    }
}
