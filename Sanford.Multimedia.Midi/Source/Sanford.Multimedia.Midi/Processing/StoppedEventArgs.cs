#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed class StoppedEventArgs : EventArgs
    {
        public StoppedEventArgs(ICollection messages)
        {
            Messages = messages;
        }

        public ICollection Messages { get; }
    }
}