#region

using System;
using System.ComponentModel;

#endregion

namespace Sanford.Threading;

/// <summary>
///     Represents information about the InvokeCompleted event.
/// </summary>
public sealed class InvokeCompletedEventArgs : AsyncCompletedEventArgs
{
    private readonly object[] args;

    public InvokeCompletedEventArgs(Delegate method, object[] args, object result, Exception error)
        : base(error, false, null)
    {
        Method = method;
        this.args = args;
        Result = result;
    }

    public Delegate Method { get; }

    public object Result { get; }

    public object[] GetArgs()
    {
        return args;
    }
}