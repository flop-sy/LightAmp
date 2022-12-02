namespace BardMusicPlayer.Seer.Events
{
    public sealed class GameStarted : SeerEvent
    {
        internal GameStarted(Game game, int pid) : base(EventSource.Game)
        {
            EventType = GetType();
            Game = game;
            Pid = pid;
        }

        public int Pid { get; }

        public override bool IsValid()
        {
            return Game is not null;
        }
    }
}