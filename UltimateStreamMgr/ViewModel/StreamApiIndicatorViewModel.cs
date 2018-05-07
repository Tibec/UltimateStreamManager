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
    class StreamApiIndicatorViewModel : DockWindowViewModel
    {
        public StreamApiIndicatorViewModel() : base ()
        {
            Title = "Stream Status";

            OpenChatCommand = new RelayCommand(() => OpenChat());
            EditChannelCommand = new RelayCommand(() => EditChannel());
            OpenSettingsCommand = new RelayCommand(() => OpenSettings());

            ChannelData = StreamChannelData.Instance;

            Games = new ObservableCollection<Game> { new Game { Name = "Super Smash Bros. for Wii U" } };
        }

        private StreamChannelData _channelData;
        public StreamChannelData ChannelData
        {
            get { return _channelData; }
            set { Set("ChannelData", ref _channelData, value); }
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
                ChannelData.UpdateChannelInfo(EditName, EditGame);
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
