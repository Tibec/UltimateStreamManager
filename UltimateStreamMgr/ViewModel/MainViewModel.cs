using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if(!string.IsNullOrEmpty(Configuration.Instance.Window.DockDisposition))
            {
                Messenger.Default.Send(Configuration.Instance.Window.DockDisposition);
            }
        }


        private List<ViewModelBase> _windows = null;
        public List<ViewModelBase> Windows
        {
            get { return _windows; }
            set { Set("Windows", ref _windows, value); }
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
