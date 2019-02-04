using NLog;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.Model
{
    class BracketData : INotifyPropertyChanged
    {
        private Timer _refreshInfo;
        private BracketApi _apiLink;
        private Logger Log = LogManager.GetCurrentClassLogger();

        private BracketData()
        {
            IsInitialized = false;
            Initialize();
        }

        private void Initialize()
        {
            _refreshInfo = new Timer(1000);
            _refreshInfo.Elapsed += RefreshPendingSets;
            _refreshInfo.AutoReset = false;
            _refreshInfo.Start();

            InitializeApiLink();
            Configuration.Instance.BracketSettingsChanged += InitializeApiLink;
        }

        private void RefreshPendingSets(object sender=null, ElapsedEventArgs e=null)
        {
            Players = new ObservableCollection<Player>(_apiLink.GetAllEntrants());
            PendingSets = new ObservableCollection<Set>(_apiLink.GetAllPendingSets());
            PendingSetsForStream = new ObservableCollection<Set>(_apiLink.GetAllPendingSets(true));
            Output.Data.Top8List = _apiLink.GetAvailablesTop8();

            if (!IsInitialized)
                IsInitialized = true;

            _refreshInfo.Start();
        }

        public void RefreshPlayer()
        {
            Players = new ObservableCollection<Player>(_apiLink.GetAllEntrants());
        }

        private void InitializeApiLink()
        {
            try
            {
                _refreshInfo.Stop();
                IsInitialized = false;
                _apiLink = Activator.CreateInstance(Configuration.Instance.Bracket.Api, Configuration.Instance.Bracket) as BracketApi;
                Log.Trace("API Link succesfully created");
                IsConfigured = true;
                _refreshInfo.Start();
            }
            catch
            {
                Log.Warn("API Link not created. Probably wrong configuration");
                IsConfigured = false;
            }
        }

        private Set _runningSet;
        public Set RunningSet
        {
            get { return _runningSet; }
            set { Set("RunningSet", ref _runningSet, value); }
        }

        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { Set("IsInitialized", ref _isInitialized, value); }
        }

        private ObservableCollection<Set> _pendingSets;
        public ObservableCollection<Set> PendingSets
        {
            get { return _pendingSets; }
            set { Set("PendingSets", ref _pendingSets, value); }
        }

        private ObservableCollection<Set> _pendingSetsForStream;
        public ObservableCollection<Set> PendingSetsForStream
        {
            get { return _pendingSetsForStream; }
            set { Set("PendingSetsForStream", ref _pendingSetsForStream, value); }
        }

        private ObservableCollection<Player> _players;
        public ObservableCollection<Player> Players
        {
            get { return _players; }
            set { Set("Players", ref _players, value); }
        }

        private bool _isReportingAvailable = false;
        public bool IsReportingAvailable
        {
            get { return _isReportingAvailable; }
            set { Set("ReportingAvailable", ref _isReportingAvailable, value); }
        }

        private bool isConfigured = false;
        public bool IsConfigured
        {
            get { return isConfigured; }
            set { Set("IsConfigured", ref isConfigured, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set<T>(string propertyName, ref T property, T value)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static BracketData instance;
        public static BracketData Instance
        {
            get
            {
                if (instance == null)
                    instance = new BracketData();

                return instance;
            }
        }
    }
}
