using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(() => Save());
        }

        public RelayCommand SaveCommand { get; set; }
        private void Save()
        {
            MessengerInstance.Send(new NotificationMessage("Save"));
            Configuration.Instance.Save("config.xml");
        }
    }
}
