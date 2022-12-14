#region

using BardMusicPlayer.Siren.AlphaTab.IO;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.SoundFont;

internal sealed class HydraShdr
{
    public const int SizeInFile = 46;

    public string SampleName { get; set; }
    public uint Start { get; set; }
    public uint End { get; set; }
    public uint StartLoop { get; set; }
    public uint EndLoop { get; set; }
    public uint SampleRate { get; set; }
    public byte OriginalPitch { get; set; }
    public sbyte PitchCorrection { get; set; }
    public ushort SampleLink { get; set; }
    public ushort SampleType { get; set; }

    public static HydraShdr Load(IReadable reader)
    {
        var shdr = new HydraShdr
        {
            SampleName = reader.Read8BitStringLength(20),
            Start = reader.ReadUInt32LE(),
            End = reader.ReadUInt32LE(),
            StartLoop = reader.ReadUInt32LE(),
            EndLoop = reader.ReadUInt32LE(),
            SampleRate = reader.ReadUInt32LE(),
            OriginalPitch = (byte)reader.ReadByte(),
            PitchCorrection = reader.ReadSignedByte(),
            SampleLink = reader.ReadUInt16LE(),
            SampleType = reader.ReadUInt16LE()
        };
        return shdr;
    }
}