#region

using System;
using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Ui.Functions;

#endregion

namespace BardMusicPlayer.Ui.Classic;

/// <summary>
///     Interaktionslogik für Classic_MainView.xaml
/// </summary>
public sealed partial class Classic_MainView
{
    private int MaxTracks = 1;

    //private NetworkPlayWindow _networkWindow = null;
    public Classic_MainView()
    {
        InitializeComponent();
        CurrentInstance = this;

        //Always start with the playlists
        _showingPlaylists = true;
        //Fill the list
        PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
        Playlist_Header.Header = "Playlists";

        SongName.Text = PlaybackFunctions.GetSongName();
        BmpMaestro.Instance.OnPlaybackTimeChanged += Instance_PlaybackTimeChanged;
        BmpMaestro.Instance.OnSongMaxTime += Instance_PlaybackMaxTime;
        BmpMaestro.Instance.OnSongLoaded += Instance_OnSongLoaded;
        BmpMaestro.Instance.OnPlaybackStarted += Instance_PlaybackStarted;
        BmpMaestro.Instance.OnPlaybackStopped += Instance_PlaybackStopped;
        BmpMaestro.Instance.OnTrackNumberChanged += Instance_TrackNumberChanged;
        BmpMaestro.Instance.OnOctaveShiftChanged += Instance_OctaveShiftChanged;
        BmpMaestro.Instance.OnSpeedChanged += Instance_OnSpeedChange;
        BmpSeer.Instance.ChatLog += Instance_ChatLog;

        Siren_Volume.Value = BmpSiren.Instance.GetVolume();
        BmpSiren.Instance.SynthTimePositionChanged += Instance_SynthTimePositionChanged;

        SongBrowser.OnLoadSongFromBrowser += Instance_SongBrowserLoadedSong;

        BmpSeer.Instance.MidibardPlaylistEvent += Instance_MidibardPlaylistEvent;

        Globals.Globals.OnConfigReload += Globals_OnConfigReload;
        LoadConfig();
    }

    public static Classic_MainView CurrentInstance { get; private set; }

    private bool _directLoaded { get; set; } //indicates if a song was loaded directly or from playlist

    private void Globals_OnConfigReload(object sender, EventArgs e)
    {
        LoadConfig(true);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        KeyHeat.InitUi();
    }


    // private void Info_Button_Click(object sender, RoutedEventArgs e)
    // {
    // InfoBox _infoBox = new InfoBox();
    // _infoBox.Show();
    // }

    // private void Info_Button_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    // {
    // /*if (_networkWindow == null)
    // _networkWindow = new NetworkPlayWindow();
    // _networkWindow.Visibility = Visibility.Visible;*/

    // MacroLaunchpad macroLaunchpad = new MacroLaunchpad();
    // macroLaunchpad.Visibility = Visibility.Visible;
    // }

    /// <summary>
    ///     triggered by the songbrowser if a file should be loaded
    /// </summary>
    private void Instance_SongBrowserLoadedSong(object sender, string filename)
    {
        if (!PlaybackFunctions.LoadSong(filename)) return;

        SongName.Text = PlaybackFunctions.GetSongName();
        InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        _directLoaded = true;
    }

    #region EventHandler

    private void Instance_PlaybackTimeChanged(object sender, CurrentPlayPositionEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => PlaybackTimeChanged(e)));
    }

    private void Instance_PlaybackMaxTime(object sender, MaxPlayTimeEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => PlaybackMaxTime(e)));
    }

    private void Instance_OnSongLoaded(object sender, SongLoadedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => OnSongLoaded(e)));
    }

    private void Instance_PlaybackStarted(object sender, bool e)
    {
        Dispatcher.BeginInvoke(new Action(PlaybackStarted));
    }

    private void Instance_PlaybackStopped(object sender, bool e)
    {
        Dispatcher.BeginInvoke(new Action(PlaybackStopped));
    }

    private void Instance_TrackNumberChanged(object sender, TrackNumberChangedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => TracknumberChanged(e)));
    }

    private void Instance_OctaveShiftChanged(object sender, OctaveShiftChangedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => OctaveShiftChanged(e)));
    }

    private void Instance_OnSpeedChange(object sender, SpeedShiftEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => SpeedShiftChange(e)));
    }

    private void Instance_ChatLog(ChatLog seerEvent)
    {
        Dispatcher.BeginInvoke(new Action(() => AppendChatLog(seerEvent)));
    }

    private void Instance_MidibardPlaylistEvent(MidibardPlaylistEvent seerEvent)
    {
        Dispatcher.BeginInvoke(new Action(() => SelectSongByIndex(seerEvent.Song)));
    }

    private void Instance_SynthTimePositionChanged(string songTitle, double currentTime, double endTime,
        int activeVoices)
    {
        Dispatcher.BeginInvoke(new Action(() => Siren_PlaybackTimeChanged(currentTime, endTime)));
    }

    private void PlaybackTimeChanged(CurrentPlayPositionEvent e)
    {
        var Seconds = e.timeSpan.Seconds.ToString();
        var Minutes = e.timeSpan.Minutes.ToString();
        var time = (Minutes.Length == 1 ? "0" + Minutes : Minutes) + ":" +
                   (Seconds.Length == 1 ? "0" + Seconds : Seconds);
        ElapsedTime.Content = time;

        if (!_Playbar_dragStarted) Playbar_Slider.Value = e.tick;
    }

    private void PlaybackMaxTime(MaxPlayTimeEvent e)
    {
        var Seconds = e.timeSpan.Seconds.ToString();
        var Minutes = e.timeSpan.Minutes.ToString();
        var time = (Minutes.Length == 1 ? "0" + Minutes : Minutes) + ":" +
                   (Seconds.Length == 1 ? "0" + Seconds : Seconds);
        TotalTime.Content = time;

        Playbar_Slider.Maximum = e.tick;
    }

    private void OnSongLoaded(SongLoadedEvent e)
    {
        //Statistics update
        UpdateStats(e);
        //update heatmap
        KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
        SpeedNumValue = 1.0f;
        if (PlaybackFunctions.PlaybackState != PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
            Play_Button_State();

        MaxTracks = e.MaxTracks;
        if (NumValue <= MaxTracks)
            return;

        NumValue = MaxTracks;

        BmpMaestro.Instance.SetTracknumberOnHost(MaxTracks);
    }

    public void PlaybackStarted()
    {
        PlaybackFunctions.PlaybackState = PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING;
        Play_Button_State(true);
    }

    public void PlaybackStopped()
    {
        PlaybackFunctions.StopSong();
        Play_Button_State();

        //if this wasn't a song from the playlist, do nothing
        if (_directLoaded)
            return;

        if (!BmpPigeonhole.Instance.PlaylistAutoPlay) return;

        playNextSong();
        var rnd = new Random();
        PlaybackFunctions.PlaySong(rnd.Next(15, 35) * 100);
        Play_Button_State(true);
    }

    public void TracknumberChanged(TrackNumberChangedEvent e)
    {
        if (!e.IsHost) return;

        NumValue = e.TrackNumber;
        UpdateNoteCountForTrack();
    }

    public void OctaveShiftChanged(OctaveShiftChangedEvent e)
    {
        if (e.IsHost) OctaveNumValue = e.OctaveShift;
    }

    public void SpeedShiftChange(SpeedShiftEvent e)
    {
        if (e.IsHost) SpeedNumValue = e.SpeedShift;
    }

    public void AppendChatLog(ChatLog ev)
    {
        if (BmpMaestro.Instance.GetHostPid() == ev.ChatLogGame.Pid)
        {
            ChatBox.AppendText(ev);
            ChatBox.ScrollToEnd();
        }

        if (ev.ChatLogCode != "0039") return;

        if (!ev.ChatLogLine.Contains(@"Anzählen beginnt") &&
            !ev.ChatLogLine.Contains("The count-in will now commence.") &&
            !ev.ChatLogLine.Contains("orchestre est pr")) return;
        if (BmpPigeonhole.Instance.AutostartMethod != (int)Globals.Globals.Autostart_Types.VIA_CHAT)
            return;
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
            return;

        PlaybackFunctions.PlaySong(3000);
        Play_Button_State(true);
    }

    #endregion

    #region Track UP/Down

    private int _numValue = 1;

    public int NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
            track_txtNum.Text = "t" + value;

            //update heatmap
            KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }
    }

    private void track_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue == MaxTracks)
            return;

        NumValue++;
        BmpMaestro.Instance.SetTracknumberOnHost(NumValue);
    }

    private void track_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue == 1)
            return;

        NumValue--;
        BmpMaestro.Instance.SetTracknumberOnHost(NumValue);
    }

    private void track_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (track_txtNum == null)
            return;

        if (!int.TryParse(track_txtNum.Text.Replace("t", ""), out _numValue)) return;
        if (_numValue < 0 || _numValue > MaxTracks)
            return;

        track_txtNum.Text = "t" + _numValue;
        BmpMaestro.Instance.SetTracknumberOnHost(_numValue);
    }

    #endregion

    #region Octave UP/Down

    private int _octavenumValue = 1;

    public int OctaveNumValue
    {
        get => _octavenumValue;
        set
        {
            _octavenumValue = value;
            octave_txtNum.Text = @"ø" + value;
            KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
        }
    }

    private void octave_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        OctaveNumValue++;
        BmpMaestro.Instance.SetOctaveshiftOnHost(OctaveNumValue);
    }

    private void octave_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        OctaveNumValue--;
        BmpMaestro.Instance.SetOctaveshiftOnHost(OctaveNumValue);
    }

    private void octave_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (octave_txtNum == null)
            return;

        if (!int.TryParse(octave_txtNum.Text.Replace(@"ø", ""), out _octavenumValue)) return;

        octave_txtNum.Text = @"ø" + _octavenumValue;
        BmpMaestro.Instance.SetOctaveshiftOnHost(_octavenumValue);
    }

    #endregion

    #region Speed shift

    private float _speedNumValue = 1.0f;

    public float SpeedNumValue
    {
        get => _speedNumValue;
        set
        {
            _speedNumValue = value;
            speed_txtNum.Text = value * 100 + "%";
        }
    }

    private void speed_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (speed_txtNum == null)
            return;

        if (!int.TryParse(speed_txtNum.Text.Replace(@"%", ""), out var t)) return;

        var speedShift = (Convert.ToDouble(t) / 100).Clamp(0.1f, 2.0f);
        BmpMaestro.Instance.SetSpeedShiftOnHost((float)speedShift);
    }

    private void speed_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        var speedShift = SpeedNumValue + 0.01;
        BmpMaestro.Instance.SetSpeedShiftOnHost((float)speedShift);
    }

    private void speed_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        var speedShift = SpeedNumValue - 0.01;
        BmpMaestro.Instance.SetSpeedShiftOnHost((float)speedShift);
    }

    #endregion
}