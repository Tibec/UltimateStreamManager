using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.ViewModel
{
    class PendingSetsViewModel : ViewModelBase
    {
        private List<Player> _players;
        private BracketApi _api;
        private Timer _refreshInfo;

        public PendingSetsViewModel()
        {
            Configuration.Instance.BracketSettingsChanged += RefreshApiLink;
            RefreshApiLink();

            _refreshInfo = new Timer(1000);
            _refreshInfo.Elapsed += RefreshPendingSets;
            _refreshInfo.AutoReset = false;
            _refreshInfo.Start();

            StartSetCommand = new RelayCommand<Set>((s) => StartSet(s));
        }

        private void RefreshPendingSets(object sender, ElapsedEventArgs e)
        {
            _players = _api.GetAllEntrants();
            PendingSets = _api.GetAllPendingSets();
            Output.Data.Top8List = _api.GetAvailablesTop8();
            _refreshInfo.Start();
        }

        private void RefreshApiLink()
        {
            try
            {
                _api = Activator.CreateInstance(Configuration.Instance.Bracket.Api, Configuration.Instance.Bracket) as BracketApi;
            }
            catch (Exception e)
            {
                // api not set
            }
        }

        private List<Set> _pendingSets;
        public List<Set> PendingSets
        {
            get { return _pendingSets; }
            set { Set("PendingSets", ref _pendingSets, value); }
        }

        public RelayCommand<Set> StartSetCommand { get; set; }
        private void StartSet(Set set)
        {
            MessengerInstance.Send(set);
        }
    }
}
