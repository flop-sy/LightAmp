﻿#region

using System;
using System.Collections.Generic;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;

internal sealed class HotbarData : IDisposable
{
    public Dictionary<int, HotbarRow> Rows = new();

    public HotbarRow this[int i]
    {
        get
        {
            if (!Rows.ContainsKey(i)) Rows[i] = new HotbarRow();

            return Rows[i];
        }
        set => Rows[i] = value;
    }

    public void Dispose()
    {
        if (Rows == null) return;

        foreach (var slot in Rows.Values) slot?.Dispose();

        Rows.Clear();
    }

    ~HotbarData()
    {
        Dispose();
    }
}