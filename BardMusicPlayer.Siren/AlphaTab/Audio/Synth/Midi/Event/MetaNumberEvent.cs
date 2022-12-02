#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal class MetaNumberEvent : MetaEvent
    {
        public MetaNumberEvent(int delta, byte status, byte metaId, int number)
            : base(delta, status, metaId, 0)
        {
            Value = number;
        }

        public int Value { get; }


        public override void WriteTo(IWriteable s)
        {
            s.WriteByte(0xFF);
            s.WriteByte((byte)MetaStatus);

            MidiFile.WriteVariableInt(s, 3);

            var b = new[]
            {
                (byte)((Value >> 16) & 0xFF), (byte)((Value >> 8) & 0xFF), (byte)(Value & 0xFF)
            };
            s.Write(b, 0, b.Length);
        }
    }
}