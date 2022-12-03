#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Script
{
    public sealed class BmpScriptException : BmpException
    {
        internal BmpScriptException()
        {
        }

        internal BmpScriptException(string message) : base(message)
        {
        }
    }
}