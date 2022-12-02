#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Events
{
    internal class ExceptionEvent : EventArgs
    {
        public ExceptionEvent(object sender, Exception exception)
        {
            Sender = sender;
            Exception = exception;
        }

        public Exception Exception { get; set; }

        public object Sender { get; set; }
    }
}