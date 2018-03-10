using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api.BracketApis;

namespace UltimateStreamMgr.ViewModel
{
    class SmashggSettingsViewModel : ViewModelBase
    {
        public SmashggSettingsViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));

            if (Configuration.Instance.Bracket is SmashGgSettings)
                TournamentName = (Configuration.Instance.Bracket as SmashGgSettings).TournamentName;
        }

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set { Set("Enabled", ref _enabled, value); }
        }

        private string _tournamentName;
        public string TournamentName
        {
            get { return _tournamentName; }
            set { Set("TournamentName", ref _tournamentName, value); }
        }

        private void Save(NotificationMessage msg)
        {
            if (msg.Notification == "SaveBracket" && Enabled )
            {
                Configuration.Instance.Bracket = new SmashGgSettings { TournamentName = TournamentName };
            }
        }
    }
}