﻿#region

#region

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Config;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

#endregion

namespace BardMusicPlayer.Transmogrify.Processor
{
    internal sealed class LyricProcessor : BaseProcessor
    {
        internal LyricProcessor(LyricProcessorConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public LyricProcessorConfig ProcessorConfig { get; set; }

        public override Task<List<TrackChunk>> Process()
        {
            var trackChunk = new List<TrackChunk> { Song.TrackContainers[ProcessorConfig.Track].SourceTrackChunk }
                .Concat(ProcessorConfig.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk))
                .Merge();

            var lyricEvents = new List<TimedEvent>();

            var lyricLineCount = 0;

            var tempoMap = Song.SourceTempoMap.Clone();

            foreach (var midiEvent in trackChunk.GetTimedEvents()
                         .Where(static e => e.Event.EventType == MidiEventType.Lyric))
            {
                lyricLineCount++;
                midiEvent.Time = midiEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 + 120000;
                lyricEvents.Add(midiEvent);
            }

            var trackChunks = new List<TrackChunk>();

            for (var i = 0; i < ProcessorConfig.PlayerCount; i++)
            {
                trackChunk = new TrackChunk();
                trackChunk.AddObjects(lyricEvents);
                trackChunk.AddObjects(new List<ITimedObject>
                    { new TimedEvent(new SequenceTrackNameEvent("lyric:" + lyricLineCount)) });
                trackChunks.Add(trackChunk);
            }

            return Task.FromResult(trackChunks);
        }
    }
}