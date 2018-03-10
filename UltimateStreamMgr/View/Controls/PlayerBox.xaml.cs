using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.View.Controls
{
    /// <summary>
    /// Logique d'interaction pour PlayerBox.xaml
    /// </summary>
    public partial class PlayerBox : UserControl
    {
        public PlayerBox()
        {
            InitializeComponent();

            SimilarPlayers = new ObservableCollection<Player>();

            UpdateButtonVisibility();
        }

        #region Publics settings

        public bool DisplayFlag { get; set; }

        public string Watermark
        {
            set { TextBoxHelper.SetWatermark(playerName, value); }
        }

        public ICommand ButtonCommand { get; set; }

        #endregion

        #region Dependencies Properties

        public static readonly DependencyProperty SelectedPlayerProperty = DependencyProperty.Register(
          "SelectedPlayer",
          typeof(Player),
          typeof(PlayerBox),
          new FrameworkPropertyMetadata(
            null, 
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedPlayerChanged));

        public Player SelectedPlayer
        {
            get { return (Player)GetValue(SelectedPlayerProperty); }
            set { SetValue(SelectedPlayerProperty, value); }
        }

        static private void OnSelectedPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlayerBox pb = (PlayerBox)d;
            if (pb.SelectedPlayer != null)
            {
                pb.tagDisp.Visibility = Visibility.Visible;
                pb.name.Text = pb.SelectedPlayer.Name;
                if (pb.SelectedPlayer.Team != null)
                    pb.team.Text = pb.SelectedPlayer.Team.ShortName + " | ";
                else
                    pb.team.Text = "";
            }
            else
            {
                pb.tagDisp.Visibility = Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty PlayerSourceProperty = DependencyProperty.Register(
          "PlayerSource",
          typeof(IEnumerable<Player>),
          typeof(PlayerBox),
          new FrameworkPropertyMetadata(
            null, 
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable<Player> PlayerSource
        {
            get { return (IEnumerable<Player>)GetValue(PlayerSourceProperty); }
            set { SetValue(PlayerSourceProperty, value); }
        }

        public static readonly DependencyProperty SimilarPlayersProperty = DependencyProperty.Register(
          "SimilarPlayers",
          typeof(ObservableCollection<Player>),
          typeof(PlayerBox),
          new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<Player> SimilarPlayers
        {
            get { return (ObservableCollection<Player>)GetValue(SimilarPlayersProperty); }
            set { SetValue(SimilarPlayersProperty, value); }
        }



        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text",
          typeof(string),
          typeof(PlayerBox),
          new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        private void playerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonVisibility();

            if (string.IsNullOrEmpty(playerName.Text) || !playerName.IsFocused)
            {
                SimilarPlayers.Clear();
                suggestionPopup.IsOpen = false;
                return;
            }

            if (SelectedPlayer != null && SelectedPlayer.Name != playerName.Text)
                SelectedPlayer = null;

            List<Player> similar = PlayerSource.Where((p) => p.Name.StartsWith(playerName.Text, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (Player p in SimilarPlayers.ToList())
                if(!similar.Contains(p))
                    SimilarPlayers.Remove(p);

            similar.ForEach((p) => {
                if (!SimilarPlayers.Contains(p))
                    SimilarPlayers.Add(p);
            });

            suggestionPopup.IsOpen = SimilarPlayers.Count > 0;
        }

        private void UpdateButtonVisibility()
        {
            if(string.IsNullOrEmpty(Text))
            {
                settingsBtn.Visibility = clearBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                settingsBtn.Visibility = clearBtn.Visibility = Visibility.Visible;
            }
        }

        private void playerName_GotFocus(object sender, RoutedEventArgs e)
        {
            suggestionPopup.IsOpen = !string.IsNullOrEmpty(playerName.Text);
        }

        private void playerName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.HeightChanged)
            {
                double diff = e.NewSize.Height - e.PreviousSize.Height;
                suggestionPopup.HorizontalOffset = diff;
            }
            if(e.WidthChanged)
            {
                suggestionPopup.Width = e.NewSize.Width;
            }
        }

        private void PackIconModern_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Settings(object sender, RoutedEventArgs e)
        {
            ButtonCommand?.Execute(null);
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            Text = "";
            SelectedPlayer = null;
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void suggestionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            suggestionPopup.IsOpen = false;
            if (e.AddedItems.Count > 0)
            {
                SelectedPlayer = e.AddedItems[0] as Player;
                Text = SelectedPlayer.Name;
            }
        }

        private void tagDisp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tagDisp.Visibility = Visibility.Collapsed;
            playerName.Focus();
            playerName.SelectAll();
        }

        private void playerName_LostFocus(object sender, RoutedEventArgs e)
        {
            if(SelectedPlayer != null && tagDisp.Visibility == Visibility.Collapsed)
            {
                tagDisp.Visibility = Visibility.Visible;
            }
        }
    }


}
