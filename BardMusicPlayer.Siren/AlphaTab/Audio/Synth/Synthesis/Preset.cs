namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Synthesis
{
    internal sealed class Preset
    {
        public string Name { get; set; }
        public ushort PresetNumber { get; set; }
        public ushort Bank { get; set; }
        public Region[] Regions { get; set; }
        public float[] FontSamples { get; set; }
    }
}