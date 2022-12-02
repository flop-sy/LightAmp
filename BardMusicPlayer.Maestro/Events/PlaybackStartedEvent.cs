namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PlaybackStartedEvent : MaestroEvent
    {
        public bool Started;

        internal PlaybackStartedEvent()
        {
            EventType = GetType();
            Started = true;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}