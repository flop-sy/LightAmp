#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont
{
    internal sealed class HydraInst
    {
        public const int SizeInFile = 22;

        public string InstName { get; set; }
        public ushort InstBagNdx { get; set; }

        public static HydraInst Load(IReadable reader)
        {
            var inst = new HydraInst
            {
                InstName = reader.Read8BitStringLength(20),
                InstBagNdx = reader.ReadUInt16LE()
            };
            return inst;
        }
    }
}