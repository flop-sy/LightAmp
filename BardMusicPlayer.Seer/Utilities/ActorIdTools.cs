namespace BardMusicPlayer.Seer.Utilities
{
    internal static class ActorIdTools
    {
        internal static bool RangeOkay(uint actorId)
        {
            return actorId >= 200000000 && actorId < 300000000;
        }

        internal static bool RangeOkay(int actorId)
        {
            return actorId >= 200000000 && actorId < 300000000;
        }
    }
}