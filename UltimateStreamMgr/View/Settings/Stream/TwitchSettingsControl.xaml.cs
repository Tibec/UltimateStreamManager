using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UltimateStreamMgr.View.Settings.Stream
{
    /// <summary>
    /// Logique d'interaction pour TwitchSettingsControl.xaml
    /// </summary>
    public partial class TwitchSettingsControl : UserControl
    {
        public TwitchSettingsControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            content.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            content.IsEnabled = (bool)e.NewValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new TwitchAuthorizationWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this.TryFindParent<MetroWindow>();
            win.ShowDialog();
            if(!string.IsNullOrEmpty(win.Token))
            {
                token.Text = win.Token;
            }
        }
    }
}
