﻿#region

using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;

#endregion

namespace BardMusicPlayer.Ui.Controls;

/// <summary>
///     Interaktionslogik für SongEditWindow.xaml
/// </summary>
public sealed partial class SongEditWindow
{
    private readonly BmpSong _song;

    public SongEditWindow(BmpSong song)
    {
        InitializeComponent();
        if (song == null)
        {
            Close();
            return;
        }

        Visibility = Visibility.Visible;

        _song = song;
        Internal_TrackName.Text = _song.Title;
        Displayed_TrackName.Text = _song.DisplayedTitle;
    }

    private void CopyI_D_Click(object sender, RoutedEventArgs e)
    {
        Displayed_TrackName.Text = Internal_TrackName.Text;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _song.DisplayedTitle = Displayed_TrackName.Text;
        BmpCoffer.Instance.SaveSong(_song);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}