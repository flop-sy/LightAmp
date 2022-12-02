#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Ui.Globals.SkinContainer;

#endregion

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    ///     Interaktionslogik für MediaBrowser.xaml
    /// </summary>
    public partial class MediaBrowser : Window
    {
        private IPlaylist _currentPlaylist; //The current selected playlist at the browser
        private int _currentPlaylistIndex;
        public EventHandler<string> OnPlaylistChanged;

        public MediaBrowser()
        {
            InitializeComponent();
            ApplySkin();
            SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;

            PlaylistsContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
        }

        private void PlaylistsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ui things
            var col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMAL];
            var fcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG];
            var bcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            for (var i = 0; i < PlaylistsContainer.Items.Count; i++)
            {
                var lvitem = PlaylistsContainer.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (lvitem == null)
                    continue;
                lvitem.Foreground = fcol;
                lvitem.Background = bcol;
            }

            col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_CURRENT];
            fcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_SELECTBG];
            bcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));

            var lvtem =
                PlaylistsContainer.ItemContainerGenerator.ContainerFromItem(PlaylistsContainer.SelectedItem) as
                    ListViewItem;
            if (lvtem == null)
                return;
            lvtem.Foreground = fcol;
            lvtem.Background = bcol;

            //functional things
            _currentPlaylistIndex = PlaylistsContainer.SelectedIndex;
            _currentPlaylist = BmpCoffer.Instance.GetPlaylist(PlaylistsContainer.SelectedItem as string);
            PlaylistContainer.Items.Clear();
            foreach (var item in _currentPlaylist)
                PlaylistContainer.Items.Add(item.Title);
            PlaylistName_Box.Text = _currentPlaylist.GetName();
        }

        private void PlaylistsContainer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnPlaylistChanged.Invoke(this, (string)PlaylistsContainer.SelectedItem);
        }

        #region Skinning

        private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
        {
            ApplySkin();
        }

        public void ApplySkin()
        {
            //top
            MEDIABROWSER_TOP_LEFT.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_LEFT];
            MEDIABROWSER_TOP_TILE.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TILE];
            MEDIABROWSER_TOP_TITLE.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TITLE];
            MEDIABROWSER_TOP_TILE_II.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_TILE];
            MEDIABROWSER_TOP_RIGHT.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_TOP_RIGHT];
            //mid
            MEDIABROWSER_LEFT_TILE.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_MID_LEFT];
            MEDIABROWSER_RIGHT_TILE.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_MID_RIGHT];
            //bottom
            MEDIABROWSER_BOTTOM_LEFT_CORNER.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_BOTTOM_LEFT];
            MEDIABROWSER_BOTTOM_TILE.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_BOTTOM_TILE];
            MEDIABROWSER_BOTTOM_RIGHT_CORNER.Fill =
                SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_BOTTOM_RIGHT];
            Close_Button.Background = SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_CLOSE];
            Close_Button.Background.Opacity = 0;

            Prev_Button.Background = SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_PREV];
            Prev_Button.Background.Opacity = 0;
            Next_Button.Background = SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_NEXT];
            Next_Button.Background.Opacity = 0;
            New_Button.Background = SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_NEW];
            New_Button.Background.Opacity = 0;
            Reload_Button.Background = SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_RELOAD];
            Reload_Button.Background.Opacity = 0;
            Remove_Button.Background = SkinContainer.MEDIABROWSER[SkinContainer.MEDIABROWSER_TYPES.MEDIABROWSER_REMOVE];
            Remove_Button.Background.Opacity = 0;

            var col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG];
            Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG];
            PlaylistsContainer.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            PlaylistContainer.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            PlaylistName_Box.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));

            col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMAL];
            PlaylistsContainer.Foreground = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            PlaylistContainer.Foreground = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
            PlaylistName_Box.Foreground = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
        }

        #endregion


        #region Titlebar functions and buttons

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Close_Button.Background.Opacity = 1;
        }

        private void Close_Button_Up(object sender, MouseButtonEventArgs e)
        {
            Close_Button.Background.Opacity = 0;
        }

        #endregion


        #region minibar functions and buttons

        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylistIndex <= 0)
                return;

            _currentPlaylistIndex--;
            var t = PlaylistsContainer.Items[_currentPlaylistIndex] as string;
            _currentPlaylist = BmpCoffer.Instance.GetPlaylist(t);
            PlaylistsContainer.SelectedIndex = _currentPlaylistIndex;
        }

        private void Prev_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Prev_Button.Background.Opacity = 1;
        }

        private void Prev_Button_Up(object sender, MouseButtonEventArgs e)
        {
            Prev_Button.Background.Opacity = 0;
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylistIndex == PlaylistsContainer.Items.Count - 1)
                return;

            _currentPlaylistIndex++;
            var t = PlaylistsContainer.Items[_currentPlaylistIndex] as string;
            _currentPlaylist = BmpCoffer.Instance.GetPlaylist(t);
            PlaylistsContainer.SelectedIndex = _currentPlaylistIndex;
        }

        private void Next_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Next_Button.Background.Opacity = 1;
        }

        private void Next_Button_Up(object sender, MouseButtonEventArgs e)
        {
            Next_Button.Background.Opacity = 0;
        }

        private void New_Button_Click(object sender, RoutedEventArgs e)
        {
            if (BmpCoffer.Instance.GetPlaylistNames().Contains(PlaylistName_Box.Text))
                return;
            _currentPlaylist = BmpCoffer.Instance.CreatePlaylist(PlaylistName_Box.Text);
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
            PlaylistsContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
        }

        private void New_Button_Down(object sender, MouseButtonEventArgs e)
        {
            New_Button.Background.Opacity = 1;
        }

        private void New_Button_Up(object sender, MouseButtonEventArgs e)
        {
            New_Button.Background.Opacity = 0;
        }

        private void Reload_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylist == null)
                return;
            _currentPlaylist.SetName(PlaylistName_Box.Text);
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
            PlaylistsContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
        }

        private void Reload_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Reload_Button.Background.Opacity = 1;
        }

        private void Reload_Button_Up(object sender, MouseButtonEventArgs e)
        {
            Reload_Button.Background.Opacity = 0;
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylist == null)
                return;
            BmpCoffer.Instance.DeletePlaylist(_currentPlaylist);
            PlaylistsContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
        }

        private void Remove_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Remove_Button.Background.Opacity = 1;
        }

        private void Remove_Button_Up(object sender, MouseButtonEventArgs e)
        {
            Remove_Button.Background.Opacity = 0;
        }

        #endregion
    }
}