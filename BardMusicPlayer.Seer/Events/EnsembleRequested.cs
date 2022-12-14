namespace BardMusicPlayer.Seer.Events

{
    public sealed class EnsembleRequested : SeerEvent
    {
        internal EnsembleRequested(EventSource readerBackendType) : base(readerBackendType, 100)
        {
            EventType = GetType();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}