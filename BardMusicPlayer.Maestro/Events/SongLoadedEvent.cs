#region

using System.Collections.Generic;
using BardMusicPlayer.Maestro.Sequencing;

#endregion

namespace BardMusicPlayer.Maestro.Events
{
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

        public int TotalNoteCount
        {
            get
            {
                var sum = 0;
                foreach (var s in _sequencer.notesPlayedCount.Values)
                    sum += s;
                return sum;
            }
        }

        public List<int> CurrentNoteCountForTracks
        {
            get
            {
                var t = new List<int>();
                foreach (var s in _sequencer.notesPlayedCount)
                    t.Add(s.Key.Count);
                return t;
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}