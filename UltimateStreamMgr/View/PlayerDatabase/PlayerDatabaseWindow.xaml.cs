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
using UltimateStreamMgr.Model;
using UltimateStreamMgr.ViewModel;

namespace UltimateStreamMgr.View.PlayerDatabase
{
    /// <summary>
    /// Logique d'interaction pour PlayerDatabaseWindow.xaml
    /// </summary>
    public partial class PlayerDatabaseWindow : MetroWindow
    {
        private Flyout editWindow;

        public PlayerDatabaseWindow()
        {
            InitializeComponent();
            editWindow = Flyouts.Items[0] as Flyout;
            Messenger.Default.Register<NotificationMessage>(this, (msg) => HandleMessage(msg));
        }

        private void HandleMessage(NotificationMessage msg)
        {
            switch(msg.Notification)
            {
                case "OpenEditionWindow":
                    editWindow.IsOpen = true;
                    break;
                case "CloseEditionWindow":
                    editWindow.IsOpen = false;
                    break;
            }
        }
    }
}
