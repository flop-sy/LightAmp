#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Events
{
    public sealed class MidibardPlaylistEvent : SeerEvent
    {
        internal MidibardPlaylistEvent(EventSource readerBackendType, Game game, int song) : base(readerBackendType)
        {
            EventType = GetType();
            ChatLogGame = game;
            Song = song;
        }

        public Game ChatLogGame { get; }
        public DateTime ChatLogTimeStamp { get; }
        public int Song { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}