#region

using System;

#endregion

namespace Sanford.Multimedia;

public sealed class ErrorEventArgs : EventArgs
{
    public ErrorEventArgs(Exception ex)
    {
        Error = ex;
    }

    public Exception Error { get; }
}