#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Transmogrify
{
    public sealed class BmpTransmogrifyException : BmpException
    {
        public BmpTransmogrifyException(string message) : base(message)
        {
        }
    }
}