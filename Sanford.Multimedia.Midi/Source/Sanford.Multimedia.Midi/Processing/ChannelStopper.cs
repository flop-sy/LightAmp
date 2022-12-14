#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class ChannelStopper
{
    private readonly ChannelMessageBuilder builder = new();

    private readonly bool[] holdPedal1Message;

    private readonly bool[] holdPedal2Message;
    private readonly ChannelMessage[,] noteOnMessage;

    private readonly bool[] sustenutoMessage;

    public ChannelStopper()
    {
        const int c = ChannelMessage.MidiChannelMaxValue + 1;
        const int d = ShortMessage.DataMaxValue + 1;

        noteOnMessage = new ChannelMessage[c, d];

        holdPedal1Message = new bool[c];
        holdPedal2Message = new bool[c];
        sustenutoMessage = new bool[c];
    }

    public event EventHandler<StoppedEventArgs> Stopped;

    public void Process(ChannelMessage message)
    {
        switch (message.Command)
        {
            case ChannelCommand.NoteOn:
                if (message.Data2 > 0)
                    noteOnMessage[message.MidiChannel, message.Data1] = message;
                else
                    noteOnMessage[message.MidiChannel, message.Data1] = null;

                break;

            case ChannelCommand.NoteOff:
                noteOnMessage[message.MidiChannel, message.Data1] = null;
                break;

            case ChannelCommand.Controller:
                switch (message.Data1)
                {
                    case (int)ControllerType.HoldPedal1:
                        if (message.Data2 > 63)
                            holdPedal1Message[message.MidiChannel] = true;
                        else
                            holdPedal1Message[message.MidiChannel] = false;

                        break;

                    case (int)ControllerType.HoldPedal2:
                        if (message.Data2 > 63)
                            holdPedal2Message[message.MidiChannel] = true;
                        else
                            holdPedal2Message[message.MidiChannel] = false;

                        break;

                    case (int)ControllerType.SustenutoPedal:
                        if (message.Data2 > 63)
                            sustenutoMessage[message.MidiChannel] = true;
                        else
                            sustenutoMessage[message.MidiChannel] = false;

                        break;
                }

                break;
        }
    }

    public void AllSoundOff()
    {
        var stoppedMessages = new ArrayList();

        for (var c = 0; c <= ChannelMessage.MidiChannelMaxValue; c++)
        {
            for (var n = 0; n <= ShortMessage.DataMaxValue; n++)
            {
                if (noteOnMessage[c, n] == null) continue;

                builder.MidiChannel = c;
                builder.Command = ChannelCommand.NoteOff;
                builder.Data1 = noteOnMessage[c, n].Data1;
                builder.Build();

                stoppedMessages.Add(builder.Result);

                noteOnMessage[c, n] = null;
            }

            if (holdPedal1Message[c])
            {
                builder.MidiChannel = c;
                builder.Command = ChannelCommand.Controller;
                builder.Data1 = (int)ControllerType.HoldPedal1;
                builder.Build();

                stoppedMessages.Add(builder.Result);

                holdPedal1Message[c] = false;
            }

            if (holdPedal2Message[c])
            {
                builder.MidiChannel = c;
                builder.Command = ChannelCommand.Controller;
                builder.Data1 = (int)ControllerType.HoldPedal2;
                builder.Build();

                stoppedMessages.Add(builder.Result);

                holdPedal2Message[c] = false;
            }

            if (!sustenutoMessage[c]) continue;

            builder.MidiChannel = c;
            builder.Command = ChannelCommand.Controller;
            builder.Data1 = (int)ControllerType.SustenutoPedal;
            builder.Build();

            stoppedMessages.Add(builder.Result);

            sustenutoMessage[c] = false;
        }

        OnStopped(new StoppedEventArgs(stoppedMessages));
    }

    public void Reset()
    {
        for (var c = 0; c <= ChannelMessage.MidiChannelMaxValue; c++)
        {
            for (var n = 0; n <= ShortMessage.DataMaxValue; n++) noteOnMessage[c, n] = null;

            holdPedal1Message[c] = false;
            holdPedal2Message[c] = false;
            sustenutoMessage[c] = false;
        }
    }

    private void OnStopped(StoppedEventArgs e)
    {
        var handler = Stopped;

        handler?.Invoke(this, e);
    }
}