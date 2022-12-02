#region

using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.DalamudBridge
{
    public class DalamudBridgeException : BmpException
    {
        internal DalamudBridgeException()
        {
        }

        internal DalamudBridgeException(string message) : base(message)
        {
        }
    }
}