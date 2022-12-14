#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Grunt;

public sealed class BmpGruntException : BmpException
{
    internal BmpGruntException()
    {
    }

    internal BmpGruntException(string message) : base(message)
    {
    }
}