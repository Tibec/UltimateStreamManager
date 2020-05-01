using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api.BracketApis;

namespace UltimateStreamMgr.ViewModel
{
    class SmashggGQLSettingsViewModel : ViewModelBase
    {
        public SmashggGQLSettingsViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));

            if (Configuration.Instance.Bracket is SmashGgGQLSettings settings)
            {
                ApiKey = settings.ApiKey;
                TournamentName = settings.TournamentName;
            }
        }
        ~SmashggGQLSettingsViewModel()
        {
            Messenger.Default.Unregister(this);
        }

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => Set("Enabled", ref _enabled, value);
        }

        private string _tournamentName;
        public string TournamentName
        {
            get => _tournamentName;
            set => Set("TournamentName", ref _tournamentName, value);
        }

        private string _apiKey;
        public string ApiKey
        {
            get => _apiKey; 
            set => Set("ApiKey", ref _apiKey, value); 
        }

        private void Save(NotificationMessage msg)
        {
            if (msg.Notification == "SaveBracket" && Enabled )
            {
                Configuration.Instance.Bracket = new SmashGgGQLSettings { TournamentName = TournamentName, ApiKey  = ApiKey};
            }
        }
    }
}