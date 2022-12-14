#region

using BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Util;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Synthesis;

internal sealed class VoiceLfo
{
    public int SamplesUntil { get; set; }
    public float Level { get; set; }
    public float Delta { get; set; }

    public void Setup(float delay, int freqCents, float outSampleRate)
    {
        SamplesUntil = (int)(delay * outSampleRate);
        Delta = 4.0f * SynthHelper.Cents2Hertz(freqCents) / outSampleRate;
        Level = 0;
    }

    public void Process(int blockSamples)
    {
        if (SamplesUntil > blockSamples)
        {
            SamplesUntil -= blockSamples;
            return;
        }

        Level += Delta * blockSamples;
        switch (Level)
        {
            case > 1.0f:
                Delta = -Delta;
                Level = 2.0f - Level;
                break;
            case < -1.0f:
                Delta = -Delta;
                Level = -2.0f - Level;
                break;
        }
    }
}