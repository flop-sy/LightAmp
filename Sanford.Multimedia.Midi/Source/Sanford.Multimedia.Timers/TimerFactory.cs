#region

using System;

#endregion

namespace Sanford.Multimedia.Timers
{
    /// <summary>
    ///     Use this factory to create ITimer instances.
    /// </summary>
    /// Caller is responsible for Dispose.
    public static class TimerFactory
    {
        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        ///     Creates an instance of ITimer
        /// </summary>
        /// <returns>Newly created instance of ITimer</returns>
        public static ITimer Create()
        {
            if (IsRunningOnMono()) return new ThreadTimer();

            return new Timer();
        }
    }
}