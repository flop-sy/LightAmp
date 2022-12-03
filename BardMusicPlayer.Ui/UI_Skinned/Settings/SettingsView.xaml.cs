#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Utils;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using UI.Resources;

#endregion

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    ///     Interaktionslogik für Settings.xaml
    /// </summary>
    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            ApplySkin();
            SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;

            //Design Tab
            ClassicSkin.IsChecked = BmpPigeonhole.Instance.ClassicUi;

            //Playback Tab
            HoldNotesBox.IsChecked = BmpPigeonhole.Instance.HoldNotes;
            ForcePlaybackBox.IsChecked = BmpPigeonhole.Instance.ForcePlayback;
            MIDI_Input_DeviceBox.Items.Clear();
            MIDI_Input_DeviceBox.ItemsSource = MidiInput.ReloadMidiInputDevices();
            MIDI_Input_DeviceBox.SelectedIndex = BmpPigeonhole.Instance.MidiInputDev + 1;
            AutoPlayBox.IsChecked = BmpPigeonhole.Instance.PlaylistAutoPlay;
            LiveMidiDelay.IsChecked = BmpPigeonhole.Instance.LiveMidiPlayDelay;

            //Local Orchestra Tab
            LocalOrchestraBox.IsChecked = BmpPigeonhole.Instance.LocalOrchestra;
            AutoEquipBox.IsChecked = BmpPigeonhole.Instance.AutoEquipBards;
            KeepTrackSettingsBox.IsChecked = BmpPigeonhole.Instance.EnsembleKeepTrackSetting;

            //Syncsettings Tab
            Autostart_source.SelectedIndex = BmpPigeonhole.Instance.AutostartMethod;
            MidiBardComp.IsChecked = BmpPigeonhole.Instance.MidiBardCompatMode;

            //Misc Tab
            SirenVolume.Value = BmpSiren.Instance.GetVolume();
            AMPInFrontBox.IsChecked = BmpPigeonhole.Instance.BringBMPtoFront;

            //Path Tab
            SongsDir.Text = BmpPigeonhole.Instance.SongDirectory;
            SkinsDir.Text = BmpPigeonhole.Instance.SkinDirectory;

            //Load the skin previews
            if (!Directory.Exists(BmpPigeonhole.Instance.SkinDirectory)) return;

            var files = Directory
                .EnumerateFiles(BmpPigeonhole.Instance.SkinDirectory, "*.wsz", SearchOption.TopDirectoryOnly)
                .ToArray();
            Parallel.ForEach(files, file =>
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var bmap =
                        Skinned_MainView.ExtractBitmapFromZip(file,
                            "Screenshot.png") ?? Skinned_MainView.ExtractBitmapFromZip(
                            file, "MAIN.BMP");
                    SkinPreviewBox.Items.Add(new SkinData { Title = name, ImageData = bmap });
                }));
            });
        }

        /// <summary>
        ///     Helper class to get skin preview working
        /// </summary>
        public sealed class SkinData
        {
            public string Title { get; set; }

            public BitmapImage ImageData { get; set; }
        }

        #region Window design and buttons

        /// <summary>
        ///     Triggered by the SkinLoader, if a new skin was loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
        {
            ApplySkin();
        }

        /// <summary>
        ///     Applies a skin
        /// </summary>
        public void ApplySkin()
        {
            BARDS_TOP_LEFT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_LEFT_CORNER];
            BARDS_TOP_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_TILE];
            BARDS_TOP_RIGHT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_RIGHT_CORNER];

            BARDS_BOTTOM_LEFT_CORNER.Fill =
                SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_LEFT_CORNER];
            BARDS_BOTTOM_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_TILE];
            BARDS_BOTTOM_RIGHT_CORNER.Fill =
                SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_RIGHT_CORNER];

            BARDS_LEFT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_LEFT_TILE];
            BARDS_RIGHT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_RIGHT_TILE];

            Close_Button.Background = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_CLOSE_SELECTED];
            Close_Button.Background.Opacity = 0;
        }

        /// <summary>
        ///     if a mousedown event was triggered by the title bar, dragmove the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        /// <summary>
        ///     The close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     if mouse button down on close button, change bitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Down(object sender, MouseButtonEventArgs e)
        {
            Close_Button.Background.Opacity = 1;
        }

        /// <summary>
        ///     if mouse button up on close button, change bitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Button_Up(object sender, MouseButtonEventArgs e)
        {
            Close_Button.Background.Opacity = 0;
        }

        #endregion

        #region DesignTab controls

        /// <summary>
        ///     The classic skin checkbox action
        /// </summary>
        private void ClassicSkin_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.ClassicUi = ClassicSkin.IsChecked ?? true;
        }

        /// <summary>
        ///     skinpreview doubleclick: change skin
        /// </summary>
        private void SkinPreviewBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SkinPreviewBox.SelectedItem is not SkinData d)
                return;

            var fileName = BmpPigeonhole.Instance.SkinDirectory + d.Title + ".wsz";
            if (Application.Current.MainWindow != null)
                ((Skinned_MainView)Application.Current.MainWindow.DataContext).LoadSkin(fileName);
            BmpPigeonhole.Instance.LastSkin = fileName;
        }

        #endregion

        #region PlaybackTab controls

        private void Hold_Notes_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.HoldNotes = HoldNotesBox.IsChecked ?? false;
        }

        private void Force_Playback_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.ForcePlayback = ForcePlaybackBox.IsChecked ?? false;
        }

        private void MIDI_Input_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = (KeyValuePair<int, string>)MIDI_Input_DeviceBox.SelectedItem;
            BmpPigeonhole.Instance.MidiInputDev = d.Key;
            if (d.Key == -1)
            {
                BmpMaestro.Instance.CloseInputDevice();
                return;
            }

            BmpMaestro.Instance.OpenInputDevice(d.Key);
        }

        private void AutoPlay_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.PlaylistAutoPlay = AutoPlayBox.IsChecked ?? false;
        }

        private void LiveMidiDelay_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.LiveMidiPlayDelay = LiveMidiDelay.IsChecked ?? false;
        }

        #endregion

        #region Local orchestra controls

        private void LocalOrchestraBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.LocalOrchestra = LocalOrchestraBox.IsChecked ?? false;
        }

        private void AutoEquipBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.AutoEquipBards = AutoEquipBox.IsChecked ?? false;
        }

        private void KeepTrackSettingsBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.EnsembleKeepTrackSetting = KeepTrackSettingsBox.IsChecked ?? false;
        }

        #endregion

        #region SyncsettingsTab controls

        private void Autostart_source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = Autostart_source.SelectedIndex;
            BmpPigeonhole.Instance.AutostartMethod = d;
        }

        private void MidiBard_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.MidiBardCompatMode = MidiBardComp.IsChecked ?? false;
        }

        #endregion

        #region Miscsettings Tab controls

        private void SirenVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var g = SirenVolume.Value;
            BmpSiren.Instance.SetVolume((float)g);
        }

        private void AMPInFrontBox_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.BringBMPtoFront = AMPInFrontBox.IsChecked ?? false;
        }

        #endregion

        #region PathTab controls

        private void SongsDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            BmpPigeonhole.Instance.SongDirectory =
                SongsDir.Text + (SongsDir.Text.EndsWith("\\", StringComparison.Ordinal) ? "" : "\\");
        }

        private void SongsDir_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderPicker
            {
                InputPath = Directory.Exists(BmpPigeonhole.Instance.SongDirectory)
                    ? Path.GetFullPath(BmpPigeonhole.Instance.SongDirectory)
                    : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            if (dlg.ShowDialog() != true) return;

            var path = dlg.ResultPath;
            if (!Directory.Exists(path))
                return;

            path += path.EndsWith("\\", StringComparison.Ordinal) ? "" : "\\";
            SongsDir.Text = path;
            BmpPigeonhole.Instance.SongDirectory = path;
        }

        private void SkinsDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            BmpPigeonhole.Instance.SkinDirectory =
                SkinsDir.Text + (SkinsDir.Text.EndsWith("\\", StringComparison.Ordinal) ? "" : "\\");
        }

        private void SkinsDir_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderPicker
            {
                InputPath = Directory.Exists(BmpPigeonhole.Instance.SkinDirectory)
                    ? Path.GetFullPath(BmpPigeonhole.Instance.SkinDirectory)
                    : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            if (dlg.ShowDialog() != true) return;

            var path = dlg.ResultPath;
            if (!Directory.Exists(path))
                return;

            path += path.EndsWith("\\", StringComparison.Ordinal) ? "" : "\\";
            SkinsDir.Text = path;
            BmpPigeonhole.Instance.SkinDirectory = path;
        }

        #endregion
    }
}