#region

using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Quotidian.Structs;
using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Managers;

public partial class MidiManager
{
    /// <summary>
    ///     Set the Channel numbers for all tracks
    /// </summary>
    internal void AutoSetChanNumber()
    {
        var midiFile = GetMelanchallMidiFile();
        midiFile = AutoSetChannelsForAllTracks(midiFile);
        melanchallToSequencer(midiFile);
    }

    /// <summary>
    ///     Removes tracks without notes
    /// </summary>
    internal void RemoveEmptyTracks()
    {
        var empty = new List<Track>();

        foreach (var track in ToIEnumerable(sequence.GetEnumerator()))
        {
            var hasNotes = false;
            foreach (var ev in track.Iterator())
            {
                if (ev.MidiMessage is MetaMessage { MetaType: MetaType.TimeSignature })
                {
                    hasNotes = true;
                    break;
                }

                if (ev.MidiMessage is not ChannelMessage { Command: ChannelCommand.NoteOn }) continue;
                hasNotes = true;
                break;
            }

            if (!hasNotes)
                empty.Add(track);
        }

        foreach (var t in empty)
            sequence.Remove(t);
    }

    #region Primary Track Functions

    /// <summary>
    ///     add a track
    /// </summary>
    internal void AddTrack()
    {
        sequence.Add(new Track());
    }

    /// <summary>
    ///     Merge tracks
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    internal void MergeTracks(int source, int dest)
    {
        //Set the channel numbers
        var midiFile = GetMelanchallMidiFile();
        midiFile = SetChanNumber(midiFile, source);
        midiFile = SetChanNumber(midiFile, dest);
        melanchallToSequencer(midiFile);

        //Do this with sanford, Melanchall is too slow
        var src_track = ToIEnumerable(sequence.GetEnumerator()).ElementAt(source);
        var dest_track = ToIEnumerable(sequence.GetEnumerator()).ElementAt(dest);

        //Remove all progchanges
        var flt = src_track.Iterator().AsParallel().Where(ev =>
            ev.MidiMessage is ChannelMessage { Command: ChannelCommand.ProgramChange });
        foreach (var ev in flt)
            src_track.Remove(ev);

        flt = dest_track.Iterator().AsParallel().Where(ev =>
            ev.MidiMessage is ChannelMessage { Command: ChannelCommand.ProgramChange });
        foreach (var ev in flt)
            dest_track.Remove(ev);


        sequence.MergeTracks(source, dest);

        dest_track = ToIEnumerable(sequence.GetEnumerator()).ElementAt(dest);

        var lastchannel = -1;
        foreach (var ev in dest_track.Iterator())
            if (ev.MidiMessage is ChannelMessage { Command: ChannelCommand.NoteOn } chanMsg)
                if (chanMsg.MidiChannel != lastchannel)
                {
                    cmBuilder.Command = ChannelCommand.ProgramChange;
                    cmBuilder.Data1 = Instrument.Parse(GetTrackName(chanMsg.MidiChannel)).MidiProgramChangeCode;
                    cmBuilder.Data2 = 64;
                    cmBuilder.MidiChannel = chanMsg.MidiChannel;
                    cmBuilder.Build();
                    dest_track.Insert(ev.AbsoluteTicks, cmBuilder.Result);
                    lastchannel = chanMsg.MidiChannel;
                }

        SetChanNumber(dest);
    }

    /// <summary>
    ///     Remove the track[id]
    /// </summary>
    /// <param name="selectedTrack"></param>
    internal void RemoveTrack(int selectedTrack)
    {
        sequence.RemoveAt(selectedTrack);
    }

    #endregion
}