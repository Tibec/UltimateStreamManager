using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api.StreamApis;

namespace UltimateStreamMgr.ViewModel
{
    class TwitchSettingsViewModel : ViewModelBase
    {
        IDialogCoordinator _dialogCoordinator = new DialogCoordinator();

        public TwitchSettingsViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));
            if (Configuration.Instance.Stream is TwitchSettings)
            {
                TwitchSettings s = Configuration.Instance.Stream as TwitchSettings;
                ChannelName = s.ChannelName;
                OAuthToken = s.OAuthToken;
            }

            CheckChannelCommand = new RelayCommand(() => CheckChannel());
            GetTokenCommand = new RelayCommand(() => GetToken());
        }
        ~TwitchSettingsViewModel()
        {
            Messenger.Default.Unregister(this);
        }
        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set { Set(ref _enabled, value); }
        }

        private bool _checkButtonEnabled;
        public bool CheckButtonEnabled
        {
            get { return _checkButtonEnabled; }
            set { Set(ref _checkButtonEnabled, value); }
        }

        private string _channelName;
        public string ChannelName
        {
            get { return _channelName; }
            set { Set(ref _channelName, value); }
        }

        private string _oauthToken;
        public string OAuthToken
        {
            get { return _oauthToken; }
            set 
            { 
                Set(ref _oauthToken, value); 
                CheckButtonEnabled = !string.IsNullOrEmpty(OAuthToken); 
            }
        }

        public RelayCommand CheckChannelCommand { get; set; }
        private void CheckChannel()
        {
            bool result = (new Twitch(new TwitchSettings { OAuthToken = OAuthToken })).ChannelExist(ChannelName);
            if (result)
                _dialogCoordinator.ShowMessageAsync(this, "Succès", "La chaine indiqué est valide !");
            else
                _dialogCoordinator.ShowMessageAsync(this, "Erreur", "Impossible de trouver la chaine indiquée !");
        }

        public RelayCommand GetTokenCommand { get; set; }
        private void GetToken()
        {

        }

        private void Save(NotificationMessage msg)
        {
            if (msg.Notification == "SaveStream" && Enabled )
            {
                Configuration.Instance.Stream = new TwitchSettings {
                    ChannelName = ChannelName,
                    OAuthToken = OAuthToken };
            }
        }
    }
}