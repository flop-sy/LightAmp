#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraPmod
{
    public const int SizeInFile = 10;

    public ushort ModSrcOper { get; set; }
    public ushort ModDestOper { get; set; }
    public ushort ModAmount { get; set; }
    public ushort ModAmtSrcOper { get; set; }
    public ushort ModTransOper { get; set; }

    public static HydraPmod Load(IReadable reader)
    {
        var pmod = new HydraPmod
        {
            ModSrcOper = reader.ReadUInt16LE(),
            ModDestOper = reader.ReadUInt16LE(),
            ModAmount = reader.ReadUInt16LE(),
            ModAmtSrcOper = reader.ReadUInt16LE(),
            ModTransOper = reader.ReadUInt16LE()
        };

        return pmod;
    }
}