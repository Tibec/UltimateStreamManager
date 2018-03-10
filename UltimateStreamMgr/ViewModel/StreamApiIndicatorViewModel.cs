using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.ViewModel
{
    class StreamApiIndicatorViewModel : ViewModelBase
    {
        private StreamApi _apiLink;
        private Timer _refreshInfo;

        public StreamApiIndicatorViewModel()
        {
            Configuration.Instance.StreamSettingsChanged += RefreshApiLink;
            RefreshApiLink();

            _refreshInfo = new Timer(1000);
            _refreshInfo.Elapsed += RefreshChannelInfo;
            _refreshInfo.Start();

            OpenChatCommand = new RelayCommand(() => OpenChat());
            EditChannelCommand = new RelayCommand(() => EditChannel());
            OpenSettingsCommand = new RelayCommand(() => OpenSettings());

            Games = new ObservableCollection<Game> { new Game { Name = "Super Smash Bros. for Wii U" } };
        }

        private void RefreshChannelInfo(object sender, ElapsedEventArgs e)
        {
            if(_apiLink !=null)
            {
                ChannelInfo = _apiLink.GetChannelInfo();
                if (ChannelInfo.Viewers > ViewersPeak)
                    ViewersPeak = ChannelInfo.Viewers;

                if(string.IsNullOrEmpty(EditName))
                    EditName = ChannelInfo.Title;
                if (string.IsNullOrEmpty(EditGame?.Name))
                    EditGame = ChannelInfo.Game;
            }
            _refreshInfo.Start();
        }

        private void RefreshApiLink()
        {
            try
            {
                _apiLink = Activator.CreateInstance(Configuration.Instance.Stream.Api, Configuration.Instance.Stream) as StreamApi;
            }
            catch(Exception e)
            {
                // api not set
            }
        }

        private ChannelInfo _channelInfo;
        public ChannelInfo ChannelInfo
        {
            get { return _channelInfo; }
            set { Set("ChannelInfo", ref _channelInfo, value); }
        }

        private int _viewersPeak;
        public int ViewersPeak
        {
            get { return _viewersPeak; }
            set { Set("ViewersPeak", ref _viewersPeak, value); }
        }

        private string _editName;
        public string EditName
        {
            get { return _editName; }
            set { Set("EditName", ref _editName, value); }
        }

        private Game _editGame;
        public Game EditGame
        {
            get { return _editGame; }
            set { Set("EditGame", ref _editGame, value); }
        }

        private ObservableCollection<Game> _games;
        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set { Set("Games", ref _games, value); }
        }

        public RelayCommand OpenChatCommand { get; set; }
        private void OpenChat()
        {
            MessengerInstance.Send(new NotificationMessage("OpenChat"));
        }

        public RelayCommand EditChannelCommand { get; set; }
        private void EditChannel()
        {
            try
            {
                _apiLink.UpdateChannelInfo(EditName, EditGame);
            }
            catch(HttpRequestException e)
            {
                if(e.Message.Contains("401"))
                {
                    // Display not enough right
                }
            }
        }

        public RelayCommand OpenSettingsCommand { get; set; }
        private void OpenSettings()
        {

        }
    }
}
