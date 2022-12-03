#region

using System;

#endregion

namespace BardMusicPlayer.Quotidian
{
    public sealed class BmpLog
    {
        public delegate void LogEventHandler(string output);

        public enum Source
        {
            Coffer,
            Grunt,
            Maestro,
            Pigeonhole,
            Quotidian,
            Seer,
            Siren,
            Transmogrify,
            Ui
        }

        public enum Verbosity
        {
            Verbose,
            Debug,
            Info,
            Warning,
            Error
        }

        private static readonly Lazy<BmpLog> Arbiter = new(static () => new BmpLog());

        private Verbosity _minVerbosity;

        public static BmpLog Instance => Arbiter.Value;

        public static void V(Source source, string format, params object[] args)
        {
            Instance.Log(Verbosity.Verbose, source, format, args);
        }

        public static void D(Source source, string format, params object[] args)
        {
            Instance.Log(Verbosity.Debug, source, format, args);
        }

        public static void I(Source source, string format, params object[] args)
        {
            Instance.Log(Verbosity.Info, source, format, args);
        }

        public static void W(Source source, string format, params object[] args)
        {
            Instance.Log(Verbosity.Warning, source, format, args);
        }

        public static void E(Source source, string format, params object[] args)
        {
            Instance.Log(Verbosity.Error, source, format, args);
        }

        public static void SetMinVerbosity(Verbosity verbosity)
        {
            Instance._minVerbosity = verbosity;
        }

        public event LogEventHandler LogEvent;

        private void Log(Verbosity verbosity, Source source, string format, params object[] args)
        {
            if (verbosity < _minVerbosity) return;

            format = "[" + verbosity + "] - [" + source + "] - " + format;
            var output = string.Format(format, args);

            LogEvent?.Invoke(output);
        }
    }
}