#region

using BardMusicPlayer.Seer.Utilities;

#endregion

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ActorIdChanged : SeerEvent
    {
        internal ActorIdChanged(EventSource readerBackendType, uint actorId) : base(readerBackendType)
        {
            EventType = GetType();
            ActorId = actorId;
        }

        public uint ActorId { get; }

        public override bool IsValid()
        {
            return ActorIdTools.RangeOkay(ActorId);
        }
    }
}