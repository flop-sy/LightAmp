#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraPbag
{
    public const int SizeInFile = 4;

    public ushort GenNdx { get; set; }
    public ushort ModNdx { get; set; }

    public static HydraPbag Load(IReadable reader)
    {
        var pbag = new HydraPbag
        {
            GenNdx = reader.ReadUInt16LE(),
            ModNdx = reader.ReadUInt16LE()
        };
        return pbag;
    }
}