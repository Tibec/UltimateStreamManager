using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.View;
using UltimateStreamMgr.ViewModel;

namespace UltimateStreamMgr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (File.Exists("config.xml"))
            {
                Configuration.Instance.Load("config.xml");
            }

            PlayerDatabase.Init();

            InitializeQuickConverter();

            MainWindow = new MainWindow();

            if (Configuration.Instance.Window.AppHeight != 0
                && Configuration.Instance.Window.AppWidth != 0)
            {

                MainWindow.Width = Configuration.Instance.Window.AppWidth;
                MainWindow.Height = Configuration.Instance.Window.AppHeight;
            }

            MainWindow.Show();
        }



        private void InitializeQuickConverter()
        {
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Visibility));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(string));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(ChannelStatus));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(OutputFormat));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(SetMode));
        }
    }
}
