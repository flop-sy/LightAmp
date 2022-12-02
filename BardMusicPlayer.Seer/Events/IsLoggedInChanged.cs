namespace BardMusicPlayer.Seer.Events
{
    public sealed class IsLoggedInChanged : SeerEvent
    {
        internal IsLoggedInChanged(EventSource readerBackendType, bool status) : base(readerBackendType)
        {
            EventType = GetType();
            IsLoggedIn = status;
        }

        public bool IsLoggedIn { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}