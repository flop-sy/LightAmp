#region

using System;

#endregion

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public class NTPEventArgs : EventArgs
    {
        public NTPEventArgs(string server, long latency, long skew)
        {
            Server = server;
            Latency = latency;
            Skew = skew;
        }

        public string Server { get; }
        public long Latency { get; }
        public long Skew { get; }
    }
}