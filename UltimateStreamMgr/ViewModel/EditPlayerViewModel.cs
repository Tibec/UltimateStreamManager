using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.ViewModel
{
    class EditPlayerViewModel : ViewModelBase
    {
        public EditPlayerViewModel()
        {
            SavePlayerCommand = new RelayCommand(() => SavePlayer());
            CancelCommand = new RelayCommand(() => Cancel());

            TeamList = PlayerDatabase.GetTeams();
            CountryList = PlayerDatabase.GetCountries();

            MessengerInstance.Register<Player>(this, ReceivePlayer);
        }

        private void ReceivePlayer(Player obj)
        {
            Player = obj;
            if(obj.Team != null)
                Player.Team = TeamList.Find(t => t.Id == obj.Team.Id);
        }

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set { Set("Player", ref _player, value); }
        }

        private List<Team> _teamList;
        public List<Team> TeamList
        {
            get { return _teamList; }
            set { Set("TeamList", ref _teamList, value); }
        }

        private string _newTeam;
        public string NewTeam
        {
            get { return _newTeam; }
            set { Set("NewTeam", ref _newTeam, value); }
        }

        private List<Country> _countryList;
        public List<Country> CountryList
        {
            get { return _countryList; }
            set { Set("CountryList", ref _countryList, value); }
        }

        public RelayCommand SavePlayerCommand { get; set; }
        private void SavePlayer()
        {

            if(Player.Team == null && !string.IsNullOrWhiteSpace(NewTeam))
            {
                Player.Team = new Team { Id = -1, ShortName = NewTeam };
            }

            PlayerDatabase.UpdateOrAddPlayer(Player);

            MessengerInstance.Send(new NotificationMessage("CloseEditionWindow"));
        }

        private void Clear()
        {
            Player = new Player();
        }

        public RelayCommand CancelCommand { get; set; }
        private void Cancel()
        {
            MessengerInstance.Send(new NotificationMessage("CloseEditionWindow"));
        }


    }
}
