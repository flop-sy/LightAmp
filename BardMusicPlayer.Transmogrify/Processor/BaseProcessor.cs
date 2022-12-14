#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Core;

#endregion

namespace BardMusicPlayer.Transmogrify.Processor;

internal abstract class BaseProcessor : IDisposable
{
    protected BaseProcessor(BmpSong song)
    {
        Song = song;
    }

    protected BmpSong Song { get; set; }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public abstract Task<List<TrackChunk>> Process();

    ~BaseProcessor()
    {
        Dispose();
    }
}