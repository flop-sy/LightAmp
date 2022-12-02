#region

using System;
using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal class HotbarRow : IDisposable
    {
        public Dictionary<int, HotbarJobSlot> Slots = new();

        public HotbarJobSlot this[int i]
        {
            get
            {
                if (!Slots.ContainsKey(i)) Slots[i] = new HotbarJobSlot();
                return Slots[i];
            }
            set => Slots[i] = value;
        }

        public void Dispose()
        {
            if (Slots == null) return;

            foreach (var slot in Slots.Values) slot?.Dispose();

            Slots.Clear();
        }

        ~HotbarRow()
        {
            Dispose();
        }
    }
}