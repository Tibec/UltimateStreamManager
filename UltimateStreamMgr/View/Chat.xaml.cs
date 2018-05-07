using MahApps.Metro.Controls;
using System.Windows.Controls;
using CefSharp;
using System;
using System.Windows;

namespace UltimateStreamMgr.View
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public Chat()
        {
            InitializeComponent();
            browser.FrameLoadEnd += HideProgress;
            browser.FrameLoadStart += ShowProgress;
        }

        private void ShowProgress(object sender, FrameLoadStartEventArgs e)
        {
            progress.Dispatcher.BeginInvoke((Action)(() => {
                progress.Visibility = Visibility.Visible;
            }));
        }

        private void HideProgress(object sender, FrameLoadEndEventArgs e)
        {
            progress.Dispatcher.BeginInvoke((Action)(() => {
                progress.Visibility = Visibility.Collapsed;
            }));
        }
    }
    
}