#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal sealed partial class Reader
    {
        public bool CanGetCharacterId()
        {
            return Scanner.Locations.ContainsKey(Signatures.CharacterIdKey);
        }

        public string GetCharacterId()
        {
            var id = "";
            if (!CanGetCharacterId() || !MemoryHandler.IsAttached) return id;

            var characterIdMap = (IntPtr)Scanner.Locations[Signatures.CharacterIdKey];

            try
            {
                id = MemoryHandler.GetString(characterIdMap, MemoryHandler.Structures.CharacterId.Offset,
                    MemoryHandler.Structures.CharacterId.SourceSize);
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return id;
        }
    }
}