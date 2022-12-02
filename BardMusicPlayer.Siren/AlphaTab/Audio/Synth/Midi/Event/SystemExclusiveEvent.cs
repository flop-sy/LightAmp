#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal class SystemExclusiveEvent : SystemCommonEvent
    {
        public SystemExclusiveEvent(int delta, byte status, short id, byte[] data)
            : base(delta, status, (byte)(id & 0x00FF), (byte)(id >> 8))
        {
            Data = data;
        }

        public byte[] Data { get; }


        public int ManufacturerId => Message >> 8;

        public override void WriteTo(IWriteable s)
        {
            s.WriteByte(0xF0);
            var l = Data.Length + 2;
            s.WriteByte((byte)ManufacturerId);
            var b = new[]
            {
                (byte)((l >> 24) & 0xFF), (byte)((l >> 16) & 0xFF), (byte)((l >> 8) & 0xFF), (byte)(l & 0xFF)
            };
            s.Write(b, 0, b.Length);
            s.WriteByte(0xF7);
        }
    }
}