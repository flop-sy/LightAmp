#region

using System;
using BardMusicPlayer.Grunt.Helper.Dalamud;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.Grunt
{
    public sealed class BmpGrunt
    {
        private static readonly Lazy<BmpGrunt> LazyInstance = new(static () => new BmpGrunt());

        internal DalamudServer DalamudServer;

        private BmpGrunt()
        {
        }

        /// <summary>
        /// </summary>
        public bool Started { get; private set; }

        public static BmpGrunt Instance => LazyInstance.Value;

        /// <summary>
        ///     Start Grunt.
        /// </summary>
        public void Start()
        {
            if (Started) return;

            if (!BmpPigeonhole.Initialized) throw new BmpGruntException("Grunt requires Pigeonhole to be initialized.");
            if (!BmpSeer.Instance.Started) throw new BmpGruntException("Grunt requires Seer to be running.");

            DalamudServer = new DalamudServer();
            Started = true;
        }

        /// <summary>
        ///     Stop Grunt.
        /// </summary>
        public void Stop()
        {
            if (!Started) return;

            DalamudServer?.Dispose();
            DalamudServer = null;
            Started = false;
        }

        ~BmpGrunt()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}