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
    class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            SaveCommand = new RelayCommand(() => Save());
            CancelCommand = new RelayCommand(() => Cancel());
            MessengerInstance.Register<int>(this, OnTabChangeRequest);
        }

        private void OnTabChangeRequest(int obj)
        {
            ActiveTab = obj;
        }

        public RelayCommand SaveCommand { get; set; }
        private void Save()
        {
            MessengerInstance.Send(new NotificationMessage("Save"));
            Configuration.Instance.Save("config.xml");
            MessengerInstance.Send(new NotificationMessage("CloseSettings"));
        }

        public RelayCommand CancelCommand { get; set; }
        private void Cancel()
        {
            MessengerInstance.Send(new NotificationMessage("CloseSettings"));
        }

        private int _activeTab;
        public int ActiveTab
        {
            get { return _activeTab; }
            set { Set("ActiveTab", ref _activeTab, value); }
        }
    }
}
