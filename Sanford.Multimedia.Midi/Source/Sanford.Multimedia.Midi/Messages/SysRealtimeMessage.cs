#region

using System.ComponentModel;
using System.Diagnostics;

#endregion

namespace Sanford.Multimedia.Midi;

#region System Realtime Message Types

/// <summary>
///     Defines constants representing the various system realtime message types.
/// </summary>
public enum SysRealtimeType
{
    /// <summary>
    ///     Represents the clock system realtime type.
    /// </summary>
    Clock = 0xF8,

    /// <summary>
    ///     Represents the tick system realtime type.
    /// </summary>
    Tick,

    /// <summary>
    ///     Represents the start system realtime type.
    /// </summary>
    Start,

    /// <summary>
    ///     Represents the continue system realtime type.
    /// </summary>
    Continue,

    /// <summary>
    ///     Represents the stop system realtime type.
    /// </summary>
    Stop,

    /// <summary>
    ///     Represents the active sense system realtime type.
    /// </summary>
    ActiveSense = 0xFE,

    /// <summary>
    ///     Represents the reset system realtime type.
    /// </summary>
    Reset
}

#endregion

/// <summary>
///     Represents MIDI system realtime messages.
/// </summary>
/// <remarks>
///     System realtime messages are MIDI messages that are primarily concerned
///     with controlling and synchronizing MIDI devices.
/// </remarks>
[ImmutableObject(true)]
public sealed class SysRealtimeMessage : ShortMessage
{
    #region SysRealtimeMessage Members

    #region System Realtime Messages

    /// <summary>
    ///     The instance of the system realtime start message.
    /// </summary>
    public static readonly SysRealtimeMessage StartMessage = new(SysRealtimeType.Start);

    /// <summary>
    ///     The instance of the system realtime continue message.
    /// </summary>
    public static readonly SysRealtimeMessage ContinueMessage = new(SysRealtimeType.Continue);

    /// <summary>
    ///     The instance of the system realtime stop message.
    /// </summary>
    public static readonly SysRealtimeMessage StopMessage = new(SysRealtimeType.Stop);

    /// <summary>
    ///     The instance of the system realtime clock message.
    /// </summary>
    public static readonly SysRealtimeMessage ClockMessage = new(SysRealtimeType.Clock);

    /// <summary>
    ///     The instance of the system realtime tick message.
    /// </summary>
    public static readonly SysRealtimeMessage TickMessage = new(SysRealtimeType.Tick);

    /// <summary>
    ///     The instance of the system realtime active sense message.
    /// </summary>
    public static readonly SysRealtimeMessage ActiveSenseMessage = new(SysRealtimeType.ActiveSense);

    /// <summary>
    ///     The instance of the system realtime reset message.
    /// </summary>
    public static readonly SysRealtimeMessage ResetMessage = new(SysRealtimeType.Reset);

    #endregion

    // Make construction private so that a system realtime message cannot
    // be constructed directly.
    private SysRealtimeMessage(SysRealtimeType type)
    {
        msg = (int)type;

        #region Ensure

        Debug.Assert(SysRealtimeType == type);

        #endregion
    }

    #region Methods

    /// <summary>
    ///     Returns a value for the current SysRealtimeMessage suitable for use in
    ///     hashing algorithms.
    /// </summary>
    /// <returns>
    ///     A hash code for the current SysRealtimeMessage.
    /// </returns>
    public override int GetHashCode()
    {
        return msg;
    }

    /// <summary>
    ///     Determines whether two SysRealtimeMessage instances are equal.
    /// </summary>
    /// <param name="obj">
    ///     The SysRealtimeMessage to compare with the current SysRealtimeMessage.
    /// </param>
    /// <returns>
    ///     <b>true</b> if the specified SysRealtimeMessage is equal to the current
    ///     SysRealtimeMessage; otherwise, <b>false</b>.
    /// </returns>
    public override bool Equals(object obj)
    {
        #region Guard

        if (obj is not SysRealtimeMessage realtimeMessage) return false;

        #endregion

        return msg == realtimeMessage.msg;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets the SysRealtimeType.
    /// </summary>
    public SysRealtimeType SysRealtimeType => (SysRealtimeType)msg;

    /// <summary>
    ///     Gets the MessageType.
    /// </summary>
    public override MessageType MessageType => MessageType.SystemRealtime;

    #endregion

    #endregion
}