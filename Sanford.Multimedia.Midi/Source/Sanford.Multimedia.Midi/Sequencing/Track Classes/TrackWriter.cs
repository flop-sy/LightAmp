#region

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Writes a Track to a Stream.
/// </summary>
internal sealed class TrackWriter
{
    private static readonly byte[] TrackHeader =
    {
        (byte)'M',
        (byte)'T',
        (byte)'r',
        (byte)'k'
    };

    // The Track data in raw bytes.
    private readonly List<byte> trackData = new();

    // Running status.
    private int runningStatus;

    // The Stream to write to.
    private Stream stream;

    // The Track to write to the Stream.
    private Track track = new();

    /// <summary>
    ///     Gets or sets the Track to write to the Stream.
    /// </summary>
    public Track Track
    {
        get { return track; }
        set
        {
            #region Require

            if (value == null) throw new ArgumentNullException("Track");

            #endregion

            runningStatus = 0;
            trackData.Clear();

            track = value;
        }
    }

    public void Write(Stream strm)
    {
        stream = strm;

        trackData.Clear();

        stream.Write(TrackHeader, 0, TrackHeader.Length);

        foreach (var e in track.Iterator())
        {
            WriteVariableLengthValue(e.DeltaTicks);

            switch (e.MidiMessage.MessageType)
            {
                case MessageType.Channel:
                    Write((ChannelMessage)e.MidiMessage);
                    break;

                case MessageType.SystemExclusive:
                    Write((SysExMessage)e.MidiMessage);
                    break;

                case MessageType.Meta:
                    Write((MetaMessage)e.MidiMessage);
                    break;

                case MessageType.SystemCommon:
                    Write((SysCommonMessage)e.MidiMessage);
                    break;

                case MessageType.SystemRealtime:
                    Write((SysRealtimeMessage)e.MidiMessage);
                    break;

                case MessageType.Short:
                    Write((ShortMessage)e.MidiMessage);
                    break;
            }
        }

        var trackLength = BitConverter.GetBytes(trackData.Count);

        if (BitConverter.IsLittleEndian) Array.Reverse(trackLength);

        stream.Write(trackLength, 0, trackLength.Length);

        foreach (var b in trackData) stream.WriteByte(b);
    }

    private void WriteVariableLengthValue(int value)
    {
        var v = value;
        var array = new byte[4];
        var count = 0;

        array[0] = (byte)(v & 0x7F);

        v >>= 7;

        while (v > 0)
        {
            count++;
            array[count] = (byte)((v & 0x7F) | 0x80);
            v >>= 7;
        }

        while (count >= 0)
        {
            trackData.Add(array[count]);
            count--;
        }
    }

    private void Write(IMidiMessage message)
    {
        trackData.AddRange(message.GetBytes());
    }

    private void Write(ChannelMessage message)
    {
        if (runningStatus != message.Status)
        {
            trackData.Add((byte)message.Status);
            runningStatus = message.Status;
        }

        trackData.Add((byte)message.Data1);

        if (ChannelMessage.DataBytesPerType(message.Command) == 2) trackData.Add((byte)message.Data2);
    }

    private void Write(SysExMessage message)
    {
        // System exclusive message cancel running status.
        runningStatus = 0;

        trackData.Add((byte)message.Status);

        WriteVariableLengthValue(message.Length - 1);

        for (var i = 1; i < message.Length; i++) trackData.Add(message[i]);
    }

    private void Write(MetaMessage message)
    {
        trackData.Add((byte)message.Status);
        trackData.Add((byte)message.MetaType);

        WriteVariableLengthValue(message.Length);

        trackData.AddRange(message.GetBytes());
    }

    private void Write(SysCommonMessage message)
    {
        // Escaped messages cancel running status.
        runningStatus = 0;

        // Escaped message.
        trackData.Add(0xF7);

        trackData.Add((byte)message.Status);

        switch (message.SysCommonType)
        {
            case SysCommonType.MidiTimeCode:
                trackData.Add((byte)message.Data1);
                break;

            case SysCommonType.SongPositionPointer:
                trackData.Add((byte)message.Data1);
                trackData.Add((byte)message.Data2);
                break;

            case SysCommonType.SongSelect:
                trackData.Add((byte)message.Data1);
                break;
        }
    }

    private void Write(SysRealtimeMessage message)
    {
        // Escaped messages cancel running status.
        runningStatus = 0;

        // Escaped message.
        trackData.Add(0xF7);

        trackData.Add((byte)message.Status);
    }
}