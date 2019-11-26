using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;

namespace UltimateStreamMgr.ViewModel
{
    class PlayerDatabaseViewModel : BaseViewModel
    {
        public PlayerDatabaseViewModel()
        {
            EditPlayerCommand = new RelayCommand<Player>((p) => EditPlayer(p));
            DeletePlayerCommand = new RelayCommand<Player>((p) => DeletePlayer(p));
            AddPlayerCommand = new RelayCommand(() => AddPlayer());
            BracketSynchroCommand = new RelayCommand(() => BracketSynchro());
            DeleteTeamCommand = new RelayCommand<Team>((t) => DeleteTeam(t));

            UpdatePlayerList();

            PlayerDatabase.DatabaseContentModified += UpdatePlayerList;
        }

        private void UpdatePlayerList()
        {
            Players = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers());
            Teams = new ObservableCollection<Team>(PlayerDatabase.GetTeams());

            PlayersView = CollectionViewSource.GetDefaultView(Players);
            PlayersView.Filter = FilterPlayer;
        }

        private ObservableCollection<Player> _players;
        public ObservableCollection<Player> Players
        {
            get { return _players; }
            set { Set("Players", ref _players, value); }
        }

        private ICollectionView _playersView;
        public ICollectionView PlayersView
        {
            get { return _playersView; }
            set { Set("PlayersView", ref _playersView, value); }
        }

        private ObservableCollection<Team> _teams;
        public ObservableCollection<Team> Teams
        {
            get { return _teams; }
            set { Set("Teams", ref _teams, value); }
        }

        private string _searchPlayer;
        public string SearchPlayer
        {
            get { return _searchPlayer; }
            set { Set("SearchPlayer", ref _searchPlayer, value); PlayersView.Refresh(); }
        }

        public bool FilterPlayer(object p)
        {
            if (p as Player == null)
                return false;
            if (string.IsNullOrEmpty(SearchPlayer))
                return true;
            return (p as Player).Name.StartsWith(SearchPlayer, StringComparison.OrdinalIgnoreCase);
        }

        private Player _selectedPlayer;
        public Player SelectedPlayers
        {
            get { return _selectedPlayer; }
            set { Set("SelectedPlayers", ref _selectedPlayer, value); }
        }

        private bool _editPanelOpen;
        public bool EditPanelOpen
        {
            get { return _editPanelOpen; }
            set { Set("EditPanelOpen", ref _editPanelOpen, value); }
        }


        public RelayCommand<Player> EditPlayerCommand { get; set; }
        private void EditPlayer(Player p)
        {
            EditPanelOpen = true;
            MessengerInstance.Send(p);
        }


        public RelayCommand<Player> DeletePlayerCommand { get; set; }
        private void DeletePlayer(Player p)
        {
            PlayerDatabase.DeletePlayer(p);
        }


        public RelayCommand<Team> DeleteTeamCommand { get; set; }
        private void DeleteTeam(Team p)
        {
            PlayerDatabase.DeleteTeam(p);
        }

        public RelayCommand BracketSynchroCommand { get; set; }
        private void BracketSynchro()
        {

            Task.Run(()=>{
                BracketApi api = Activator.CreateInstance(Configuration.Instance.Bracket.Api, Configuration.Instance.Bracket) as BracketApi;
                List<Player> list = api.GetAllEntrants();
                foreach (Player player in list)
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        PlayerDatabase.UpdateOrAddPlayer(player);
                    });
                }
            });
        }

        public RelayCommand AddPlayerCommand { get; set; }
        private void AddPlayer()
        {
            EditPanelOpen = true;
            MessengerInstance.Send(new Player());
        }

    }
}
