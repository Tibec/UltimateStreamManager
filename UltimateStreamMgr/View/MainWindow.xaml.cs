using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UltimateStreamMgr.ViewModel;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace UltimateStreamMgr.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            Messenger.Default.Register<NotificationMessage>(this, HandleMessage);
            Messenger.Default.Register<string>(this, HandleDockChanged);
            DataContext = new MainViewModel();

            Timer t = new Timer(1000);
            t.AutoReset = true;
            t.Elapsed += dockMgr_LayoutChanged;
            t.Start();
        }

        private void HandleMessage(NotificationMessage obj)
        {
            if(obj.Notification == "OpenChat")
            {
                Window w = new Chat();
                w.WindowStartupLocation = WindowStartupLocation.Manual;
                double startX = Left + Width + 3;
                double startY = Top;
                w.Left = startX;
                w.Top = startY;
                w.Show();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window w = new PlayerDatabase.PlayerDatabaseWindow();
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = this;
            w.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window w = new Settings.SettingsWindow();
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = this;
            w.Show();
        }

        private void dockMgr_LayoutChanged(object sender, EventArgs e)
        {
            if (serializedDock == null)
                return;
            Dispatcher.Invoke(() =>
            {
                var serializer = new XmlLayoutSerializer(dockMgr);
                using (StringWriter textWriter = new StringWriter())
                {
                    serializer.Serialize(textWriter);
                    serializedDock.Text = textWriter.ToString();
                }
            });
        }

        private void HandleDockChanged(string obj)
        {
            var serializer = new XmlLayoutSerializer(dockMgr);
            using (TextReader reader = new StringReader(obj))
            {
                serializer.Deserialize(reader);
            }
        }

        private void test()
        {

        }
    }
}
