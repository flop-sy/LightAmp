#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed class ChasedEventArgs : EventArgs
    {
        public ChasedEventArgs(ICollection messages)
        {
            Messages = messages;
        }

        public ICollection Messages { get; }
    }
}