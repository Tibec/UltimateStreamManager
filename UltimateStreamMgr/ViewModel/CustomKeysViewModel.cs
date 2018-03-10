using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UltimateStreamMgr.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace UltimateStreamMgr.ViewModel
{
    class CustomKeysViewModel : ViewModelBase
    {
        public CustomKeysViewModel()
        {
            if(Configuration.Instance.Output.SavedKeys != null)
            {
                Keys = new ObservableCollection<CustomKey>(Configuration.Instance.Output.SavedKeys);
            }

            DeleteKeyCommand = new RelayCommand<object>((k) => DeleteKey(k));

            Keys.CollectionChanged += Update;
            Update(null, null);
        }

        private void Update(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                    foreach (ObservableObject item in e.OldItems)
                        item.PropertyChanged -= CustomKeyChanged;
                else if (e.Action == NotifyCollectionChangedAction.Add)
                    foreach (ObservableObject item in e.NewItems)
                        item.PropertyChanged += CustomKeyChanged;
            }
            else
                foreach (ObservableObject item in Keys)
                    item.PropertyChanged += CustomKeyChanged;

            Output.Data.Custom = new List<CustomKey>(Keys);
            Configuration.Instance.Output.SavedKeys = new List<CustomKey>(Output.Data.Custom);

        }

        private void CustomKeyChanged(object sender, PropertyChangedEventArgs e)
        {
            Output.Data.Custom = new List<CustomKey>(Keys);
            Configuration.Instance.Output.SavedKeys = new List<CustomKey>(Output.Data.Custom);
        }

        private ObservableCollection<CustomKey> _keys = new ObservableCollection<CustomKey>();
        public ObservableCollection<CustomKey> Keys
        {
            get { return _keys; }
            set { Set("Keys", ref _keys, value); }
        }


        public RelayCommand<object> DeleteKeyCommand { get; set; }
        private void DeleteKey(object key)
        {
            if(key is CustomKey)
                Keys.Remove(key as CustomKey);
        }

    }
}
