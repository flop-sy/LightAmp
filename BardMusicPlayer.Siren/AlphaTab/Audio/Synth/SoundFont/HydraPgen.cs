#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraPgen
{
    public const int SizeInFile = 4;

    public const int GenInstrument = 41;
    public const int GenKeyRange = 43;
    public const int GenVelRange = 44;
    public const int GenSampleId = 53;

    public ushort GenOper { get; set; }
    public HydraGenAmount GenAmount { get; set; }

    public static HydraPgen Load(IReadable reader)
    {
        var pgen = new HydraPgen
        {
            GenOper = reader.ReadUInt16LE(),
            GenAmount = HydraGenAmount.Load(reader)
        };
        return pgen;
    }
}