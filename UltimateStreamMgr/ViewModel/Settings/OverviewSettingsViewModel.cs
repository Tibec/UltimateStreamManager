using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.ViewModel
{
    class OverviewSettingsViewModel : BaseViewModel
    {
        public OverviewSettingsViewModel()
        {
            Configuration.Instance.StreamSettingsChanged += UpdateSettings;
            Configuration.Instance.BracketSettingsChanged += UpdateSettings;

            UpdateSettings();

            GoToStreamSettings = new RelayCommand(() => DoOpenStreamSettings());
            GoToBracketSettings = new RelayCommand(() => DoOpenBracketSettings());
        }

        private void UpdateSettings()
        {
            Stream = Configuration.Instance.Stream;
            Bracket = Configuration.Instance.Bracket;
        }

        private BracketSettings _bracket;
        public BracketSettings Bracket
        {
            get { return _bracket; }
            set { Set("Bracket", ref _bracket, value); }
        }

        private StreamSettings _stream;
        public StreamSettings Stream
        {
            get { return _stream; }
            set { Set("Stream", ref _stream, value); }
        }

        private RelayCommand _goToStreamSettings;
        public RelayCommand GoToStreamSettings { get; set; }
        private void DoOpenStreamSettings()
        {
            MessengerInstance.Send(1);
        }

        private RelayCommand _goToBracketSettings;
        public RelayCommand GoToBracketSettings { get; set; }
        private void DoOpenBracketSettings()
        {
            MessengerInstance.Send(2);
        }
    }
}
