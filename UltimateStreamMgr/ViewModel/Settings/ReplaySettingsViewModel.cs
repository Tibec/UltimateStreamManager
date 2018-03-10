using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.ViewModel
{
    class ReplaySettingsViewModel : ViewModelBase
    {
        public ReplaySettingsViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));

            ReplayEnabled = Configuration.Instance.Replay.Enabled;
            InputFolder = Configuration.Instance.Replay.InputDirectory;
            OutputFolder = Configuration.Instance.Replay.OutputDirectory;
        }


        private void Save(NotificationMessage m)
        {
            if (m.Notification == "Save")
            {
                Configuration.Instance.Replay.Enabled = ReplayEnabled;
                Configuration.Instance.Replay.InputDirectory = InputFolder;
                Configuration.Instance.Replay.OutputDirectory = OutputFolder;
            }
        }

        private bool _replayEnabled;
        public bool ReplayEnabled
        {
            get { return _replayEnabled; }
            set { Set("ReplayEnabled", ref _replayEnabled, value); }
        }

        private string _inputFolder;
        public string InputFolder
        {
            get { return _inputFolder; }
            set { Set("ReplayEnabled", ref _inputFolder, value); }
        }

        private string _outputFolder;
        public string OutputFolder
        {
            get { return _outputFolder; }
            set { Set("OutputFolder", ref _outputFolder, value); }
        }

    }
}
