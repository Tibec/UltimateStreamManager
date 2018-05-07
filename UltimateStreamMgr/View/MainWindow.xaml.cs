using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace UltimateStreamMgr.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow(bool firstLaunch)
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


            if(firstLaunch)
            {
                this.IsVisibleChanged += PopupFirstLaunch;
            }
        }

        private void PopupFirstLaunch(object sender, DependencyPropertyChangedEventArgs e)
        {
            Window w = new Settings.SettingsWindow();
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = this;
            w.Show();
            MessageBox.Show(w, "Il s'agit du premier lancement. Veuillez renseigner les paramètres nécessaires au bon fonctionnement de l'application");
            this.IsVisibleChanged -= PopupFirstLaunch;
        }

        private void HandleMessage(NotificationMessage obj)
        {
            if(obj.Notification == "OpenChat")
            {
                var window = new LayoutAnchorable()
                {
                    Content = new ChatViewModel(),
                    FloatingHeight = 500,
                    FloatingWidth = 200,
                    
                };

                window.AddToLayout(dockMgr, AnchorableShowStrategy.Top);
                window.Float();
            }
        }

        private void OpenPlayerDatabase(object sender = null, RoutedEventArgs e=null)
        {
            Window w = new PlayerDatabase.PlayerDatabaseWindow();
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = this;
            w.Show();
        }

        private void OpenSettings(object sender=null, RoutedEventArgs e=null)
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
            serializer.LayoutSerializationCallback += LayoutDeserialization;
            using (TextReader reader = new StringReader(obj))
            {
                serializer.Deserialize(reader);
            }
        }
        
        private void LayoutDeserialization(object sender, LayoutSerializationCallbackEventArgs args)
        {
            try
            {
                Type t = Type.GetType(args.Model.ContentId);
                DockWindowViewModel vm = null;
                foreach(var anchorable in dockMgr.AnchorablesSource)
                {
                    if (anchorable.GetType() == t)
                    {
                        vm = anchorable as DockWindowViewModel;
                        vm.IsVisible = args.Model.IsEnabled;
                    }
                }
                
                // found a match - return it
                if (vm != null)
                    args.Content = vm;
                else
                    args.Cancel = true;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
