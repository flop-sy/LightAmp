#region

using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Maestro.Sequencing;

#endregion

namespace BardMusicPlayer.Maestro.Events;

public sealed class SongLoadedEvent : MaestroEvent
{
    private readonly Sequencer _sequencer;

    internal SongLoadedEvent(int maxtracks, Sequencer sequencer)
    {
        EventType = GetType();
        MaxTracks = maxtracks;
        _sequencer = sequencer;
    }

    public int MaxTracks { get; }

    public int TotalNoteCount => _sequencer.notesPlayedCount.Values.Sum();

    public List<int> CurrentNoteCountForTracks
    {
        get { return _sequencer.notesPlayedCount.Select(static s => s.Key.Count).ToList(); }
    }

    public override bool IsValid()
    {
        return true;
    }
}