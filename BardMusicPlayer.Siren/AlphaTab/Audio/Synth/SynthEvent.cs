#region

using BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth
{
    internal class SynthEvent
    {
        public SynthEvent(int eventIndex, MidiEvent e)
        {
            EventIndex = eventIndex;
            Event = e;
        }

        public int EventIndex { get; set; }
        public MidiEvent Event { get; set; }
        public double Time { get; set; }
    }
}