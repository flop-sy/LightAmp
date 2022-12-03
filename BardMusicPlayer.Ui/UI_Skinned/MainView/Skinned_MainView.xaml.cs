#region

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Ui.Globals.SkinContainer;

#endregion

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    ///     Interaktionslogik für Skinned_MainView.xaml
    /// </summary>
    public sealed partial class Skinned_MainView
    {
        public BardsWindow _BardListView;
        public Skinned_MainView_Ex _MainView_Ex;

        private TimeSpan _maxTime;
        public NetworkPlayWindow _Networkplaywindow;
        private bool _Playbar_dragStarted;

        public Skinned_PlaylistView _PlaylistView;
        public SongbrowserWindow _SongbrowserView;

        private CancellationTokenSource Scroller = new();

        public Skinned_MainView()
        {
            InitializeComponent();
            LoadSkin(BmpPigeonhole.Instance.LastSkin);

            _MainView_Ex = new Skinned_MainView_Ex();
            if (BmpPigeonhole.Instance.SkinnedUi_UseExtendedView) _MainView_Ex.Visibility = Visibility.Visible;

            //init the songbrowser
            _SongbrowserView = new SongbrowserWindow();
            _SongbrowserView.Show();
            _SongbrowserView.Visibility = Visibility.Hidden;
            _SongbrowserView.OnLoadSongFromBrowser += OnLoadSongFromSongbrowser;


            //open the bards window
            _BardListView = new BardsWindow();
            _BardListView.Show();
            _BardListView.Visibility = Visibility.Hidden;

            _Networkplaywindow = new NetworkPlayWindow();
            _Networkplaywindow.Show();
            _Networkplaywindow.Visibility = Visibility.Hidden;

            //open the playlist and bind the event
            _PlaylistView = new Skinned_PlaylistView();
            _PlaylistView.Show();
            _PlaylistView.Top = ((MainWindow)Application.Current.MainWindow).Top +
                                ((MainWindow)Application.Current.MainWindow).ActualHeight;
            _PlaylistView.Left = ((MainWindow)Application.Current.MainWindow).Left;
            _PlaylistView.OnLoadSongFromPlaylist += OnLoadSongFromPlaylist;

            //bind events from maestro
            BmpMaestro.Instance.OnSongLoaded += Instance_OnSongLoaded;
            BmpMaestro.Instance.OnSongMaxTime += Instance_PlaybackMaxTime;
            BmpMaestro.Instance.OnPlaybackTimeChanged += Instance_PlaybackTimeChanged;
            BmpMaestro.Instance.OnTrackNumberChanged += Instance_TrackNumberChanged;
            BmpMaestro.Instance.OnOctaveShiftChanged += Instance_OctaveShiftChanged;
            BmpMaestro.Instance.OnPlaybackStarted += Instance_PlaybackStarted;
            BmpMaestro.Instance.OnPlaybackStopped += Instance_PlaybackStopped;

            //same for seer
            BmpSeer.Instance.ChatLog += Instance_ChatLog;

            //Set the *bar params
            Trackbar_Slider.Maximum = 8;
            Trackbar_Slider.Value = 1;
            Octavebar_Slider.Maximum = 8;
            Octavebar_Slider.Minimum = 0;
            Octavebar_Slider.Value = 4;

            //if we have selected all tracks in the config use it
            if (BmpPigeonhole.Instance.PlayAllTracks)
                BmpMaestro.Instance.SetTracknumberOnHost(0);

            var track = BmpMaestro.Instance.GetHostBardTrack();
            WriteSmallDigitField(track.ToString());
        }

        private int MaxTracks { get; set; } = 1;
        private bool _Trackbar_dragStarted { get; set; }
        private bool _Octavebar_dragStarted { get; set; }
        private bool _showLapTime { get; set; } = true;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowPositions();
        }


        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (Application.Current.MainWindow == null) return;

            var oLeft = Application.Current.MainWindow.Left;
            var oTop = Application.Current.MainWindow.Top;

            ((MainWindow)Application.Current.MainWindow).DragMove();
            oLeft = Application.Current.MainWindow.Left - oLeft;
            oTop = Application.Current.MainWindow.Top - oTop;


            if (_MainView_Ex.Visibility == Visibility.Visible)
            {
                var mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Width = mainWindow.Width;
                _MainView_Ex.Top = mainWindow.Top + mainWindow.Height;
                _MainView_Ex.Left = mainWindow.Left;

                _PlaylistView.Width = mainWindow.Width;
                _PlaylistView.Left += oLeft;
                _PlaylistView.Top += oTop;
                mainWindow = null;
            }
            else
            {
                var mainWindow = Application.Current.MainWindow;
                _PlaylistView.Left += oLeft;
                _PlaylistView.Top += oTop;
            }
        }

        private void MainLostFocus(object sender, EventArgs e)
        {
            //TitleBar.Fill = _titlebar_image[1];
        }


        private void Playbar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void Playbar_Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _Playbar_dragStarted = true;
        }

        private void Playbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            BmpMaestro.Instance.SetPlaybackStart((int)Playbar_Slider.Value);
            _Playbar_dragStarted = false;
        }

        private void DisplayPlayTime(TimeSpan t)
        {
            //Display the lap time or the remaining time
            if (!_showLapTime) t = _maxTime - t;

            var Seconds = t.ToString().Split(':')[2];
            Second_Last.Dispatcher.BeginInvoke(new Action(() =>
            {
                Second_Last.Fill =
                    SkinContainer.NUMBERS[
                        (SkinContainer.NUMBER_TYPES)(Seconds.Length == 1
                            ? Convert.ToInt32(Seconds)
                            : Convert.ToInt32(Seconds.Substring(1, 1)))];
            }));
            Second_First.Dispatcher.BeginInvoke(new Action(() =>
            {
                Second_First.Fill =
                    SkinContainer.NUMBERS[
                        (SkinContainer.NUMBER_TYPES)(Seconds.Length == 1
                            ? 0
                            : Convert.ToInt32(Seconds.Substring(0, 1)))];
            }));

            var Minutes = t.ToString().Split(':')[1];
            Minutes_Last.Dispatcher.BeginInvoke(new Action(() =>
            {
                Minutes_Last.Fill =
                    SkinContainer.NUMBERS[
                        (SkinContainer.NUMBER_TYPES)(Minutes.Length == 1
                            ? Convert.ToInt32(Minutes)
                            : Convert.ToInt32(Minutes.Substring(1, 1)))];
            }));
            Minutes_First.Dispatcher.BeginInvoke(new Action(() =>
            {
                Minutes_First.Fill =
                    SkinContainer.NUMBERS[
                        (SkinContainer.NUMBER_TYPES)(Minutes.Length == 1
                            ? 0
                            : Convert.ToInt32(Minutes.Substring(0, 1)))];
            }));
        }

        private void ShowSongBrowserWindow_Click(object sender, RoutedEventArgs e)
        {
            _SongbrowserView.Visibility = Visibility.Visible;
        }

        private void ShowBardsWindow_Click(object sender, RoutedEventArgs e)
        {
            _BardListView.Visibility = Visibility.Visible;
        }

        private void ShowNetworkWindow_Click(object sender, RoutedEventArgs e)
        {
            _Networkplaywindow.Visibility = Visibility.Visible;
        }

        private void ShowPlaylistWindow_Click(object sender, RoutedEventArgs e)
        {
            _PlaylistView.Visibility = Visibility.Visible;
        }

        private void ShowExtendedView_Click(object sender, RoutedEventArgs e)
        {
            if (_MainView_Ex.IsVisible)
            {
                _MainView_Ex.Visibility = Visibility.Hidden;
            }
            else
            {
                _MainView_Ex.Visibility = Visibility.Visible;
                var mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Width = mainWindow.Width;
                _MainView_Ex.Top = mainWindow.Top + mainWindow.Height;
                _MainView_Ex.Left = mainWindow.Left;
            }

            BmpPigeonhole.Instance.SkinnedUi_UseExtendedView =
                _MainView_Ex.Visibility == Visibility.Visible;
        }

        /// <summary>
        ///     Set the exview and playlist position to the main window
        /// </summary>
        private void SetWindowPositions()
        {
            if (_MainView_Ex.Visibility == Visibility.Visible)
            {
                var mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Top = mainWindow.Top + mainWindow.Height;
                _MainView_Ex.Left = mainWindow.Left;

                _PlaylistView.Top = mainWindow.Top + mainWindow.Height + 172;
                _PlaylistView.Left = mainWindow.Left;
                mainWindow = null;
            }
            else
            {
                var mainWindow = Application.Current.MainWindow;
                _PlaylistView.Top = mainWindow.Top + mainWindow.Height;
                _PlaylistView.Left = mainWindow.Left;
            }
        }

        /// <summary>
        ///     Set the exview and playlist position to the main window
        /// </summary>
        private void ResethWindowPositions_Click(object sender, RoutedEventArgs e)
        {
            if (_MainView_Ex.Visibility == Visibility.Visible)
            {
                var mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Width = mainWindow.Width;
                _MainView_Ex.Top = mainWindow.Top + mainWindow.Height;
                _MainView_Ex.Left = mainWindow.Left;

                _PlaylistView.Width = mainWindow.Width;
                _PlaylistView.Top = mainWindow.Top + mainWindow.Height + 172;
                _PlaylistView.Left = mainWindow.Left;
                mainWindow = null;
            }
            else
            {
                var mainWindow = Application.Current.MainWindow;
                _PlaylistView.Width = mainWindow.Width;
                _PlaylistView.Top = mainWindow.Top + mainWindow.Height;
                _PlaylistView.Left = mainWindow.Left;
            }
        }

        /// <summary>
        ///     when clicked on the time digits, toggle between lap and remaining time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Time_Display_Clicked(object sender, MouseButtonEventArgs e)
        {
            _showLapTime = !_showLapTime;
        }

        #region EventCallbacks

        /// <summary>
        ///     triggered by the songbrowser if a file should be loaded
        /// </summary>
        private void OnLoadSongFromSongbrowser(object sender, string filename)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!PlaybackFunctions.LoadSong(filename)) return;

                Scroller.Cancel();
                Scroller = new CancellationTokenSource();
                UpdateScroller(Scroller.Token, PlaybackFunctions.GetSongName()).ConfigureAwait(false);
                WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());
            }));
        }

        /// <summary>
        ///     called from playlist if a song should be loaded
        /// </summary>
        private void OnLoadSongFromPlaylist(object sender, BmpSong e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //Cancel and rebuild the scroller
                Scroller.Cancel();
                Scroller = new CancellationTokenSource();
                UpdateScroller(Scroller.Token, PlaybackFunctions.GetSongName()).ConfigureAwait(false);
                WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());

                //if playlist is on autoplay, play next song
                if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYNEXT)
                    PlaybackFunctions.PlaySong(0);
            }));
        }

        private void Instance_OnSongLoaded(object sender, SongLoadedEvent e)
        {
            Dispatcher.BeginInvoke(new Action(() => OnSongLoaded(e)));
        }

        private void Instance_PlaybackMaxTime(object sender, MaxPlayTimeEvent e)
        {
            Dispatcher.BeginInvoke(new Action(() => PlaybackMaxTime(e)));
        }

        private void Instance_PlaybackTimeChanged(object sender, CurrentPlayPositionEvent e)
        {
            Dispatcher.BeginInvoke(new Action(() => PlaybackTimeChanged(e)));
        }

        private void Instance_TrackNumberChanged(object sender, TrackNumberChangedEvent e)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateTrackNumberAndInstrument(e)));
        }

        private void Instance_OctaveShiftChanged(object sender, OctaveShiftChangedEvent e)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateOctaveShift(e)));
        }

        private void Instance_PlaybackStarted(object sender, bool e)
        {
            Dispatcher.BeginInvoke(new Action(OnSongStarted));
        }

        private void Instance_PlaybackStopped(object sender, bool e)
        {
            Dispatcher.BeginInvoke(new Action(OnSongStopped));
        }

        private void Instance_ChatLog(ChatLog seerEvent)
        {
            Dispatcher.BeginInvoke(new Action(() => AppendChatLog(seerEvent.ChatLogCode, seerEvent.ChatLogLine)));
        }

        /// <summary>
        ///     triggered if a song was loaded into maestro
        /// </summary>
        private void OnSongLoaded(SongLoadedEvent e)
        {
            MaxTracks = e.MaxTracks;
            if (Trackbar_Slider.Value > MaxTracks) Trackbar_Slider.Value = MaxTracks;

            if (BmpMaestro.Instance.GetHostBardTrack() <= MaxTracks)
                return;

            BmpMaestro.Instance.SetTracknumberOnHost(MaxTracks);
        }

        /// <summary>
        ///     triggered if we know the max time from meastro
        /// </summary>
        private void PlaybackMaxTime(MaxPlayTimeEvent e)
        {
            DisplayPlayTime(e.timeSpan);
            Playbar_Slider.Dispatcher.BeginInvoke(new Action(() => { Playbar_Slider.Maximum = e.tick; }));
            _maxTime = e.timeSpan;
        }

        /// <summary>
        ///     triggered via maestro
        /// </summary>
        private void PlaybackTimeChanged(CurrentPlayPositionEvent e)
        {
            if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
                DisplayPlayTime(e.timeSpan);
            if (!_Playbar_dragStarted)
                Playbar_Slider.Dispatcher.BeginInvoke(new Action(() => { Playbar_Slider.Value = e.tick; }));
        }

        /// <summary>
        ///     update the track and instrument if a track was changed
        /// </summary>
        private void UpdateTrackNumberAndInstrument(TrackNumberChangedEvent e)
        {
            if (!e.IsHost)
                return;

            var track = BmpMaestro.Instance.GetHostBardTrack();
            Trackbar_Slider.Value = track;
            WriteSmallDigitField(e.TrackNumber.ToString());
            WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());
        }

        /// <summary>
        ///     update the octaveshift slider and displayed value
        /// </summary>
        private void UpdateOctaveShift(OctaveShiftChangedEvent e)
        {
            if (!e.IsHost)
                return;

            Octavebar_Slider.Value = e.OctaveShift + 4;
        }

        /// <summary>
        ///     triggered if playback was started
        /// </summary>
        private static void OnSongStarted()
        {
            PlaybackFunctions.PlaybackState = PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING;
        }

        /// <summary>
        ///     triggered if playback was stopped
        /// </summary>
        private void OnSongStopped()
        {
            if (_PlaylistView.LoopPlay)
                _PlaylistView.PlayNextSong();
            else
                PlaybackFunctions.StopSong();
        }

        /// <summary>
        ///     triggered if a chatmsg is comming
        /// </summary>
        /// <param name="code"></param>
        /// <param name="line"></param>
        public static void AppendChatLog(string code, string line)
        {
            //The old autostart method with the chat
            if (code != "0039") return;

            if (!line.Contains(@"Anzählen beginnt") &&
                !line.Contains("The count-in will now commence.") &&
                !line.Contains("orchestre est pr")) return;
            if (BmpPigeonhole.Instance.AutostartMethod != (int)Globals.Globals.Autostart_Types.VIA_CHAT)
                return;

            if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
                return;

            PlaybackFunctions.PlaySong(3000);
        }

        #endregion
    }
}