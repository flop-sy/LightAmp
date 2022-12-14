namespace BardMusicPlayer.Seer.Events

{
    public sealed class EnsembleStopped : SeerEvent
    {
        internal EnsembleStopped(EventSource readerBackendType) : base(readerBackendType, 100)
        {
            EventType = GetType();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}