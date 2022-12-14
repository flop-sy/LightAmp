#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraPhdr
{
    public const int SizeInFile = 38;

    public string PresetName { get; set; }
    public ushort Preset { get; set; }
    public ushort Bank { get; set; }
    public ushort PresetBagNdx { get; set; }
    public uint Library { get; set; }
    public uint Genre { get; set; }
    public uint Morphology { get; set; }

    public static HydraPhdr Load(IReadable reader)
    {
        var phdr = new HydraPhdr
        {
            PresetName = reader.Read8BitStringLength(20),
            Preset = reader.ReadUInt16LE(),
            Bank = reader.ReadUInt16LE(),
            PresetBagNdx = reader.ReadUInt16LE(),
            Library = reader.ReadUInt32LE(),
            Genre = reader.ReadUInt32LE(),
            Morphology = reader.ReadUInt32LE()
        };

        return phdr;
    }
}