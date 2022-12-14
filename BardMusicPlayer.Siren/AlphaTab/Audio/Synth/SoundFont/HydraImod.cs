#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraImod
{
    public const int SizeInFile = 10;

    public ushort ModSrcOper { get; set; }
    public ushort ModDestOper { get; set; }
    public short ModAmount { get; set; }
    public ushort ModAmtSrcOper { get; set; }
    public ushort ModTransOper { get; set; }

    public static HydraImod Load(IReadable reader)
    {
        var imod = new HydraImod
        {
            ModSrcOper = reader.ReadUInt16LE(),
            ModDestOper = reader.ReadUInt16LE(),
            ModAmount = reader.ReadInt16LE(),
            ModAmtSrcOper = reader.ReadUInt16LE(),
            ModTransOper = reader.ReadUInt16LE()
        };
        return imod;
    }
}