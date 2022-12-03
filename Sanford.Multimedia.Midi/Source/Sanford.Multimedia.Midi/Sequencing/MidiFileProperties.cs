#region

using System;
using System.Diagnostics;
using System.IO;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Defintes constants representing SMPTE frame rates.
    /// </summary>
    public enum SmpteFrameRate
    {
        Smpte24 = 24,
        Smpte25 = 25,
        Smpte30Drop = 29,
        Smpte30 = 30
    }

    /// <summary>
    ///     The different types of sequences.
    /// </summary>
    public enum SequenceType
    {
        Ppqn,
        Smpte
    }

    /// <summary>
    ///     Represents MIDI file properties.
    /// </summary>
    internal sealed class MidiFileProperties
    {
        private const int PropertyLength = 2;

        private static readonly byte[] MidiFileHeader =
        {
            (byte)'M',
            (byte)'T',
            (byte)'h',
            (byte)'d',
            0,
            0,
            0,
            6
        };

        private int division = PpqnClock.PpqnMinValue;

        private int format = 1;

        private int trackCount;

        public int Format
        {
            get { return format; }
            set
            {
                #region Require

                if (value < 0 || value > 3)
                    throw new ArgumentOutOfRangeException("Format", value,
                        "MIDI file format out of range.");

                if (value == 0 && trackCount > 1)
                    throw new ArgumentException(
                        "MIDI file format invalid for this track count.");

                #endregion

                format = value;

                #region Invariant

                AssertValid();

                #endregion
            }
        }

        public int TrackCount
        {
            get { return trackCount; }
            set
            {
                #region Require

                if (value < 0)
                    throw new ArgumentOutOfRangeException("TrackCount", value,
                        "Track count out of range.");

                if (value > 1 && Format == 0)
                    throw new ArgumentException(
                        "Track count invalid for this format.");

                #endregion

                trackCount = value;

                #region Invariant

                AssertValid();

                #endregion
            }
        }

        public int Division
        {
            get { return division; }
            set
            {
                if (IsSmpte(value))
                {
                    var data = BitConverter.GetBytes((short)value);

                    if (BitConverter.IsLittleEndian) Array.Reverse(data);

                    if ((sbyte)data[0] != -(int)SmpteFrameRate.Smpte24 &&
                        (sbyte)data[0] != -(int)SmpteFrameRate.Smpte25 &&
                        (sbyte)data[0] != -(int)SmpteFrameRate.Smpte30 &&
                        (sbyte)data[0] != -(int)SmpteFrameRate.Smpte30Drop)
                        throw new ArgumentException("Invalid SMPTE frame rate.");

                    SequenceType = SequenceType.Smpte;
                }
                else
                {
                    if (value < PpqnClock.PpqnMinValue)
                        throw new ArgumentOutOfRangeException("Ppqn", value,
                            "Pulses per quarter note is smaller than 24.");

                    SequenceType = SequenceType.Ppqn;
                }

                division = value;

                #region Invariant

                AssertValid();

                #endregion
            }
        }

        public SequenceType SequenceType { get; private set; } = SequenceType.Ppqn;

        public void Read(Stream strm)
        {
            #region Require

            if (strm == null) throw new ArgumentNullException(nameof(strm));

            #endregion

            format = trackCount = 0;
            division = PpqnClock.PpqnMinValue;

            FindHeader(strm);
            Format = ReadProperty(strm);
            TrackCount = ReadProperty(strm);
            Division = ReadProperty(strm);

            #region Invariant

            AssertValid();

            #endregion
        }

        private void FindHeader(Stream stream)
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

                        if (result == 'h')
                        {
                            result = stream.ReadByte();

                            if (result == 'd') found = true;
                        }
                    }
                }

                if (result < 0) throw new MidiFileException("Unable to find MIDI file header.");
            }

            // Eat the header length.
            for (var i = 0; i < 4; i++)
                if (stream.ReadByte() < 0)
                    throw new MidiFileException("Unable to find MIDI file header.");
        }

        private ushort ReadProperty(Stream strm)
        {
            var data = new byte[PropertyLength];

            var result = strm.Read(data, 0, data.Length);

            if (result != data.Length) throw new MidiFileException("End of MIDI file unexpectedly reached.");

            if (BitConverter.IsLittleEndian) Array.Reverse(data);

            return BitConverter.ToUInt16(data, 0);
        }

        public void Write(Stream strm)
        {
            #region Require

            if (strm == null) throw new ArgumentNullException(nameof(strm));

            #endregion

            strm.Write(MidiFileHeader, 0, MidiFileHeader.Length);
            WriteProperty(strm, (ushort)Format);
            WriteProperty(strm, (ushort)TrackCount);
            WriteProperty(strm, (ushort)Division);
        }

        private void WriteProperty(Stream strm, ushort property)
        {
            var data = BitConverter.GetBytes(property);

            if (BitConverter.IsLittleEndian) Array.Reverse(data);

            strm.Write(data, 0, PropertyLength);
        }

        private static bool IsSmpte(int division)
        {
            var data = BitConverter.GetBytes((short)division);

            if (BitConverter.IsLittleEndian) Array.Reverse(data);

            var result = (sbyte)data[0] < 0;

            return result;
        }

        [Conditional("MIDIDEBUG")]
        private void AssertValid()
        {
            if (trackCount > 1) Debug.Assert(Format == 1 || Format == 2);

            if (IsSmpte(Division))
            {
                Debug.Assert(SequenceType == SequenceType.Smpte);
            }
            else
            {
                Debug.Assert(SequenceType == SequenceType.Ppqn);
                Debug.Assert(Division >= PpqnClock.PpqnMinValue);
            }
        }
    }

    public sealed class MidiFileException : ApplicationException
    {
        public MidiFileException(string message) : base(message)
        {
        }
    }
}