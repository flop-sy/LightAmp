#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal sealed class MetaDataEvent : MetaEvent
    {
        public MetaDataEvent(int delta, byte status, byte metaId, byte[] data)
            : base(delta, status, metaId, 0)
        {
            Data = data;
        }

        public byte[] Data { get; }

        public override void WriteTo(IWriteable s)
        {
            s.WriteByte(0xFF);
            s.WriteByte((byte)MetaStatus);

            var l = Data.Length;
            MidiFile.WriteVariableInt(s, l);
            s.Write(Data, 0, Data.Length);
        }
    }
}