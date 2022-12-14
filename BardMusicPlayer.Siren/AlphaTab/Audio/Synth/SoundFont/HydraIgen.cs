#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraIgen
{
    public const int SizeInFile = 4;

    public ushort GenOper { get; set; }
    public HydraGenAmount GenAmount { get; set; }

    public static HydraIgen Load(IReadable reader)
    {
        var igen = new HydraIgen
        {
            GenOper = reader.ReadUInt16LE(),
            GenAmount = HydraGenAmount.Load(reader)
        };
        return igen;
    }
}