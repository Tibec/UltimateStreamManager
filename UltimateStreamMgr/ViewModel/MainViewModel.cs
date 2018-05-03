using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            ResetLayout = new RelayCommand(() => DoResetLayout());

            Windows = new ObservableCollection<DockWindowViewModel>();

            Windows.Add(new StreamApiIndicatorViewModel());
            Windows.Add(new PendingSetsViewModel());
            Windows.Add(new RunningSetViewModel());
            Windows.Add(new CustomKeysViewModel());
            Windows.Add(new CastersViewModel());
            /*
            if (!string.IsNullOrEmpty(Configuration.Instance.Window.DockDisposition))
            {
                Messenger.Default.Send(Configuration.Instance.Window.DockDisposition);
            }
            */
        }

        public RelayCommand ResetLayout { get; set; } 
        private void DoResetLayout()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UltimateStreamMgr.Resources.DefaultLayout.xml"))
            {
                string xml = new StreamReader(stream).ReadToEnd();
                Messenger.Default.Send(xml);
            }
        }
        private ObservableCollection<DockWindowViewModel> _windows;
        public ObservableCollection<DockWindowViewModel> Windows
        {
            get { return _windows; }
            set
            {
                Set("Windows", ref _windows, value);
            }
        }


        private string _dockContent;
        public string DockContent
        {
            get { return _dockContent; }
            set
            {
                Set("DockContent", ref _dockContent, value);
                OnDockContentChanged();
            }
        }

        private void OnDockContentChanged()
        {
            Configuration.Instance.Window.DockDisposition = DockContent;
            Configuration.Instance.Window.AppHeight = (int)System.Windows.Application.Current.MainWindow.ActualHeight;
            Configuration.Instance.Window.AppWidth = (int)System.Windows.Application.Current.MainWindow.Width;
            Configuration.Instance.Save("config.xml");
        }
    }

}
