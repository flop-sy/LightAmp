#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Script
{
    public class BmpScriptException : BmpException
    {
        internal BmpScriptException()
        {
        }

        internal BmpScriptException(string message) : base(message)
        {
        }
    }
}