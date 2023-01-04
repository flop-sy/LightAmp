#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Win32;
using Newtonsoft.Json;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Managers;

public abstract class DMaps
{
    public int MidiNote { get; set; } = 0;
    public string Instrument { get; set; } = "None";
    public int GameNote { get; set; } = 0;
}

public partial class MidiManager
{
    #region public accessor

    /// <summary>
    ///     accesor for the drum mapper
    /// </summary>
    /// <param name="selectedTrack"></param>
    public void Drummapping(int selectedTrack)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Drum map | *.json",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var memoryStream = new MemoryStream();
        var fileStream = File.Open(openFileDialog.FileName, FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        var data = memoryStream.ToArray();
        var drumlist = JsonConvert.DeserializeObject<List<DMaps>>(new UTF8Encoding(true).GetString(data));
        memoryStream.Close();
        memoryStream.Dispose();

        var midiFile = GetMelanchallMidiFile();
        midiFile = Drummapping(midiFile, selectedTrack, drumlist);
        melanchallToSequencer(midiFile);
    }

    /// <summary>
    ///     Transpose a track x halftones
    /// </summary>
    /// <param name="selectedTrack"></param>
    /// <param name="halftones"></param>
    public void Transpose(int selectedTrack, int halftones)
    {
        var midiFile = GetMelanchallMidiFile();
        midiFile = Transpose(midiFile, selectedTrack, halftones);
        melanchallToSequencer(midiFile);
    }

    #endregion


    #region private MelanchallFunctions

    private static MidiFile AutoSetChannelsForAllTracks(MidiFile midiFile)
    {
        var idx = 0;
        foreach (var tc in midiFile.GetTrackChunks())
        {
            using (var notesManager = tc.ManageNotes())
            {
                Parallel.ForEach(notesManager.Notes, note => { note.Channel = (FourBitNumber)idx; });
            }

            using (var manager = tc.ManageTimedEvents())
            {
                Parallel.ForEach(manager.Events, midiEvent =>
                {
                    switch (midiEvent.Event)
                    {
                        case ProgramChangeEvent pe:
                            pe.Channel = (FourBitNumber)idx;
                            break;
                        case ControlChangeEvent ce:
                            ce.Channel = (FourBitNumber)idx;
                            break;
                        case PitchBendEvent pbe:
                            pbe.Channel = (FourBitNumber)idx;
                            break;
                    }
                });
            }

            idx++;
        }

        return midiFile;
    }

    /// <summary>
    ///     The drum mapper
    /// </summary>
    /// <param name="midiFile">MidiFile</param>
    /// <param name="trackNumber">drum track</param>
    /// <param name="drumlist">drum list</param>
    /// <returns>MidiFile</returns>
    private MidiFile Drummapping(MidiFile midiFile, int trackNumber, IReadOnlyCollection<DMaps> drumlist)
    {
        var tc = midiFile.GetTrackChunks().ElementAt(trackNumber);

        var drumTracks = new Dictionary<string, TrackChunk>();
        foreach (var note in tc.GetNotes())
        {
            var drum = drumlist.FirstOrDefault(dm => dm.MidiNote == note.NoteNumber);
            if (drum == null)
                continue;

            var ret = drumTracks.FirstOrDefault(item => item.Key == drum.Instrument);
            if (ret.Key == null)
            {
                drumTracks[drum.Instrument] = new TrackChunk(new SequenceTrackNameEvent(drum.Instrument));
                using var notesManager = drumTracks[drum.Instrument].ManageNotes();
                var notes = notesManager.Notes;
                note.NoteNumber = (SevenBitNumber)drum.GameNote;
                notes.Add(note);
            }
            else
            {
                using var notesManager = drumTracks[drum.Instrument].ManageNotes();
                var notes = notesManager.Notes;
                note.NoteNumber = (SevenBitNumber)drum.GameNote;
                notes.Add(note);
            }
        }

        foreach (var nt in drumTracks)
            midiFile.Chunks.Add(nt.Value);

        return midiFile;
    }

    /// <summary>
    ///     Remove all prog changes
    /// </summary>
    /// <param name="midiFile"></param>
    /// <param name="trackNumber"></param>
    /// <returns>MidiFile</returns>
    private MidiFile ClearProgChanges(MidiFile midiFile, int trackNumber)
    {
        var tc = midiFile.GetTrackChunks().ElementAt(trackNumber);
        using var manager = tc.ManageTimedEvents();
        manager.Events.RemoveAll(e => e.Event.EventType == MidiEventType.ProgramChange);
        manager.Events.RemoveAll(e => e.Event.EventType == MidiEventType.ProgramName);

        return midiFile;
    }

    /// <summary>
    ///     Merge Tracks vis Melanchall
    /// </summary>
    /// <param name="midiFile"></param>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns>MidiFile</returns>
    private MidiFile MergeTracks(MidiFile midiFile, int source, int dest)
    {
        var src_tc = midiFile.GetTrackChunks().ElementAt(source);
        var dest_tc = midiFile.GetTrackChunks().ElementAt(dest);

        dest_tc = new List<TrackChunk> { src_tc, dest_tc }.Merge();
        midiFile.Chunks.Remove(src_tc);
        midiFile.Chunks.Remove(dest_tc);
        midiFile.Chunks.Insert(dest, dest_tc);
        return midiFile;
    }

    /// <summary>
    ///     Sets the channel number for a track
    /// </summary>
    /// <param name="midiFile"></param>
    /// <param name="trackNumber"></param>
    /// <returns>MidiFile</returns>
    private static MidiFile SetChanNumber(MidiFile midiFile, int trackNumber)
    {
        var tc = midiFile.GetTrackChunks().ElementAt(trackNumber);

        using (var notesManager = tc.ManageNotes())
        {
            Parallel.ForEach(notesManager.Notes, note => { note.Channel = (FourBitNumber)trackNumber; });
        }

        using (var manager = tc.ManageTimedEvents())
        {
            Parallel.ForEach(manager.Events, midiEvent =>
            {
                switch (midiEvent.Event)
                {
                    case ProgramChangeEvent pe:
                        pe.Channel = (FourBitNumber)trackNumber;
                        break;
                    case ControlChangeEvent ce:
                        ce.Channel = (FourBitNumber)trackNumber;
                        break;
                    case PitchBendEvent pbe:
                        pbe.Channel = (FourBitNumber)trackNumber;
                        break;
                }
            });
        }

        return midiFile;
    }

    /// <summary>
    ///     Transpose a track with x halftones
    /// </summary>
    /// <param name="midiFile"></param>
    /// <param name="trackNumber"></param>
    /// <param name="halftones"></param>
    /// <returns>MidiFile</returns>
    private static MidiFile Transpose(MidiFile midiFile, int trackNumber, int halftones)
    {
        var tc = midiFile.GetTrackChunks().ElementAt(trackNumber);
        using var notesManager = tc.ManageNotes();
        var notes = notesManager.Notes;
        Parallel.ForEach(notes, note => { note.NoteNumber = (SevenBitNumber)(note.NoteNumber + halftones); });

        return midiFile;
    }

    #endregion

    #region private Helpers

    private MidiFile GetMelanchallMidiFile()
    {
        var stream = GetMidiStreamFromSanford();
        var midiFile = MidiFile.Read(stream);
        stream.Close();
        stream.Dispose();
        return midiFile;
    }

    private void melanchallToSequencer(MidiFile midiFile)
    {
        var stream = new MemoryStream();
        midiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { TextEncoding = Encoding.ASCII });

        stream.Rewind();
        OpenFile(stream);
        stream.Close();
        stream.Dispose();
    }

    #endregion
}