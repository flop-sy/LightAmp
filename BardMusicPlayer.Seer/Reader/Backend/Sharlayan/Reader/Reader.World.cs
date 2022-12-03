#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal sealed partial class Reader
    {
        public bool CanGetWorld()
        {
            return Scanner.Locations.ContainsKey(Signatures.WorldKey);
        }

        public string GetWorld()
        {
            if (!CanGetWorld() || !MemoryHandler.IsAttached) return string.Empty;

            var worldMap = (IntPtr)Scanner.Locations[Signatures.WorldKey];
            try
            {
                var world = MemoryHandler.GetString(worldMap, MemoryHandler.Structures.World.Offset,
                    MemoryHandler.Structures.World.SourceSize);
                return world;
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return string.Empty;
        }
    }
}