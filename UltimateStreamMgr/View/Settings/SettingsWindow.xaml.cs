using GalaSoft.MvvmLight.Messaging;
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

namespace UltimateStreamMgr.View.Settings
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, Exit);
        }

        ~SettingsWindow()
        {
            Messenger.Default.Unregister(this);
        }

        private void Exit(NotificationMessage msg)
        {
            if(msg.Notification == "CloseSettings")
                Close();
        }
    }
}
