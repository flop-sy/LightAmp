namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Defines constants representing MIDI message types.
    /// </summary>
    public enum MessageType
    {
        Channel,

        SystemExclusive,

        SystemCommon,

        SystemRealtime,

        Meta,

        Short
    }

    /// <summary>
    ///     Represents the basic functionality for all MIDI messages.
    /// </summary>
    public interface IMidiMessage
    {
        /// <summary>
        ///     Gets the MIDI message's status value.
        /// </summary>
        int Status { get; }

        /// <summary>
        ///     Gets the MIDI event's type.
        /// </summary>
        MessageType MessageType { get; }

        /// <summary>
        ///     Delta samples when the event should be processed in the next audio buffer.
        ///     Leave at 0 for realtime input to play as fast as possible.
        ///     Set to the desired sample in the next buffer if you play a midi sequence synchronized to the audio callback
        /// </summary>
        int DeltaFrames { get; }

        /// <summary>
        ///     Gets a byte array representation of the MIDI message.
        /// </summary>
        /// <returns>
        ///     A byte array representation of the MIDI message.
        /// </returns>
        byte[] GetBytes();
    }
}