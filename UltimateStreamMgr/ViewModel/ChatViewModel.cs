using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.Model.Api;

namespace UltimateStreamMgr.ViewModel
{
    class ChatViewModel : BaseViewModel
    {
        public ChatViewModel()
        {
            Configuration.Instance.StreamSettingsChanged += RefreshApiLink;
            RefreshApiLink();
        }

        private void RefreshApiLink()
        {
            try
            {
                StreamApi apiLink = Activator.CreateInstance(Configuration.Instance.Stream.Api, Configuration.Instance.Stream) as StreamApi;
                string url = apiLink.GetChatUrl();
                if (ChatUrl != url)
                    ChatUrl = url;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private string _chatUrl;
        public string ChatUrl
        {
            get { return _chatUrl; }
            set { Set("ChatUrl", ref _chatUrl, value); }
        }

    }
}
