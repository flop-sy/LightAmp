#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class ChannelChaser
{
    private readonly ChannelMessage[] channelPressureMessages;
    private readonly ChannelMessage[,] controllerMessages;

    private readonly ChannelMessage[] pitchBendMessages;

    private readonly ChannelMessage[] polyPressureMessages;

    private readonly ChannelMessage[] programChangeMessages;

    public ChannelChaser()
    {
        const int c = ChannelMessage.MidiChannelMaxValue + 1;
        const int d = ShortMessage.DataMaxValue + 1;

        controllerMessages = new ChannelMessage[c, d];

        programChangeMessages = new ChannelMessage[c];
        pitchBendMessages = new ChannelMessage[c];
        channelPressureMessages = new ChannelMessage[c];
        polyPressureMessages = new ChannelMessage[c];
    }

    public event EventHandler<ChasedEventArgs> Chased;

    public void Process(ChannelMessage message)
    {
        switch (message.Command)
        {
            case ChannelCommand.Controller:
                controllerMessages[message.MidiChannel, message.Data1] = message;
                break;

            case ChannelCommand.ChannelPressure:
                channelPressureMessages[message.MidiChannel] = message;
                break;

            case ChannelCommand.PitchWheel:
                pitchBendMessages[message.MidiChannel] = message;
                break;

            case ChannelCommand.PolyPressure:
                polyPressureMessages[message.MidiChannel] = message;
                break;

            case ChannelCommand.ProgramChange:
                programChangeMessages[message.MidiChannel] = message;
                break;
        }
    }

    public void Chase()
    {
        var chasedMessages = new ArrayList();

        for (var c = 0; c <= ChannelMessage.MidiChannelMaxValue; c++)
        {
            for (var n = 0; n <= ShortMessage.DataMaxValue; n++)
            {
                if (controllerMessages[c, n] == null) continue;

                chasedMessages.Add(controllerMessages[c, n]);

                controllerMessages[c, n] = null;
            }

            if (programChangeMessages[c] != null)
            {
                chasedMessages.Add(programChangeMessages[c]);

                programChangeMessages[c] = null;
            }

            if (pitchBendMessages[c] != null)
            {
                chasedMessages.Add(pitchBendMessages[c]);

                pitchBendMessages[c] = null;
            }

            if (channelPressureMessages[c] != null)
            {
                chasedMessages.Add(channelPressureMessages[c]);

                channelPressureMessages[c] = null;
            }

            if (polyPressureMessages[c] == null) continue;

            chasedMessages.Add(polyPressureMessages[c]);

            polyPressureMessages[c] = null;
        }

        OnChased(new ChasedEventArgs(chasedMessages));
    }

    public void Reset()
    {
        for (var c = 0; c <= ChannelMessage.MidiChannelMaxValue; c++)
        {
            for (var n = 0; n <= ShortMessage.DataMaxValue; n++) controllerMessages[c, n] = null;

            programChangeMessages[c] = null;
            pitchBendMessages[c] = null;
            channelPressureMessages[c] = null;
            polyPressureMessages[c] = null;
        }
    }

    private void OnChased(ChasedEventArgs e)
    {
        var handler = Chased;

        handler?.Invoke(this, e);
    }
}