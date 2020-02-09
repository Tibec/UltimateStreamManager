using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.ViewModel
{
    class PendingSetsViewModel : DockWindowViewModel
    {
        public PendingSetsViewModel()
        {
            Log.Info("Initialize Module...");
            Title = "Pending Set";

            BracketInfo = BracketData.Instance;

            StartSetCommand = new RelayCommand<Set>(StartSet);
            ForceRefreshCommand = new RelayCommand(ForceRefresh);

            BracketInfo.PropertyChanged += UpdateReference;

            Log.Info("Module initialized");
        }

        private void UpdateReference(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RefreshInProgress")
            {
                ManualRefreshAllowed = !BracketInfo.RefreshInProgress;
            }
        }

        private bool manualRefreshAllowed = false;
        public bool ManualRefreshAllowed
        {
            get { return manualRefreshAllowed; }
            set { Set("ManualRefreshAllowed", ref manualRefreshAllowed, value); }
        }


        private BracketData _bracketInfo;
        public BracketData BracketInfo
        {
            get { return _bracketInfo; }
            set { Set("BracketInfo", ref _bracketInfo, value); }
        }

        public RelayCommand<Set> StartSetCommand { get; set; }
        private void StartSet(Set set)
        {
            Log.Info("Requesting to change actual for the following [{0}, {1} vs {2}]", set.RoundName, set.Opponent1.Name, set.Opponent2.Name);
            MessengerInstance.Send(set);
        }

        public RelayCommand ForceRefreshCommand { get; set; }
        private void ForceRefresh()
        {
            Task.Run(()=>BracketInfo.ForceRefreshPendingSets());
        }
    }
}
