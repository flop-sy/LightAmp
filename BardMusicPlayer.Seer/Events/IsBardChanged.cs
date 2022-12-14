namespace BardMusicPlayer.Seer.Events
{
    public sealed class IsBardChanged : SeerEvent
    {
        internal IsBardChanged(EventSource readerBackendType, bool isBard) : base(readerBackendType)
        {
            EventType = GetType();
            IsBard = isBard;
        }

        public bool IsBard { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}