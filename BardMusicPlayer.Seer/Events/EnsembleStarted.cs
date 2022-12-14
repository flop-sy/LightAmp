namespace BardMusicPlayer.Seer.Events

{
    public sealed class EnsembleStarted : SeerEvent
    {
        internal EnsembleStarted(EventSource readerBackendType, long timestamp = -1) : base(readerBackendType, 100,
            true)
        {
            EventType = GetType();
            NetTimeStamp = timestamp;
        }

        public long NetTimeStamp { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}