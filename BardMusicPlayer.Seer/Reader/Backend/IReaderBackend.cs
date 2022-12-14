#region

using System;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Seer.Events;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend
{
    internal interface IReaderBackend : IDisposable
    {
        EventSource ReaderBackendType { get; }

        ReaderHandler ReaderHandler { get; set; }

        int SleepTimeInMs { get; set; }

        Task Loop(CancellationToken token);
    }
}