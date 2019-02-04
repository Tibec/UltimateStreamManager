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
    class PendingSetsViewModel : DockWindowViewModel
    {
        public PendingSetsViewModel()
        {
            Log.Info("Initialize Module...");
            Title = "Pending Set";

            BracketInfo = BracketData.Instance;

            StartSetCommand = new RelayCommand<Set>((s) => StartSet(s));
            Log.Info("Module initialized");
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
            MessengerInstance.Send(set);
        }
    }
}
