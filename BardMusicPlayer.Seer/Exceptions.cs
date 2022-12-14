#region

#region

using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Seer.Events;

#endregion

namespace BardMusicPlayer.Seer
{
    public class BmpSeerException : BmpException
    {
        internal BmpSeerException()
        {
        }

        internal BmpSeerException(string message) : base(message)
        {
        }
    }

    public sealed class BmpSeerGamePathException : BmpSeerException
    {
        internal BmpSeerGamePathException(string message) : base(message)
        {
        }
    }

    public sealed class BmpSeerEnvironmentTypeException : BmpSeerException
    {
        internal BmpSeerEnvironmentTypeException(string message) : base(message)
        {
        }
    }

    public sealed class BmpSeerGameRegionException : BmpSeerException
    {
        internal BmpSeerGameRegionException(string message) : base(message)
        {
        }
    }

    public sealed class BmpSeerConfigPathException : BmpSeerException
    {
        internal BmpSeerConfigPathException(string message) : base(message)
        {
        }
    }

    public sealed class BmpSeerBackendAlreadyRunningException : BmpSeerException
    {
        internal BmpSeerBackendAlreadyRunningException(int pid, EventSource readerBackendType) : base("Backend " +
            readerBackendType + " already running for pid " + pid)
        {
        }
    }

    public sealed class BmpSeerMachinaException : BmpSeerException
    {
        internal BmpSeerMachinaException(string message) : base(message)
        {
        }
    }
}