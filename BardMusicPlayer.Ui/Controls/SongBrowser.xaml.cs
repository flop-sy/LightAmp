﻿using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Ui.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UI.Resources;

namespace BardMusicPlayer.Ui.Controls
{
    /// <summary>
    /// Interaktionslogik für SongBrowser.xaml
    /// </summary>
    public partial class SongBrowser : UserControl
    {
        public EventHandler<string> OnLoadSongFromBrowser;

        public SongBrowser()
        {
            InitializeComponent();
            SongPath.Text = BmpPigeonhole.Instance.SongDirectory;
        }

        private void SongbrowserContainer_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string filename = SongbrowserContainer.SelectedItem as String;
            if (!File.Exists(filename) || filename == null)
                return;

            OnLoadSongFromBrowser?.Invoke(this, filename);
        }

        private void SongSearch_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Directory.Exists(SongPath.Text))
                return;

            string[] files = Directory.EnumerateFiles(SongPath.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong")).ToArray();
            List<string> list = new List<string>(files);
            if (SongSearch.Text != "")
                list = list.FindAll(delegate (string s) { return s.ToLower().Contains(SongSearch.Text.ToLower()); });
            SongbrowserContainer.ItemsSource = list;
        }

        private void SongPath_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Directory.Exists(SongPath.Text))
                return;

            BmpPigeonhole.Instance.SongDirectory = SongPath.Text + (SongPath.Text.EndsWith("\\") ? "" : "\\"); ;

            string[] files = Directory.EnumerateFiles(SongPath.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong")).ToArray();
            List<string> list = new List<string>(files);
            SongbrowserContainer.ItemsSource = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderPicker();

            if (Directory.Exists(BmpPigeonhole.Instance.SongDirectory))
                dlg.InputPath = Path.GetFullPath(BmpPigeonhole.Instance.SongDirectory);
            else
                dlg.InputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.ResultPath;
                if (!Directory.Exists(path))
                    return;

                path = path + (path.EndsWith("\\") ? "" : "\\");
                SongPath.Text = path;
                BmpPigeonhole.Instance.SongDirectory = path;
                SongSearch_PreviewTextInput(null, null);
            }
        }
    }
}
