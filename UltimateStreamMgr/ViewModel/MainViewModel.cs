using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using MahApps.Metro;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.StreamDeck;

namespace UltimateStreamMgr.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private StreamDeckLink streamDeckLink;

        public MainViewModel()
        {
            ResetLayout = new RelayCommand(DoResetLayout);
            CompactLayout = new RelayCommand(DoCompactLayout);

            Windows = new ObservableCollection<DockWindowViewModel>
            {
                new StreamApiIndicatorViewModel(),
                new PendingSetsViewModel(),
                new RunningSetViewModel(),
                new CustomKeysViewModel(),
                new CastersViewModel(),
                new SocialModuleViewModel()
            };

            streamDeckLink = new StreamDeckLink(Windows.First(v=>v is RunningSetViewModel) as RunningSetViewModel);

            EnableDarkTheme = Configuration.Instance.DarkThemeEnabled;
            /*
                if (!string.IsNullOrEmpty(Configuration.Instance.Window.DockDisposition))
                {
                    Messenger.Default.Send(Configuration.Instance.Window.DockDisposition);
                }
            */
        }

        public override void Cleanup()
        {
            base.Cleanup();
            streamDeckLink.Stop();
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
        public RelayCommand CompactLayout { get; set; }
        private void DoCompactLayout()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UltimateStreamMgr.Resources.CompactLayout.xml"))
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

        private bool _enableDarkTheme = false;

        public bool EnableDarkTheme
        {
            get => _enableDarkTheme;
            set
            {
                Set("DarkThemeEnabled", ref _enableDarkTheme, value);
                if (value)
                    ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");
                else
                    ThemeManager.ChangeAppTheme(Application.Current, "BaseLight");

                Configuration.Instance.DarkThemeEnabled = value;
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
            Configuration.Instance.Save();
        }
    }

}
