#region

using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using Microsoft.Win32;

#endregion

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    ///     logic for the siren controls
    /// </summary>
    public partial class Skinned_PlaylistView : Window
    {
        private double lasttime; //last poll time of Instance_SynthTimePositionChanged
        private int scrollpos; //position of the title scroller
        public int SirenCurrentSongIndex { get; set; } //index of the currentSong for siren

        /// <summary>
        ///     Triggered from Siren, it's the title scroller and time update function
        /// </summary>
        private void Instance_SynthTimePositionChanged(string songTitle, double currentTime, double endTime,
            int activeVoices)
        {
            //if we are finished, stop the playback
            if (currentTime >= endTime)
                BmpSiren.Instance.Stop();

            //Scrolling
            if (lasttime + 500 < currentTime)
            {
                Dispatcher.BeginInvoke(new Action(() => WriteSongTitle(songTitle)));
                Dispatcher.BeginInvoke(new Action(() => WriteSongTime(currentTime)));
                lasttime = currentTime;
            }
        }

        #region Scroller

        /// <summary>
        ///     Writes the song title in the lower right corner
        /// </summary>
        /// <param name="data"></param>
        private void WriteSongTitle(string data)
        {
            var bitmap = new Bitmap(305, 12);
            var graphics = Graphics.FromImage(bitmap);
            for (var i = 0; i < 20; i++)
            {
                Image img;
                var a = ' ';
                if (i + scrollpos >= data.Length)
                {
                    if (i + scrollpos >= data.Length + 10)
                    {
                        scrollpos = 0;
                        return;
                    }
                }
                else
                {
                    a = data.ToArray()[i + scrollpos];
                }

                if (SkinContainer.FONT.ContainsKey(a))
                    img = SkinContainer.FONT[a];
                else
                    img = SkinContainer.FONT[32];
                graphics.DrawImage(img, 5 * i, 0);
            }

            scrollpos++;
            SongDigit.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
            SongDigit.Stretch = Stretch.UniformToFill;
        }

        /// <summary>
        ///     write the current playtime from siren in the lower right corner
        /// </summary>
        /// <param name="data">time in ms</param>
        private void WriteSongTime(double data)
        {
            var bitmap = new Bitmap(305, 12);
            var graphics = Graphics.FromImage(bitmap);

            var t = TimeSpan.FromMilliseconds(data);

            var Seconds = t.Seconds.ToString();
            var Minutes = t.Minutes.ToString();
            Image img;
            img = SkinContainer.FONT[Minutes.Length == 1 ? '0' : Minutes.ToArray()[0]];
            graphics.DrawImage(img, 5 * 0, 0);
            img = SkinContainer.FONT[Minutes.Length == 1 ? Minutes.ToArray()[0] : Minutes.ToArray()[1]];
            graphics.DrawImage(img, 5 * 1, 0);
            img = SkinContainer.FONT[58];
            graphics.DrawImage(img, 5 * 2, 0);
            img = SkinContainer.FONT[Seconds.Length == 1 ? '0' : Seconds.ToArray()[0]];
            graphics.DrawImage(img, 5 * 3, 0);
            img = SkinContainer.FONT[Seconds.Length == 1 ? Seconds.ToArray()[0] : Seconds.ToArray()[1]];
            graphics.DrawImage(img, 5 * 4, 0);


            SongTime.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
            SongTime.Stretch = Stretch.UniformToFill;
        }

        #endregion

        #region SirenButtons

        /// <summary>
        ///     selects the previous song and load it into siren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrevButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SirenCurrentSongIndex <= 0)
                return;

            SirenCurrentSongIndex--;
            var t = PlaylistContainer.Items[SirenCurrentSongIndex] as string;
            var song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, t);
            if (song == null)
                return;
            scrollpos = 0;
            lasttime = 0;
            WriteSongTitle(song.Title);
            _ = BmpSiren.Instance.Load(song);
        }

        /// <summary>
        ///     plays the loaded siren song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Playbutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BmpSiren.Instance.IsReadyForPlayback)
                _ = BmpSiren.Instance.Play();
        }

        /// <summary>
        ///     pause the loaded siren song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _ = BmpSiren.Instance.Pause();
        }

        /// <summary>
        ///     stops the siren playback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _ = BmpSiren.Instance.Stop();
        }

        /// <summary>
        ///     load the selected song into siren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist,
                PlaylistContainer.SelectedItem as string);
            if (song == null)
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = Globals.Globals.FileFilters,
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() != true)
                    return;

                if (!openFileDialog.CheckFileExists)
                    return;

                song = BmpSong.OpenFile(openFileDialog.FileName).Result;
                if (song == null)
                    return;
            }

            scrollpos = 0;
            lasttime = 0;
            WriteSongTitle(song.Title);
            _ = BmpSiren.Instance.Load(song);
        }

        /// <summary>
        ///     load next song in siren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SirenCurrentSongIndex == PlaylistContainer.Items.Count - 1)
                return;

            SirenCurrentSongIndex++;
            var t = PlaylistContainer.Items[SirenCurrentSongIndex] as string;
            var song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, t);
            if (song == null)
                return;
            scrollpos = 0;
            lasttime = 0;
            WriteSongTitle(song.Title);
            _ = BmpSiren.Instance.Load(song);
        }

        #endregion
    }
}