using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UltimateStreamMgr.Model.Api.StreamApis;

namespace UltimateStreamMgr.View.Settings.Stream
{
    /// <summary>
    /// Logique d'interaction pour TwitchAuthorizationWindow.xaml
    /// </summary>
    public partial class TwitchAuthorizationWindow : MetroWindow
    {
        public TwitchAuthorizationWindow()
        {
            InitializeComponent();
            browser.FrameLoadEnd += HideProgress;
            browser.FrameLoadStart += ShowProgress;
            browser.LoadError += Rofl;
            browser.Address = Twitch.GetAuthentificationUrl();
        }

        private void Rofl(object sender, LoadErrorEventArgs e)
        {
            string rawurl = e.FailedUrl;
            string[] url = rawurl.Split(new string[] { "#access_token=" }, StringSplitOptions.RemoveEmptyEntries);
            if (url.Count() > 1)
            {
                Token = url[1].Split('&')[0];
                Dispatcher.Invoke(() => Close());
            }
        }

        private void ShowProgress(object sender, FrameLoadStartEventArgs e)
        {
            Dispatcher.Invoke(() => progress.Visibility = Visibility.Visible);
        }

        private void HideProgress(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.Invoke(() => progress.Visibility = Visibility.Collapsed );
        }

        public string Token { get; set; }
    }
}
