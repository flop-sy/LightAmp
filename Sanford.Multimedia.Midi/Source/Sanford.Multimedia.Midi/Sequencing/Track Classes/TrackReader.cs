#region

using System;
using System.IO;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Reads a track from a stream.
/// </summary>
internal sealed class TrackReader
{
    private readonly ChannelMessageBuilder cmBuilder = new();

    private readonly SysCommonMessageBuilder scBuilder = new();
    private Track newTrack = new();

    private int previousTicks;

    private int runningStatus;

    private int status;

    private Stream stream;

    private int ticks;

    private byte[] trackData;

    private int trackIndex;

    public Track Track { get; private set; } = new();

    public void Read(Stream strm)
    {
        stream = strm;
        FindTrack();

        var trackLength = GetTrackLength();
        trackData = new byte[trackLength];

        var result = strm.Read(trackData, 0, trackLength);

        if (result < 0) throw new MidiFileException("End of MIDI file unexpectedly reached.");

        newTrack = new Track();

        ParseTrackData();

        Track = newTrack;
    }

    private void FindTrack()
    {
        var found = false;

        while (!found)
        {
            var result = stream.ReadByte();

            if (result == 'M')
            {
                result = stream.ReadByte();

                if (result == 'T')
                {
                    result = stream.ReadByte();

                    if (result == 'r')
                    {
                        result = stream.ReadByte();

                        if (result == 'k') found = true;
                    }
                }
            }

            if (result < 0) throw new MidiFileException("Unable to find track in MIDI file.");
        }
    }

    private int GetTrackLength()
    {
        var trackLength = new byte[4];

        var result = stream.Read(trackLength, 0, trackLength.Length);

        if (result < trackLength.Length) throw new MidiFileException("End of MIDI file unexpectedly reached.");

        if (BitConverter.IsLittleEndian) Array.Reverse(trackLength);

        return BitConverter.ToInt32(trackLength, 0);
    }

    private void ParseTrackData()
    {
        trackIndex = ticks = runningStatus = 0;

        while (trackIndex < trackData.Length)
        {
            previousTicks = ticks;

            ticks += ReadVariableLengthValue();

            if ((trackData[trackIndex] & 0x80) == 0x80)
            {
                status = trackData[trackIndex];
                trackIndex++;
            }
            else
            {
                status = runningStatus;
            }

            ParseMessage();
        }
    }

    private void ParseMessage()
    {
        // If this is a channel message.
        if (status is >= (int)ChannelCommand.NoteOff and <= (int)ChannelCommand.PitchWheel +
                                                            ChannelMessage.MidiChannelMaxValue)
            ParseChannelMessage();
        else
            switch (status)
            {
                // Else if this is a meta message.
                case 0xFF:
                    ParseMetaMessage();
                    break;
                // Else if this is the start of a system exclusive message.
                case (int)SysExType.Start:
                    ParseSysExMessageStart();
                    break;
                // Else if this is a continuation of a system exclusive message.
                case (int)SysExType.Continuation:
                    ParseSysExMessageContinue();
                    break;
                // Else if this is a system common message.
                default:
                {
                    switch (status)
                    {
                        case >= (int)SysCommonType.MidiTimeCode and <= (int)SysCommonType.TuneRequest:
                            ParseSysCommonMessage();
                            break;
                        // Else if this is a system realtime message.
                        case >= (int)SysRealtimeType.Clock and <= (int)SysRealtimeType.Reset:
                            ParseSysRealtimeMessage();
                            break;
                    }

                    break;
                }
            }
    }

    private void ParseChannelMessage()
    {
        if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

        cmBuilder.Command = ChannelMessage.UnpackCommand(status);
        cmBuilder.MidiChannel = ChannelMessage.UnpackMidiChannel(status);
        cmBuilder.Data1 = trackData[trackIndex];

        trackIndex++;

        if (ChannelMessage.DataBytesPerType(cmBuilder.Command) == 2)
        {
            if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

            cmBuilder.Data2 = trackData[trackIndex];

            trackIndex++;
        }

        cmBuilder.Build();
        newTrack.Insert(ticks, cmBuilder.Result);
        runningStatus = status;
    }

    private void ParseMetaMessage()
    {
        if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

        var type = (MetaType)trackData[trackIndex];

        trackIndex++;

        if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

        if (type == MetaType.EndOfTrack)
        {
            newTrack.EndOfTrackOffset = ticks - previousTicks;

            trackIndex++;
        }
        else
        {
            var data = new byte[ReadVariableLengthValue()];
            Array.Copy(trackData, trackIndex, data, 0, data.Length);
            newTrack.Insert(ticks, new MetaMessage(type, data));

            trackIndex += data.Length;
        }
    }

    private void ParseSysExMessageStart()
    {
        // System exclusive cancels running status.
        runningStatus = 0;

        var data = new byte[ReadVariableLengthValue() + 1];
        data[0] = (byte)SysExType.Start;

        Array.Copy(trackData, trackIndex, data, 1, data.Length - 1);
        newTrack.Insert(ticks, new SysExMessage(data));

        trackIndex += data.Length - 1;
    }

    private void ParseSysExMessageContinue()
    {
        trackIndex++;

        if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

        // System exclusive cancels running status.
        runningStatus = 0;

        // If this is an escaped message rather than a system exclusive
        // continuation message.
        if ((trackData[trackIndex] & 0x80) == 0x80)
        {
            status = trackData[trackIndex];
            trackIndex++;

            ParseMessage();
        }
        else
        {
            var data = new byte[ReadVariableLengthValue() + 1];
            data[0] = (byte)SysExType.Continuation;

            Array.Copy(trackData, trackIndex, data, 1, data.Length - 1);
            newTrack.Insert(ticks, new SysExMessage(data));

            trackIndex += data.Length - 1;
        }
    }

    private void ParseSysCommonMessage()
    {
        if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

        // System common cancels running status.
        runningStatus = 0;

        scBuilder.Type = (SysCommonType)status;

        switch ((SysCommonType)status)
        {
            case SysCommonType.MidiTimeCode:
                scBuilder.Data1 = trackData[trackIndex];
                trackIndex++;
                break;

            case SysCommonType.SongPositionPointer:
                scBuilder.Data1 = trackData[trackIndex];
                trackIndex++;

                if (trackIndex >= trackData.Length)
                    throw new MidiFileException("End of track unexpectedly reached.");

                scBuilder.Data2 = trackData[trackIndex];
                trackIndex++;
                break;

            case SysCommonType.SongSelect:
                scBuilder.Data1 = trackData[trackIndex];
                trackIndex++;
                break;

            case SysCommonType.TuneRequest:
                // Nothing to do here.
                break;
        }

        scBuilder.Build();

        newTrack.Insert(ticks, scBuilder.Result);
    }

    private void ParseSysRealtimeMessage()
    {
        var e = (SysRealtimeType)status switch
        {
            SysRealtimeType.ActiveSense => SysRealtimeMessage.ActiveSenseMessage,
            SysRealtimeType.Clock => SysRealtimeMessage.ClockMessage,
            SysRealtimeType.Continue => SysRealtimeMessage.ContinueMessage,
            SysRealtimeType.Reset => SysRealtimeMessage.ResetMessage,
            SysRealtimeType.Start => SysRealtimeMessage.StartMessage,
            SysRealtimeType.Stop => SysRealtimeMessage.StopMessage,
            SysRealtimeType.Tick => SysRealtimeMessage.TickMessage,
            _ => null
        };

        newTrack.Insert(ticks, e);
    }

    private int ReadVariableLengthValue()
    {
        if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

        var result = 0;

        result = trackData[trackIndex];

        trackIndex++;

        if ((result & 0x80) != 0x80) return result;

        result &= 0x7F;

        int temp;

        do
        {
            if (trackIndex >= trackData.Length) throw new MidiFileException("End of track unexpectedly reached.");

            temp = trackData[trackIndex];
            trackIndex++;
            result <<= 7;
            result |= temp & 0x7F;
        } while ((temp & 0x80) == 0x80);

        return result;
    }
}