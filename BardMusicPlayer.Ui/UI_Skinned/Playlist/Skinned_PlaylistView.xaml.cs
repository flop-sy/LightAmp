#region

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using Microsoft.Win32;
using UI.Resources;
using Path = System.IO.Path;

#endregion

namespace BardMusicPlayer.Ui.Skinned;

/// <summary>
///     Interaktionslogik für Skinned_PlaylistView.xaml
/// </summary>
public sealed partial class Skinned_PlaylistView
{
    private IPlaylist _currentPlaylist; //The currently used playlist
    public EventHandler<BmpSong> OnLoadSongFromPlaylist;

    public Skinned_PlaylistView()
    {
        InitializeComponent();
        ApplySkin();
        SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;
        BmpSiren.Instance.SynthTimePositionChanged +=
            Instance_SynthTimePositionChanged; //Handled in Skinned_PlaylistView_Siren.cs

        _currentPlaylist = PlaylistFunctions.GetFirstPlaylist() ?? PlaylistFunctions.CreatePlaylist("default");
        RefreshPlaylist();
    }

    public bool NormalPlay { get; set; } = true; //True if normal or false if shuffle

    public bool
        LoopPlay { get; set; } //if true play the whole playlist and repeat, also enables the auto load next song


    /// <summary>
    ///     Refreshes the PlaylistContainer, clears the items and rereads them
    /// </summary>
    private void RefreshPlaylist()
    {
        PlaylistContainer.Items.Clear();
        if (_currentPlaylist == null)
            return;

        foreach (var d in _currentPlaylist)
            PlaylistContainer.Items.Add(d.Title);
        var itemContainerStyle = new Style(typeof(ListBoxItem));
        itemContainerStyle.Setters.Add(new Setter(AllowDropProperty, true));
        itemContainerStyle.Setters.Add(new EventSetter(PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(PlaylistContainer_PreviewMouseLeftButtonDown)));
        itemContainerStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(PlaylistContainer_Drop)));
        PlaylistContainer.ItemContainerStyle = itemContainerStyle;
    }

    /// <summary>
    ///     plays or loeads the prev song from the playlist
    /// </summary>
    public void PlayPrevSong()
    {
        if (NormalPlay)
        {
            var idx = PlaylistContainer.SelectedIndex;
            if (idx - 1 <= -1)
                return;

            PlaylistContainer.SelectedIndex = idx - 1;
        }
        else
        {
            var rnd = new Random();
            PlaylistContainer.SelectedIndex = rnd.Next(0, PlaylistContainer.Items.Count);
        }

        var item = PlaylistContainer.SelectedItem as string;
        var song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, item);
        PlaybackFunctions.LoadSongFromPlaylist(song);

        //Check if autoplay is set
        PlaybackFunctions.PlaybackState = BmpPigeonhole.Instance.PlaylistAutoPlay
            ? PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYNEXT
            : PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_STOPPED;
        OnLoadSongFromPlaylist?.Invoke(this, song);
    }

    /// <summary>
    ///     plays or loeads the next song from the playlist
    /// </summary>
    public void PlayNextSong()
    {
        if (NormalPlay)
        {
            var idx = PlaylistContainer.SelectedIndex;
            if (idx + 1 >= PlaylistContainer.Items.Count)
            {
                if (LoopPlay)
                    PlaylistContainer.SelectedIndex = 0;
                else
                    return;
            }

            PlaylistContainer.SelectedIndex = idx + 1;
        }
        else
        {
            var rnd = new Random();
            PlaylistContainer.SelectedIndex = rnd.Next(0, PlaylistContainer.Items.Count);
        }

        var item = PlaylistContainer.SelectedItem as string;
        var song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, item);
        PlaybackFunctions.LoadSongFromPlaylist(song);

        //Check if autoplay is set
        PlaybackFunctions.PlaybackState = BmpPigeonhole.Instance.PlaylistAutoPlay
            ? PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYNEXT
            : PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_STOPPED;
        OnLoadSongFromPlaylist?.Invoke(this, song);
    }

    #region Sel_Button_Menu

    /// <summary>
    ///     Search for a song in the playlist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Search_Click(object sender, RoutedEventArgs e)
    {
        var inputbox = new TextInputWindow("Search for...", 80);
        inputbox.Focus();
        if (inputbox.ShowDialog() != true) return;

        try
        {
            var song = _currentPlaylist
                .First(x => x.Title.ToLower().Contains(inputbox.ResponseText.ToLower()));
            PlaylistContainer.SelectedIndex = PlaylistContainer.Items.IndexOf(song.Title);
            PlaylistContainer.ScrollIntoView(PlaylistContainer.Items[PlaylistContainer.SelectedIndex]);
            PlaylistContainer.UpdateLayout();
        }
        catch
        {
            MessageBox.Show("Nothing found", "Nope", MessageBoxButton.OK);
        }
    }

    #endregion

    #region Misc_Button_Menu

    /// <summary>
    ///     Exports a selected song in the playlist to Midi
    /// </summary>
    private void Export_Midi_Click(object sender, RoutedEventArgs e)
    {
        foreach (var song in from string s in PlaylistContainer.SelectedItems
                 select PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, s)
                 into song
                 where song != null
                 select song)
        {
            Stream myStream;
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "MIDI file (*.mid)|*.mid",
                FilterIndex = 2,
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    song.GetExportMidi().WriteTo(myStream);
                    myStream.Close();
                }

            break;
        }
    }

    #endregion

    /// <summary>
    ///     triggered from playlist browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPlaylistChanged(object sender, string e)
    {
        _currentPlaylist = BmpCoffer.Instance.GetPlaylist(e);
        RefreshPlaylist();
    }

    /// <summary>
    ///     Button context menu routine
    /// </summary>
    private void MenuButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;

        if (sender is not Rectangle rectangle) return;

        var contextMenu = rectangle.ContextMenu;
        if (contextMenu == null) return;

        contextMenu.PlacementTarget = rectangle;
        contextMenu.Placement = PlacementMode.Top;
        contextMenu.IsOpen = true;
    }

    #region Skinning

    private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
    {
        ApplySkin();
    }

    public void ApplySkin()
    {
        var col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG];
        Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));

        Playlist_Top_Left.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_LEFT_CORNER];
        PLAYLIST_TITLE_BAR.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_TITLE_BAR];
        PLAYLIST_TOP_TILE.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_TILE];
        PLAYLIST_TOP_TILE_II.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_TILE];
        PLAYLIST_TOP_RIGHT_CORNER.Fill =
            SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_TOP_RIGHT_CORNER];

        PLAYLIST_LEFT_TILE.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_LEFT_TILE];
        PLAYLIST_RIGHT_TILE.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_RIGHT_TILE];

        PLAYLIST_BOTTOM_LEFT_CORNER.Fill =
            SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_BOTTOM_LEFT_CORNER];
        PLAYLIST_BOTTOM_TILE.Fill = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_BOTTOM_TILE];
        PLAYLIST_BOTTOM_RIGHT_CORNER.Fill =
            SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_BOTTOM_RIGHT_CORNER];

        Close_Button.Background = SkinContainer.PLAYLIST[SkinContainer.PLAYLIST_TYPES.PLAYLIST_CLOSE_SELECTED];
        Close_Button.Background.Opacity = 0;

        col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG];
        PlaylistContainer.Background = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
        col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMAL];
        PlaylistContainer.Foreground = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));

        PlaylistContainer_SelectionChanged(null, null);
    }

    #endregion

    #region PlaylistContainer actions

    /// <summary>
    ///     MouseDoubleClick action: load the clicked song into the sequencer
    /// </summary>
    private void PlaylistContainer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        PlaybackFunctions.StopSong();
        var songTitle = PlaylistContainer.SelectedItem as string;

        var song = _currentPlaylist.AsParallel().First(s => s.Title == songTitle);
        if (song == null)
            return;

        PlaybackFunctions.LoadSongFromPlaylist(song);
        OnLoadSongFromPlaylist?.Invoke(this, song);
    }

    /// <summary>
    ///     the selection changed action. Set the selected song and change the highlight color
    /// </summary>
    private void PlaylistContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SirenCurrentSongIndex = PlaylistContainer.SelectedIndex; //tell siren our current song index

        //Fancy coloring
        var col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMAL];
        var fcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
        col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_NORMALBG];
        var bcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
        for (var i = 0; i < PlaylistContainer.Items.Count; i++)
        {
            var lvitem = PlaylistContainer.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
            if (lvitem == null)
                continue;

            lvitem.Foreground = fcol;
            lvitem.Background = bcol;
        }

        col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_CURRENT];
        fcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));
        col = SkinContainer.PLAYLISTCOLOR[SkinContainer.PLAYLISTCOLOR_TYPES.PLAYLISTCOLOR_SELECTBG];
        bcol = new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B));

        if (PlaylistContainer.ItemContainerGenerator.ContainerFromItem(PlaylistContainer.SelectedItem) is not
            ListViewItem lvtem)
            return;

        lvtem.Foreground = fcol;
        lvtem.Background = bcol;
    }

    /// <summary>
    ///     Drag start function to move songs in the playlist
    /// </summary>
    private void PlaylistContainer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListBoxItem draggedItem) return;

        PlaylistContainer.SelectedItem = draggedItem;

        DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
        draggedItem.IsSelected = true;
    }

    /// <summary>
    ///     DragStop to place the song
    /// </summary>
    private void PlaylistContainer_Drop(object sender, DragEventArgs e)
    {
        var target = ((ListBoxItem)sender).DataContext as string;

        if (e.Data.GetData(typeof(string)) is not string droppedData) return;

        var removedIdx = PlaylistContainer.Items.IndexOf(droppedData);
        if (target == null) return;

        var targetIdx = PlaylistContainer.Items.IndexOf(target);

        if (removedIdx < targetIdx)
        {
            PlaylistContainer.Items.Insert(targetIdx + 1, droppedData);
            PlaylistContainer.Items.RemoveAt(removedIdx);

            _currentPlaylist.Move(removedIdx, targetIdx);
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
        }
        else if (removedIdx == targetIdx)
        {
            PlaylistContainer.SelectedIndex = targetIdx;
        }
        else
        {
            var remIdx = removedIdx + 1;
            if (PlaylistContainer.Items.Count + 1 <= remIdx) return;

            PlaylistContainer.Items.Insert(targetIdx, droppedData);
            PlaylistContainer.Items.RemoveAt(remIdx);
            _currentPlaylist.Move(removedIdx, targetIdx);
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
        }
    }

    #endregion

    #region Add_Button_Menu

    /// <summary>
    ///     Add file(s) to the playlist
    /// </summary>
    private void AddFiles_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        if (!PlaylistFunctions.AddFilesToPlaylist(_currentPlaylist))
            return;

        RefreshPlaylist();
    }

    private void AddFolder_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        if (!PlaylistFunctions.AddFolderToPlaylist(_currentPlaylist))
            return;

        RefreshPlaylist();
    }

    #endregion

    #region Del_Button_Menu

    /// <summary>
    ///     Removes the selected song(s) from the playlist
    /// </summary>
    private void RemoveSelected_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        foreach (var song in from string s in PlaylistContainer.SelectedItems
                 select PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, s)
                 into song
                 where song != null
                 select song)
        {
            _currentPlaylist.Remove(song);
            BmpCoffer.Instance.DeleteSong(song);
        }

        BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
        RefreshPlaylist();
    }

    /// <summary>
    ///     Clears the whole playlist
    /// </summary>
    private void ClearPlaylist_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        foreach (var song in from string s in PlaylistContainer.Items
                 select PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, s)
                 into song
                 where song != null
                 select song)
        {
            _currentPlaylist.Remove(song);
            BmpCoffer.Instance.DeleteSong(song);
        }

        BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
        RefreshPlaylist();
    }

    #endregion

    #region List_Button_Menu

    /// <summary>
    ///     Creates a new music catalog, loads it and calls RefreshPlaylist()
    /// </summary>
    private void MenuItem_CreateCatalog(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = Globals.Globals.MusicCatalogFilters,
            InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" +
                               Globals.Globals.DataPath
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        BmpCoffer.Instance.LoadNew(openFileDialog.FileName);
        BmpPigeonhole.Instance.LastLoadedCatalog = openFileDialog.FileName;
        _currentPlaylist = PlaylistFunctions.GetFirstPlaylist() ?? PlaylistFunctions.CreatePlaylist("default");
        RefreshPlaylist();
    }

    /// <summary>
    ///     Loads a MusicCatalog, loads it and calls RefreshPlaylist()
    /// </summary>
    private void MenuItem_LoadCatalog(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = Globals.Globals.MusicCatalogFilters,
            Multiselect = false,
            InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" +
                               Globals.Globals.DataPath
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        if (!openFileDialog.CheckFileExists)
            return;

        BmpCoffer.Instance.LoadNew(openFileDialog.FileName);
        BmpPigeonhole.Instance.LastLoadedCatalog = openFileDialog.FileName;
        _currentPlaylist = PlaylistFunctions.GetFirstPlaylist() ?? PlaylistFunctions.CreatePlaylist("default");
        RefreshPlaylist();
    }

    /// <summary>
    ///     the export function, triggered from the Ui
    /// </summary>
    private void MenuItem_ExportCatalog(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = Globals.Globals.MusicCatalogFilters
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        BmpCoffer.Instance.Export(openFileDialog.FileName);
    }

    /// <summary>
    ///     triggeres the reabase function from Coffer
    /// </summary>
    private void MenuItem_CleanUpCatalog(object sender, RoutedEventArgs e)
    {
        BmpCoffer.Instance.CleanUpDB();
    }

    /// <summary>
    ///     opens the playlists browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_OpenPlaylistBrowser(object sender, RoutedEventArgs e)
    {
        var mb = new MediaBrowser();
        mb.Show();
        mb.OnPlaylistChanged += OnPlaylistChanged;
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
        Visibility = Visibility.Hidden;
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
}