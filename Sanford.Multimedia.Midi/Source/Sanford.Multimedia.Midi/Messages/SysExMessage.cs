#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Defines constants representing various system exclusive message types.
/// </summary>
public enum SysExType
{
    /// <summary>
    ///     Represents the start of system exclusive message type.
    /// </summary>
    Start = 0xF0,

    /// <summary>
    ///     Represents the continuation of a system exclusive message.
    /// </summary>
    Continuation = 0xF7
}

/// <summary>
///     Represents MIDI system exclusive messages.
/// </summary>
public sealed class SysExMessage : MidiMessageBase, IMidiMessage, IEnumerable
{
    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
        return data.GetEnumerator();
    }

    #endregion

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public int Status => data[0];

    /// <summary>
    ///     Gets the MessageType.
    /// </summary>
    public MessageType MessageType => MessageType.SystemExclusive;

    #region SysExEventMessage Members

    #region Constants

    /// <summary>
    ///     Maximum value for system exclusive channels.
    /// </summary>
    public const int SysExChannelMaxValue = 127;

    #endregion

    #region Fields

    // The system exclusive data.
    private readonly byte[] data;

    #endregion

    #region Construction

    /// <summary>
    ///     Initializes a new instance of the SysExMessageEventArgs class with the
    ///     specified system exclusive data.
    /// </summary>
    /// <param name="data">
    ///     The system exclusive data.
    /// </param>
    /// <remarks>
    ///     The system exclusive data's status byte, the first byte in the
    ///     data, must have a value of 0xF0 or 0xF7.
    /// </remarks>
    public SysExMessage(byte[] data)
    {
        #region Require

        if (data.Length < 1)
            throw new ArgumentException(
                "System exclusive data is too short.", nameof(data));

        if (data[0] != (byte)SysExType.Start &&
            data[0] != (byte)SysExType.Continuation)
            throw new ArgumentException(
                "Unknown status value.", nameof(data));

        #endregion

        this.data = new byte[data.Length];
        data.CopyTo(this.data, 0);
    }

    #endregion

    #region Methods

    public byte[] GetBytes()
    {
        var clone = new byte[data.Length];

        data.CopyTo(clone, 0);

        return clone;
    }

    public void CopyTo(byte[] buffer, int index)
    {
        data.CopyTo(buffer, index);
    }

    public override bool Equals(object obj)
    {
        #region Guard

        if (obj is not SysExMessage message) return false;

        #endregion

        var equals = Length == message.Length;

        for (var i = 0; i < Length && equals; i++)
            if (this[i] != message[i])
                equals = false;

        return equals;
    }

    public override int GetHashCode()
    {
        return data.GetHashCode();
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets the timestamp of the midi input driver in milliseconds since the midi input driver was started.
    /// </summary>
    /// <value>
    ///     The timestamp in milliseconds since the midi input driver was started.
    /// </value>
    public int Timestamp { get; internal set; }

    /// <summary>
    ///     Gets the element at the specified index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If index is less than zero or greater than or equal to the length
    ///     of the message.
    /// </exception>
    public byte this[int index]
    {
        get
        {
            #region Require

            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    "Index into system exclusive message out of range.");

            #endregion

            return data[index];
        }
    }

    /// <summary>
    ///     Gets the length of the system exclusive data.
    /// </summary>
    public int Length => data.Length;

    /// <summary>
    ///     Gets the system exclusive type.
    /// </summary>
    public SysExType SysExType => (SysExType)data[0];

    #endregion

    #endregion
}