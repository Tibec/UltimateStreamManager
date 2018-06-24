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

namespace UltimateStreamMgr.View
{
    /// <summary>
    /// Logique d'interaction pour SocialModule.xaml
    /// </summary>
    public partial class SocialModule : UserControl
    {
        public SocialModule()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow window = Application.Current.MainWindow as MetroWindow;
            await window.ShowMetroDialogAsync(uc.Resources["EditChannelDialog"] as CustomDialog, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Accented });
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MetroWindow window = Application.Current.MainWindow as MetroWindow;
            await window.HideMetroDialogAsync(uc.Resources["EditChannelDialog"] as CustomDialog);
        }

    }
}
