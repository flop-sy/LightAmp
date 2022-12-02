namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PerformerUpdate : MaestroEvent
    {
        internal PerformerUpdate()
        {
            EventType = GetType();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}