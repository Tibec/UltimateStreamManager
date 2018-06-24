using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api.SocialApis;

namespace UltimateStreamMgr.ViewModel
{
    class SocialModuleViewModel : DockWindowViewModel
    {

        public SocialModuleViewModel()
        {
            Title = "Social";

            Data = SocialData.Instance;

            Configuration.Instance.SocialSettingsChanged += ReloadSettings;

            SendMessage = new RelayCommand(() => SendMessageCommand());

            Data.TweetChanged += ChangeTweet;
        }

        private void ChangeTweet(object sender, EventArgs e)
        {
            if(Data.SelectedTweetId >= 0 && Data.SelectedTweetId < Data.Timeline.Count)
                Output.Data.HighlightedTweet = Data.Timeline[Data.SelectedTweetId];
        }

        private void ReloadSettings()
        {
            if(Configuration.Instance.Social is TwitterSettings)
            {
                Data.Reset();
            }
        }

        private SocialData _data;
        public SocialData Data
        {
            get { return _data; }
            set { Set("Data", ref _data, value); }
        }

        private string newMessage;
        public string NewMessage
        {
            get { return newMessage; }
            set { Set("NewMessage", ref newMessage, value); }
        }


        public RelayCommand SendMessage { get; set; }
        private void SendMessageCommand()
        {
            Data.PublishMessage(NewMessage);
            NewMessage = "";
        }
    }
}
