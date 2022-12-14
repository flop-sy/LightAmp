#region

using System;
using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal sealed class HotbarJobSlot : IDisposable
    {
        public Dictionary<int, HotbarSlot> JobSlots = new();

        public HotbarSlot this[int i]
        {
            get
            {
                if (!JobSlots.ContainsKey(i)) JobSlots[i] = new HotbarSlot();

                return JobSlots[i];
            }
            set => JobSlots[i] = value;
        }

        public void Dispose()
        {
            if (JobSlots == null) return;

            foreach (var slot in JobSlots.Values) slot?.Dispose();

            JobSlots.Clear();
        }

        ~HotbarJobSlot()
        {
            Dispose();
        }
    }
}