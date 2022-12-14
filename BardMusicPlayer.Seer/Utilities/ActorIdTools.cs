namespace BardMusicPlayer.Seer.Utilities
{
    internal static class ActorIdTools
    {
        internal static bool RangeOkay(uint actorId)
        {
            return actorId is >= 200000000 and < 300000000;
        }

        internal static bool RangeOkay(int actorId)
        {
            return actorId is >= 200000000 and < 300000000;
        }
    }
}