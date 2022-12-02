namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PlaybackStoppedEvent : MaestroEvent
    {
        public bool Stopped;

        internal PlaybackStoppedEvent()
        {
            EventType = GetType();
            Stopped = true;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}