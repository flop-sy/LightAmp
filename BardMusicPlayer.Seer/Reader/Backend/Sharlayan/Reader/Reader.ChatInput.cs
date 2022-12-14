#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader;

internal sealed partial class Reader
{
    public bool CanGetChatInput()
    {
        return Scanner.Locations.ContainsKey(Signatures.ChatInputKey);
    }

    public bool IsChatInputOpen()
    {
        if (!CanGetChatInput() || !MemoryHandler.IsAttached) return false;

        try
        {
            var chatInputMap = (IntPtr)Scanner.Locations[Signatures.ChatInputKey];
            var pointer = (IntPtr)MemoryHandler.GetInt32(chatInputMap) != IntPtr.Zero;
            return pointer;
        }
        catch (Exception ex)
        {
            MemoryHandler?.RaiseException(ex);
        }

        return false;
    }
}