using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using UltimateStreamMgr.Model.Api.SocialApis;

namespace UltimateStreamMgr.Model
{
    public class SocialData : INotifyPropertyChanged
    {
        private Timer _refreshTimelineTimer;
        private Twitter _apiLink;
        private Logger Log = LogManager.GetCurrentClassLogger();

        private SocialData()
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
            _refreshTimelineTimer = new Timer(10000);
            _refreshTimelineTimer.Elapsed += RefreshTimeline;
            _refreshTimelineTimer.AutoReset = false;

            InitializeApiLink();
            Configuration.Instance.StreamSettingsChanged += InitializeApiLink;
        }

        private void InitializeApiLink()
        {
            try
            {
                _refreshTimelineTimer.Stop();
                _apiLink = new Twitter(Configuration.Instance.Social as TwitterSettings);
                _refreshTimelineTimer.Start();
                Log.Trace("API Link succesfully created");
            }
            catch 
            {
                Log.Warn("API Link not created. Probably wrong configuration");
            }
            IsInitialized = true;
        }

        private void RefreshTimeline(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_apiLink != null && !string.IsNullOrWhiteSpace(_hashtag))
                {
                    List<SocialMessage> updatedTimeline = _apiLink.GetMessagesByHashtag("#" + _hashtag);
                    updatedTimeline.RemoveAll((s) => Timeline.Contains(s));
                    int pos = 0;
                    foreach (var newtweet in updatedTimeline)
                        Application.Current.Dispatcher.Invoke(() => {
                            Timeline.Insert(pos++, newtweet);
                        });
                    SelectedTweetId += pos;
                    
                }
                if (!IsInitialized)
                {
                    IsInitialized = true;
                    Log.Info("Module succesfully initialized");
                }
                _refreshTimelineTimer.Start();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "An error occured when trying to refresh channel info : ");
            }
        }

        public void PublishMessage(string content)
        {
            try
            {
                _apiLink.PublishMessage(content);
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("401"))
                {
                    // Display not enough right
                }
            }
        }

        private ObservableCollection<SocialMessage> _timeline = new ObservableCollection<SocialMessage>();
        public ObservableCollection<SocialMessage> Timeline
        {
            get { return _timeline; }
            set { Set("Timeline", ref _timeline, value); }
        }

        private int _selectedTweetId;
        public int SelectedTweetId
        {
            get { return _selectedTweetId; }
            set
            {
                Set("SelectedTweetId", ref _selectedTweetId, value);
                TweetChanged?.Invoke(this, null);
            }
        }

        public event EventHandler TweetChanged;


        private bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
            set { Set("IsInitialized", ref isInitialized, value); }
        }

        private string _hashtag;
        public string Hashtag
        {
            get { return _hashtag; }
            set { Set("Hashtag", ref _hashtag, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set<T>(string propertyName, ref T property, T value)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static SocialData instance;
        public static SocialData Instance
        {
            get
            {
                if (instance == null)
                    instance = new SocialData();

                return instance;
            }
        }
    }
}
