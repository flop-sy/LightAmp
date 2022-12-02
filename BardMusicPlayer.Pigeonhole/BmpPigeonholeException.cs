#region

using System;

#endregion

namespace BardMusicPlayer.Pigeonhole
{
    public class BmpPigeonholeException : Exception
    {
        public BmpPigeonholeException(string message) : base(message)
        {
        }

        public BmpPigeonholeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}