#region

using System;
using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal partial class Packet : IDisposable
    {
        private readonly Dictionary<ulong, uint> _contentId2ActorId = new();
        private readonly MachinaReaderBackend _machinaReader;

        internal Packet(MachinaReaderBackend machinaReader)
        {
            _machinaReader = machinaReader;
        }

        public void Dispose()
        {
            _contentId2ActorId.Clear();
        }

        private static bool ValidTimeSig(byte timeSig)
        {
            return timeSig > 1 && timeSig < 8;
        }

        private static bool ValidTempo(byte tempo)
        {
            return tempo > 29 && tempo < 201;
        }

        ~Packet()
        {
            Dispose();
        }
    }
}