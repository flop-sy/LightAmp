#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Grunt
{
    public class BmpGruntException : BmpException
    {
        internal BmpGruntException()
        {
        }

        internal BmpGruntException(string message) : base(message)
        {
        }
    }
}