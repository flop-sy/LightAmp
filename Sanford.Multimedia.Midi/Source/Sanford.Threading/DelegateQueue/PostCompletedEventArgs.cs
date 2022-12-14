#region

using System;
using System.ComponentModel;
using System.Threading;

#endregion

namespace Sanford.Threading;

public sealed class PostCompletedEventArgs : AsyncCompletedEventArgs
{
    public PostCompletedEventArgs(SendOrPostCallback callback, Exception error, object state)
        : base(error, false, state)
    {
        Callback = callback;
    }

    public SendOrPostCallback Callback { get; }
}