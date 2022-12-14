namespace BardMusicPlayer.Seer.Events
{
    public sealed class ChatStatusChanged : SeerEvent
    {
        internal ChatStatusChanged(EventSource readerBackendType, bool chatStatus) : base(readerBackendType, 0, true)
        {
            EventType = GetType();
            ChatStatus = chatStatus;
        }

        public bool ChatStatus { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}