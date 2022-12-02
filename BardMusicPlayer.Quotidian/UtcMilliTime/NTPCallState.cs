#region

using System.Diagnostics;
using System.Net.Sockets;

#endregion

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public class NTPCallState
    {
        public byte[] buffer = new byte[Constants.bytes_per_buffer];
        public Stopwatch latency;
        public short methodsCompleted;
        public bool priorSyncState;
        public string serverResolved;
        public Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Stopwatch timer;

        public NTPCallState()
        {
            latency = Stopwatch.StartNew();
            buffer[0] = 0x1B;
        }

        public void OrderlyShutdown()
        {
            if (timer != null)
            {
                if (timer.IsRunning) timer.Stop();
                timer = null;
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
            if (latency != null)
            {
                if (latency.IsRunning) latency.Stop();
                latency = null;
            }
        }
    }
}