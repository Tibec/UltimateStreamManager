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
    class OutputSettingsViewModel : ViewModelBase
    {
        public OutputSettingsViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));

            OutputFolder = Configuration.Instance.Output.Folder;
            SelectedFormat = Configuration.Instance.Output.OutputFormat;
            InputFolder = Configuration.Instance.Output.TemplateFolder;
        }

        private void Save(NotificationMessage m)
        {
            if (m.Notification == "Save")
            {
                Configuration.Instance.Output.Folder = OutputFolder;
                Configuration.Instance.Output.OutputFormat = SelectedFormat;
                Configuration.Instance.Output.TemplateFolder = InputFolder;
            }
        }

        private string _outputFolder;
        public string OutputFolder
        {
            get { return _outputFolder; }
            set { Set("OutputFolder", ref _outputFolder, value); }
        }

        private string _inputFolder;
        public string InputFolder
        {
            get { return _inputFolder; }
            set { Set("InputFolder", ref _inputFolder, value); }
        }

        private OutputFormat _selectedFormat;
        public OutputFormat SelectedFormat
        {
            get { return _selectedFormat; }
            set { Set("SelectedFormat", ref _selectedFormat, value); }
        }
    }
}
