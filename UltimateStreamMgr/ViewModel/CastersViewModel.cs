using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using UltimateStreamMgr.Model;
using NLog;

namespace UltimateStreamMgr.ViewModel
{
    class CastersViewModel : ViewModelBase
    {

        private Logger log;

        public CastersViewModel()
        {
            log = LogManager.GetCurrentClassLogger();

            PlayerList = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers());
            PlayerDatabase.DatabaseContentModified += RefreshPlayers;

            UpdateCommand = new RelayCommand(() => Update());
            SwapCommand = new RelayCommand(() => Swap());
        }

        private void RefreshPlayers()
        {
            log.Info("Player database content changed. Updating autocompletion list");
            Dispatcher.CurrentDispatcher.Invoke(() => PlayerList = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers()));
        }

        private Caster _caster1 = new Caster();
        public Caster Caster1
        {
            get { return _caster1; }
            set { Set("Caster1", ref _caster1, value); }
        }

        private Caster _caster2 = new Caster();
        public Caster Caster2
        {
            get { return _caster2; }
            set { Set("Caster2", ref _caster2, value); }
        }

        private ObservableCollection<Player> _playerList;
        public ObservableCollection<Player> PlayerList
        {
            get { return _playerList; }
            set { Set("PlayerList", ref _playerList, value); }
        }

        public RelayCommand UpdateCommand { get; set; }
        private void Update()
        {
            Output.Data.Caster1 = Caster1;
            Output.Data.Caster2 = Caster2;
        }

        public RelayCommand SwapCommand { get; set; }
        private void Swap()
        {
            Caster temp = Caster1;
            Caster1 = Caster2;
            Caster2 = temp;
        }


    }
}
