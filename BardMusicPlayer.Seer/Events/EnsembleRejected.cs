namespace BardMusicPlayer.Seer.Events
{
    public sealed class EnsembleRejected : SeerEvent
    {
        internal EnsembleRejected(EventSource readerBackendType) : base(readerBackendType, 100)
        {
            EventType = GetType();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}