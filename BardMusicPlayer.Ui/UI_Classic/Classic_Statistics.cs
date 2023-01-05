#region

using System.Collections.Generic;
using System.IO;
using System.Windows;
using BardMusicPlayer.Maestro.Events;
// using BardMusicPlayer.MidiUtil.Managers;
using BardMusicPlayer.Ui.Functions;
using Microsoft.Win32;

#endregion

namespace BardMusicPlayer.Ui.Classic;

/// <summary>
///     only here cuz someone would like to have it back
/// </summary>
public sealed partial class Classic_MainView
{
    private List<int> _notesCountForTracks = new();

    private void UpdateStats(SongLoadedEvent e)
    {
        Statistics_Total_Tracks_Label.Content = e.MaxTracks.ToString();
        Statistics_Total_Note_Count_Label.Content = e.TotalNoteCount.ToString();

        _notesCountForTracks.Clear();
        _notesCountForTracks = e.CurrentNoteCountForTracks;

        if (NumValue >= _notesCountForTracks.Count)
        {
            Statistics_Track_Note_Count_Label.Content = "Invalid track";
            return;
        }

        Statistics_Track_Note_Count_Label.Content = _notesCountForTracks[NumValue];
    }

    private void UpdateNoteCountForTrack()
    {
        if (PlaybackFunctions.CurrentSong == null)
            return;

        if (NumValue >= _notesCountForTracks.Count)
        {
            Statistics_Track_Note_Count_Label.Content = "Invalid track";
            return;
        }

        Statistics_Track_Note_Count_Label.Content = _notesCountForTracks[NumValue];
    }

    private void ExportAsMidi(object sender, RoutedEventArgs e)
    {
        var song = PlaybackFunctions.CurrentSong;
        Stream myStream;
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "MIDI file (*.mid)|*.mid",
            FilterIndex = 2,
            RestoreDirectory = true,
            OverwritePrompt = true
        };

        if (saveFileDialog.ShowDialog() != true) return;

        if ((myStream = saveFileDialog.OpenFile()) == null) return;

        song.GetExportMidi().WriteTo(myStream);
        myStream.Close();
    }

    // private void MidiProcessing_Click(object sender, RoutedEventArgs e)
    // {
    //     MidiUtil.MidiUtil.Instance.Start();
    //     //UiManager.Instance.mainWindow = new MidiEditWindow();
    //     if (PlaybackFunctions.CurrentSong != null)
    //         MidiManager.Instance.OpenFile(PlaybackFunctions.CurrentSong.GetExportMidi());
    // }
}