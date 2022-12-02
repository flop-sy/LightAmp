#region

using System;

#endregion

namespace BardMusicPlayer.Quotidian
{
    public class BmpException : Exception
    {
        public BmpException()
        {
        }

        public BmpException(string message) : base(message)
        {
        }

        public BmpException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}