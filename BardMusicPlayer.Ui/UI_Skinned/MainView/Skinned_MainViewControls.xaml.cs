#region

using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Ui.Globals.SkinContainer;

#endregion

namespace BardMusicPlayer.Ui.Skinned;

public sealed partial class Skinned_MainView
{
    /// <summary>
    ///     load the prev song in the playlist
    /// </summary>
    private void Prev_Button_Click(object sender, RoutedEventArgs e)
    {
        Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON];
        _PlaylistView.PlayPrevSong();
    }

    private void Prev_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON_ACTIVE];
    }

    private void Prev_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON];
    }

    /// <summary>
    ///     play a loaded song
    /// </summary>
    private void Play_Button_Click(object sender, RoutedEventArgs e)
    {
        Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON];
        PlaybackFunctions.PlaySong(0);
    }

    private void Play_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON_ACTIVE];
    }

    private void Play_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON];
    }

    /// <summary>
    ///     pause the song playback
    /// </summary>
    private void Pause_Button_Click(object sender, RoutedEventArgs e)
    {
        Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON];
        PlaybackFunctions.PauseSong();
    }

    private void Pause_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON_ACTIVE];
    }

    private void Pause_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON];
    }

    /// <summary>
    ///     stop song playback
    /// </summary>
    private void Stop_Button_Click(object sender, RoutedEventArgs e)
    {
        Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON];
        PlaybackFunctions.StopSong();
    }

    private void Stop_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON_ACTIVE];
    }

    private void Stop_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON];
    }

    /// <summary>
    ///     Plays the next song in the playlist
    /// </summary>
    private void Next_Button_Click(object sender, RoutedEventArgs e)
    {
        Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON];
        _PlaylistView.PlayNextSong();
    }

    private void Next_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON_ACTIVE];
    }

    private void Next_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON];
    }

    /// <summary>
    ///     opens a song for single playback
    /// </summary>
    private void Load_Button_Click(object sender, RoutedEventArgs e)
    {
        Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON];
        if (!PlaybackFunctions.LoadSong()) return;

        Scroller.Cancel();
        Scroller = new CancellationTokenSource();
        UpdateScroller(Scroller.Token, PlaybackFunctions.GetSongName()).ConfigureAwait(false);
        WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());
    }

    private void Load_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON_ACTIVE];
    }

    private void Load_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON];
    }

    /// <summary>
    ///     The track selection
    /// </summary>
    private void Trackbar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (Trackbar_Slider.Value > MaxTracks)
            return;

        Trackbar_Background.Fill = SkinContainer.VOLUME[(SkinContainer.VOLUME_TYPES)Trackbar_Slider.Value];
        WriteSmallDigitField(Trackbar_Slider.Value.ToString(CultureInfo.InvariantCulture));
    }

    private void Trackbar_Slider_DragStarted(object sender, DragStartedEventArgs e)
    {
        _Trackbar_dragStarted = true;
    }

    private void Trackbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (Trackbar_Slider.Value > MaxTracks) Trackbar_Slider.Value = MaxTracks;

        BmpPigeonhole.Instance.PlayAllTracks = Trackbar_Slider.Value == 0;

        BmpMaestro.Instance.SetTracknumberOnHost((int)Trackbar_Slider.Value);
        _Trackbar_dragStarted = false;
    }

    /// <summary>
    ///     The octave shifting
    /// </summary>
    private void Octavebar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Octavebar_Background.Fill = SkinContainer.BALANCE[(SkinContainer.BALANCE_TYPES)Octavebar_Slider.Value];
        WriteSmallOctaveDigitField((Octavebar_Slider.Value - 4).ToString(CultureInfo.InvariantCulture));
    }

    private void Octavebar_Slider_DragStarted(object sender, DragStartedEventArgs e)
    {
        _Octavebar_dragStarted = true;
    }

    private void Octavebar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _Octavebar_dragStarted = false;
        BmpMaestro.Instance.SetOctaveshiftOnHost((int)Octavebar_Slider.Value - 4);
    }

    /// <summary>
    ///     open the settings
    /// </summary>
    private void Settings_Button_Click(object sender, RoutedEventArgs e)
    {
        Settings_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON];
        var _settings = new SettingsView();
        _settings.Show();
    }

    private void Settings_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Settings_Button.Background =
            SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON_DEPRESSED];
    }

    private void Settings_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Settings_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON];
    }

    /// <summary>
    ///     close the player
    /// </summary>
    private void Close_Button_Click(object sender, RoutedEventArgs e)
    {
        Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON];
        Scroller.Cancel();
        Application.Current.Shutdown();
    }

    private void Close_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON_DEPRESSED];
    }

    private void Close_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON];
    }

    /// <summary>
    ///     Show/Hide Playlist
    /// </summary>
    private void Playlist_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_PlaylistView.Visibility == Visibility.Visible)
        {
            _PlaylistView.Visibility = Visibility.Hidden;
            Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON];
        }
        else
        {
            _PlaylistView.Visibility = Visibility.Visible;
            Playlist_Button.Background =
                SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_SELECTED];
        }
    }

    private void Playlist_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Playlist_Button.Background = _PlaylistView.Visibility == Visibility.Visible
            ? SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_DEPRESSED]
            : SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_DEPRESSED_SELECTED];
    }

    private void Playlist_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Playlist_Button.Background = _PlaylistView.Visibility == Visibility.Visible
            ? SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON]
            : SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_SELECTED];
    }

    /// <summary>
    ///     sets the playlist to random or normal play
    /// </summary>
    private void Random_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_PlaylistView.NormalPlay)
        {
            _PlaylistView.NormalPlay = false;
            Random_Button.Background =
                SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED];
        }
        else
        {
            _PlaylistView.NormalPlay = true;
            Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON];
        }
    }

    private void Random_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Random_Button.Background = _PlaylistView.NormalPlay
            ? SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_DEPRESSED]
            : SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED_DEPRESSED];
    }

    private void Random_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Random_Button.Background = _PlaylistView.NormalPlay
            ? SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED]
            : SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON];
    }

    /// <summary>
    ///     Enables the playlist load next after song stopped
    /// </summary>
    private void Loop_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_PlaylistView.LoopPlay)
        {
            _PlaylistView.LoopPlay = false;
            Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON];
        }
        else
        {
            _PlaylistView.LoopPlay = true;
            Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED];
        }
    }

    private void Loop_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Loop_Button.Background = _PlaylistView.LoopPlay
            ? SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_DEPRESSED]
            : SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED_DEPRESSED];
    }

    private void Loop_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Loop_Button.Background = _PlaylistView.LoopPlay
            ? SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON]
            : SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED];
    }
}