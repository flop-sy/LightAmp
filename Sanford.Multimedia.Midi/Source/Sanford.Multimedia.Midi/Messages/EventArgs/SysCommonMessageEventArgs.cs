#region

using System;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed class SysCommonMessageEventArgs : EventArgs
    {
        public SysCommonMessageEventArgs(SysCommonMessage message)
        {
            Message = message;
        }

        public SysCommonMessage Message { get; }
    }
}