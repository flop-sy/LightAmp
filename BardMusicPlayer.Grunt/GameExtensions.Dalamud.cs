#region

using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.Grunt
{
    public static partial class GameExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static bool IsDalamudHooked(this Game game)
        {
            if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");
            return BmpGrunt.Instance.DalamudServer.IsConnected(game.Pid);
        }
    }
}