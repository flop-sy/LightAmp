#region

using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class OctaveShiftChangedEvent : MaestroEvent
    {
        internal OctaveShiftChangedEvent(Game g, int octaveShift, bool isHost = false)
        {
            EventType = GetType();
            OctaveShift = octaveShift;
            game = g;
            IsHost = isHost;
        }

        public Game game { get; }
        public int OctaveShift { get; }
        public bool IsHost { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}