#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using BardMusicPlayer.Pigeonhole;
using UI.Resources;

#endregion

namespace BardMusicPlayer.Ui.Controls
{
    /// <summary>
    ///     The songbrowser but much faster than the BMP 1.x had
    /// </summary>
    public sealed partial class SongBrowser
    {
        public EventHandler<string> OnLoadSongFromBrowser;

        public SongBrowser()
        {
            InitializeComponent();
            SongPath.Text = BmpPigeonhole.Instance.SongDirectory;
        }

        /// <summary>
        ///     Load the doubleclicked song into the sequencer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongbrowserContainer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var filename = SongbrowserContainer.SelectedItem as string;
            if (!File.Exists(filename) || filename == null)
                return;

            OnLoadSongFromBrowser?.Invoke(this, filename);
        }

        /// <summary>
        ///     Sets the search parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongSearch_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Directory.Exists(SongPath.Text))
                return;

            var files = Directory.EnumerateFiles(SongPath.Text, "*.*", SearchOption.AllDirectories)
                .Where(static s => s.EndsWith(".mid", StringComparison.Ordinal) ||
                                   s.EndsWith(".mml", StringComparison.Ordinal) ||
                                   s.EndsWith(".mmsong", StringComparison.Ordinal)).ToArray();
            var list = new List<string>(files);
            if (SongSearch.Text != "")
                list = list.FindAll(delegate(string s) { return s.ToLower().Contains(SongSearch.Text.ToLower()); });

            SongbrowserContainer.ItemsSource = list;
        }

        /// <summary>
        ///     Sets the songs folder path by typing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongPath_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Directory.Exists(SongPath.Text))
                return;

            BmpPigeonhole.Instance.SongDirectory =
                SongPath.Text + (SongPath.Text.EndsWith("\\", StringComparison.Ordinal) ? "" : "\\");

            var files = Directory.EnumerateFiles(SongPath.Text, "*.*", SearchOption.AllDirectories)
                .Where(static s => s.EndsWith(".mid", StringComparison.Ordinal) ||
                                   s.EndsWith(".mml", StringComparison.Ordinal) ||
                                   s.EndsWith(".mmsong", StringComparison.Ordinal)).ToArray();
            var list = new List<string>(files);
            SongbrowserContainer.ItemsSource = list;
        }

        /// <summary>
        ///     Sets the songs folder path by folderselection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
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
            SongPath.Text = path;
            BmpPigeonhole.Instance.SongDirectory = path;
            SongSearch_PreviewTextInput(null, null);
        }
    }
}