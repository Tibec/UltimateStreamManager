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
    public partial class Chat : MetroWindow
    {
        public Chat()
        {
            InitializeComponent();
            browser.FrameLoadEnd += HideProgress;
            browser.FrameLoadStart += ShowProgress;
        }

        private void ShowProgress(object sender, FrameLoadStartEventArgs e)
        {
            Dispatcher.Invoke(() => progress.Visibility = Visibility.Visible);
        }

        private void HideProgress(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.Invoke(() => progress.Visibility = Visibility.Collapsed);
        }
    }
    
}