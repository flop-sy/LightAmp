#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class MidiEvent
{
    internal MidiEvent(object owner, int absoluteTicks, IMidiMessage message)
    {
        #region Require

        if (absoluteTicks < 0)
            throw new ArgumentOutOfRangeException(nameof(absoluteTicks), absoluteTicks,
                "Absolute ticks out of range.");

        #endregion

        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        AbsoluteTicks = absoluteTicks;
        MidiMessage = message ?? throw new ArgumentNullException("e");
    }

    internal object Owner { get; }

    public int AbsoluteTicks { get; private set; }

    public int DeltaTicks
    {
        get
        {
            int deltaTicks;

            if (Previous != null)
                deltaTicks = AbsoluteTicks - Previous.AbsoluteTicks;
            else
                deltaTicks = AbsoluteTicks;

            return deltaTicks;
        }
    }

    public IMidiMessage MidiMessage { get; }

    internal MidiEvent Next { get; set; }

    internal MidiEvent Previous { get; set; }

    internal void SetAbsoluteTicks(int absoluteTicks)
    {
        AbsoluteTicks = absoluteTicks;
    }
}