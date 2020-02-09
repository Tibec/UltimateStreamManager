using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.Model
{
    public class StreamChannelData : INotifyPropertyChanged
    {
        private Timer _refreshChannelInfo;
        private StreamApi _apiLink;
        private Logger Log = LogManager.GetCurrentClassLogger();

        private StreamChannelData()
        {
            Initialize();
        }

        public void Reset()
        {
            IsInitialized = false;
            Initialize();
        }

        private void Initialize()
        {
            _refreshChannelInfo = new Timer(5000);
            _refreshChannelInfo.Elapsed += RefreshChannelInfo;
            _refreshChannelInfo.AutoReset = false;

            InitializeApiLink();
            Configuration.Instance.StreamSettingsChanged += InitializeApiLink;
        }

        private void InitializeApiLink()
        {
            try
            {
                _refreshChannelInfo.Stop();
                _apiLink = Activator.CreateInstance(Configuration.Instance.Stream.Api, Configuration.Instance.Stream) as StreamApi;
                ApiInfo = _apiLink.ApiInfo;
                IsConfigured = _apiLink.IsCorrectlySetup();
                IsInitialized = !IsConfigured;
                _refreshChannelInfo.Start();
                Log.Trace("API Link succesfully created");
            }
            catch 
            {
                IsConfigured = false;
                IsInitialized = true;
                Log.Warn("API Link not created. Probably wrong configuration");
            }
        }

        private void RefreshChannelInfo(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_apiLink != null)
                {
                    ChannelInfo = _apiLink.GetChannelInfo();
                    ViewersEvolution.Add(ChannelInfo.Viewers);
                    if (ChannelInfo.Viewers > ViewersPeak)
                        ViewersPeak = ChannelInfo.Viewers;
                }
                if (!IsInitialized)
                {
                    IsInitialized = true;
                    Log.Info("Module succesfully initialized");
                }
                _refreshChannelInfo.Start();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "An error occured when trying to refresh channel info : ");
                IsConfigured = false;
            }
        }

        public void UpdateChannelInfo(string name, Game game)
        {
            try
            {
                _apiLink.UpdateChannelInfo(name, game);
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("401"))
                {
                    // Display not enough right
                }
            }
        }

        private ChannelInfo _channelInfo;
        public ChannelInfo ChannelInfo
        {
            get { return _channelInfo; }
            set { Set("ChannelInfo", ref _channelInfo, value); }
        }

        private StreamApiInfo _apiInfo;
        public StreamApiInfo ApiInfo
        {
            get { return _apiInfo; }
            set { Set("ApiInfo", ref _apiInfo, value); }
        }

        private int _viewersPeak;
        public int ViewersPeak
        {
            get { return _viewersPeak; }
            set { Set("ViewersPeak", ref _viewersPeak, value); }
        }

        private List<int> _viewersEvolution = new List<int>();
        public List<int> ViewersEvolution
        {
            get { return _viewersEvolution; }
            set { Set("ViewersEvolution", ref _viewersEvolution, value); }
        }

        private string _chatUrl;
        public string ChatUrl
        {
            get { return _chatUrl; }
            set { Set("ChatUrl", ref _chatUrl, value); }
        }


        private bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
            set { Set("IsInitialized", ref isInitialized, value); }
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

        private static StreamChannelData instance;
        public static StreamChannelData Instance
        {
            get
            {
                if (instance == null)
                    instance = new StreamChannelData();

                return instance;
            }
        }
    }
}
