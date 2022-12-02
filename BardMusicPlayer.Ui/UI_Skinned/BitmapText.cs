#region

using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using Image = System.Drawing.Image;

#endregion

namespace BardMusicPlayer.Ui.Skinned
{
    public partial class Skinned_MainView : UserControl
    {
        /*void WriteTrackField(string data)
        {
            Bitmap bitmap = new Bitmap(50, 6);
            var graphics = Graphics.FromImage(bitmap);
            int index = 0;
            foreach (var a in data)
            {
                graphics.DrawImage(SkinContainer.FONT[a], 5 * index, 0);
                index++;
            }
            //TrackDigit.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
        }*/

        /// <summary>
        ///     draws the bitmaptext from string into SmallOctaveDigit (the Octave field)
        /// </summary>
        /// <param name="data"></param>
        private void WriteSmallOctaveDigitField(string data)
        {
            var bitmap = new Bitmap(30, 8);
            var graphics = Graphics.FromImage(bitmap);
            var index = 0;
            foreach (var a in data)
            {
                Image img;
                if (SkinContainer.FONT.ContainsKey(a))
                    img = SkinContainer.FONT[a];
                else
                    img = SkinContainer.FONT[32];
                graphics.DrawImage(img, 5 * index, 0);
                index++;
            }

            SmallOctaveDigit.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
            SmallOctaveDigit.Stretch = Stretch.UniformToFill;
        }

        /// <summary>
        ///     draws the bitmaptext from string into WriteSmallDigitField (the Track field)
        /// </summary>
        /// <param name="data"></param>
        private void WriteSmallDigitField(string data)
        {
            data = data.Insert(0, "T");
            data = data.Insert(1, ":");

            var bitmap = new Bitmap(30, 8);
            var graphics = Graphics.FromImage(bitmap);
            var index = 0;
            foreach (var a in data)
            {
                Image img;
                if (SkinContainer.FONT.ContainsKey(a))
                    img = SkinContainer.FONT[a];
                else
                    img = SkinContainer.FONT[32];
                graphics.DrawImage(img, 5 * index, 0);
                index++;
            }

            SmallTrackDigit.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
            SmallTrackDigit.Stretch = Stretch.UniformToFill;
        }

        /// <summary>
        ///     draws the bitmaptext from string into InstrumentDigit
        /// </summary>
        /// <param name="data"></param>
        private void WriteInstrumentDigitField(string data)
        {
            if (data == null)
                return;
            var bitmap = new Bitmap(110, 12);
            var graphics = Graphics.FromImage(bitmap);
            var index = 0;
            foreach (var a in data)
            {
                Image img;
                if (SkinContainer.FONT.ContainsKey(a))
                    img = SkinContainer.FONT[a];
                else
                    img = SkinContainer.FONT[32];
                graphics.DrawImage(img, 5 * index, 0);
                index++;
            }

            InstrumentDigit.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
        }


        #region SongField & Scroller

        private int scrollpos;
        private bool scrolldir = true;

        protected async Task UpdateScroller(CancellationToken stoppingToken, string data)
        {
            var songname = data;
            scrollpos = 0;
            scrolldir = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                WriteSongField(songname);
                await Task.Delay(500, stoppingToken);
            }
        }

        /// <summary>
        ///     draws the bitmaptext from string into SongDigit
        /// </summary>
        /// <param name="data"></param>
        private void WriteSongField(string data)
        {
            var bitmap = new Bitmap(305, 12);
            var graphics = Graphics.FromImage(bitmap);
            for (var i = 0; i < 33; i++)
            {
                Image img;
                var a = ' ';

                if (scrollpos == -1 && !scrolldir)
                {
                    scrolldir = true;
                }
                else
                {
                    if (i + scrollpos >= data.Length)
                    {
                        scrolldir = false;
                    }
                    else
                    {
                        if (scrollpos == -2)
                            scrollpos = 0;
                        a = data.ToArray()[i + scrollpos];
                    }
                }

                if (SkinContainer.FONT.ContainsKey(a))
                    img = SkinContainer.FONT[a];
                else
                    img = SkinContainer.FONT[32];
                graphics.DrawImage(img, 5 * i, 0);
            }

            if (scrolldir)
                scrollpos++;
            else
                scrollpos--;

            SongDigit.Source = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())).ImageSource;
            SongDigit.Stretch = Stretch.UniformToFill;
        }

        #endregion
    }
}