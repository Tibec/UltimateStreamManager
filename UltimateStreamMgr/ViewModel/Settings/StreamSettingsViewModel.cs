using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.ViewModel
{
    class StreamSettingsViewModel : ViewModelBase
    {
        public StreamSettingsViewModel()
        {
            MessengerInstance.Register<NotificationMessage>(this, (m) => Save(m));

            InitializeBracketApiList();
        }

        ~StreamSettingsViewModel()
        {
            Messenger.Default.Unregister(this);
        }

        private void InitializeBracketApiList()
        {
            Type[] apis = Assembly.GetExecutingAssembly().GetTypes().Where((t) => t.IsSubclassOf(typeof(StreamApi))).ToArray();
            ApiList = new ObservableCollection<string>();
            foreach (var type in apis)
            {
                StreamSettings s = new StreamSettings();
                StreamApi a = Activator.CreateInstance(type, s) as StreamApi;
                ApiList.Add(a.ApiName);
                if (type == Configuration.Instance.Stream?.GetType())
                    SelectedApi = ApiList.Count - 1;
            }
        }

        private ObservableCollection<string> _apiList;
        public ObservableCollection<string> ApiList
        {
            get { return _apiList; }
            set { Set("ApiList", ref _apiList, value); }
        }

        private int _selectedApi;
        public int SelectedApi
        {
            get { return _selectedApi; }
            set { Set("SelectedApi", ref _selectedApi, value); }
        }

        private string _selectedApiName;
        public string SelectedApiName
        {
            get { return _selectedApiName; }
            set { Set("SelectedApiName", ref _selectedApiName, value); }
        }


        private void Save(NotificationMessage msg)
        {
            if (msg.Notification == "Save")
            {
                MessengerInstance.Send(new NotificationMessage("SaveStream"));
            }
        }
    }
}
