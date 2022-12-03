#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed class InvalidSysExMessageEventArgs : EventArgs
    {
        private readonly byte[] messageData;

        public InvalidSysExMessageEventArgs(byte[] messageData)
        {
            this.messageData = messageData;
        }

        public ICollection MessageData => messageData;
    }
}