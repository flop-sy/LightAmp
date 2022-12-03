#region

using System;
using System.Diagnostics;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed partial class Track
    {
        [Conditional("MIDIDEBUG")]
        public static void Test()
        {
            TestInsert();
            TestRemoveAt();
            TestMerge();
        }

        [Conditional("MIDIDEBUG")]
        private static void TestInsert()
        {
            var track = new Track();
            const int midiEventCount = 2000;
            const int positionMax = 32000;
            const int endOfTrackOffset = 1000;
            var length = 0;
            var message = new ChannelMessage(ChannelCommand.NoteOff, 0, 60, 0);
            var r = new Random();

            for (var i = 0; i < midiEventCount; i++)
            {
                var position = r.Next(positionMax);

                if (position > length) length = position;

                track.Insert(position, message);
            }

            track.EndOfTrackOffset = endOfTrackOffset;

            length += track.EndOfTrackOffset;

            Debug.Assert(track.Count == midiEventCount + 1);
            Debug.Assert(track.Length == length);
        }

        [Conditional("MIDIDEBUG")]
        private static void TestRemoveAt()
        {
            var a = new Track();
            var message = new ChannelMessage(ChannelCommand.NoteOff, 0, 60, 0);

            a.Insert(0, message);
            a.Insert(10, message);
            a.Insert(20, message);
            a.Insert(30, message);
            a.Insert(40, message);

            var count = a.Count;

            a.RemoveAt(0);

            Debug.Assert(a.Count == count - 1);

            a.RemoveAt(a.Count - 2);

            Debug.Assert(a.Count == count - 2);
            Debug.Assert(a.GetMidiEvent(0).AbsoluteTicks == 10);
            Debug.Assert(a.GetMidiEvent(a.Count - 2).AbsoluteTicks == 30);

            a.RemoveAt(0);
            a.RemoveAt(0);
            a.RemoveAt(0);

            Debug.Assert(a.Count == 1);
        }

        [Conditional("MIDIDEBUG")]
        private static void TestMerge()
        {
            var a = new Track();
            var b = new Track();

            a.Merge(b);

            Debug.Assert(a.Count == 1);

            var message = new ChannelMessage(ChannelCommand.NoteOff, 0, 60, 0);

            b.Insert(0, message);
            b.Insert(10, message);
            b.Insert(20, message);
            b.Insert(30, message);
            b.Insert(40, message);

            a.Merge(b);

            Debug.Assert(a.Count == 1 + b.Count - 1);

            a.Clear();

            Debug.Assert(a.Count == 1);

            a.Insert(0, message);
            a.Insert(10, message);
            a.Insert(20, message);
            a.Insert(30, message);
            a.Insert(40, message);

            var count = a.Count;

            a.Merge(b);

            Debug.Assert(a.Count == count + b.Count - 1);
            Debug.Assert(a.GetMidiEvent(0).DeltaTicks == 0);
            Debug.Assert(a.GetMidiEvent(1).DeltaTicks == 0);
            Debug.Assert(a.GetMidiEvent(2).DeltaTicks == 10);
            Debug.Assert(a.GetMidiEvent(3).DeltaTicks == 0);
            Debug.Assert(a.GetMidiEvent(4).DeltaTicks == 10);
            Debug.Assert(a.GetMidiEvent(5).DeltaTicks == 0);
            Debug.Assert(a.GetMidiEvent(6).DeltaTicks == 10);
            Debug.Assert(a.GetMidiEvent(7).DeltaTicks == 0);
            Debug.Assert(a.GetMidiEvent(8).DeltaTicks == 10);
            Debug.Assert(a.GetMidiEvent(9).DeltaTicks == 0);
        }
    }
}