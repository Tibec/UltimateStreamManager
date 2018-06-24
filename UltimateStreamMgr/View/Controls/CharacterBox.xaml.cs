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
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.View.Controls
{
    /// <summary>
    /// Logique d'interaction pour CharacterBox.xaml
    /// </summary>
    public partial class CharacterBox : UserControl
    {
        public CharacterBox()
        {
            InitializeComponent();
        }

        #region Dependency Property

        public static readonly DependencyProperty SelectedCharacterProperty = DependencyProperty.Register(
          "SelectedCharacter",
          typeof(Character),
          typeof(CharacterBox),
          new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedCharacterChanged));

        public Character SelectedCharacter
        {
            get { return (Character)GetValue(SelectedCharacterProperty); }
            set { SetValue(SelectedCharacterProperty, value); }
        }

        static private void OnSelectedCharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterBox pb = (CharacterBox)d;
            if (pb.SelectedCharacter != null && !string.IsNullOrEmpty(pb.SelectedCharacter.FilePath))
            {
                pb.selectedCharImg.Source = new BitmapImage(new Uri(pb.SelectedCharacter.FilePath, UriKind.Absolute));
            }
            else
            {
            }
        }

        public static readonly DependencyProperty CharacterSourceProperty = DependencyProperty.Register(
          "CharacterSource",
          typeof(IEnumerable<Character>),
          typeof(CharacterBox),
          new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable<Character> CharacterSource
        {
            get { return (IEnumerable<Character>)GetValue(CharacterSourceProperty); }
            set { SetValue(CharacterSourceProperty, value); }
        }

        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listPopup.IsOpen = true;
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 1)
            {
                SelectedCharacter = e.AddedItems[0] as Character;
                listPopup.IsOpen = false;
            }
        }
    }
}
