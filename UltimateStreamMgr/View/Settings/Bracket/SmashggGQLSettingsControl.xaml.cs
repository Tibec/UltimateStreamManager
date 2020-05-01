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

namespace UltimateStreamMgr.View.Settings.Bracket
{
    /// <summary>
    /// Logique d'interaction pour SmashggGQLSettingsControl.xaml
    /// </summary>
    public partial class SmashggGQLSettingsControl : UserControl
    {
        public SmashggGQLSettingsControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            content.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            content.IsEnabled = (bool)e.NewValue;
        }

    }
}
