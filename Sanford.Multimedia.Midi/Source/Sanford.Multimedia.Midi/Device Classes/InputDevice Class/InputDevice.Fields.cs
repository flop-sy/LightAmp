#region

using System.Collections.Generic;
using Sanford.Threading;

#endregion

namespace Sanford.Multimedia.Midi
{
    public sealed partial class InputDevice
    {
        private readonly ChannelMessageBuilder cmBuilder = new ChannelMessageBuilder();

        private readonly DelegateQueue delegateQueue;

        private readonly MidiHeaderBuilder headerBuilder = new MidiHeaderBuilder();
        private readonly object lockObject = new object();

        private readonly MidiInProc midiInProc;

        private readonly SysCommonMessageBuilder scBuilder = new SysCommonMessageBuilder();

        private readonly List<byte> sysExData = new List<byte>();

        private volatile int bufferCount;

        private bool recording;

        private volatile bool resetting;

        private int sysExBufferSize = 4096;

        private delegate void GenericDelegate<in T>(T args);
    }
}