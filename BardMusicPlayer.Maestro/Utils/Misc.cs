#region

using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Maestro.Utils
{
    public sealed class NoteEvent
    {
        public int note;
        public int origNote;
        public Track track;
        public int trackNum;
    }

    public sealed class ProgChangeEvent
    {
        public Track track;
        public int trackNum;
        public int voice;
    }

    public class ChannelAfterTouchEvent
    {
        public int command;
        public Track track;
        public int trackNum;
    }

    public static class NoteHelper
    {
        public static int ApplyOctaveShift(int note, int octave)
        {
            return note - 12 * 4 + 12 * octave;
        }
    }
}