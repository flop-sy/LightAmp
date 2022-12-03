#region

using System;
using System.Collections.Generic;

#endregion

namespace Sanford.Multimedia.Midi
{
    public abstract class MidiMessageBase
    {
        /// <summary>
        ///     Delta samples when the event should be processed in the next audio buffer.
        ///     Leave at 0 for realtime input to play as fast as possible.
        ///     Set to the desired sample in the next buffer if you play a midi sequence synchronized to the audio callback
        /// </summary>
        public int DeltaFrames { get; set; }
    }

    /// <summary>
    ///     Represents the basic class for all MIDI short messages.
    /// </summary>
    /// <remarks>
    ///     MIDI short messages represent all MIDI messages except meta messages
    ///     and system exclusive messages. This includes channel messages, system
    ///     realtime messages, and system common messages.
    /// </remarks>
    public class ShortMessage : MidiMessageBase, IMidiMessage
    {
        #region ShortMessage Members

        #region Constants

        public const int DataMaxValue = 127;

        public const int StatusMaxValue = 255;

        //
        // Bit manipulation constants.
        //

        private const int StatusMask = ~255;
        protected const int DataMask = ~StatusMask;
        private const int Data1Mask = ~65280;
        private const int Data2Mask = ~Data1Mask + DataMask;
        private const int Shift = 8;

        #endregion

        protected int msg;

        private byte[] message;
        private bool rawMessageBuilt;

        #region Methods

        public byte[] GetBytes()
        {
            return Bytes;
        }

        public ShortMessage()
        {
            //sub classes will fill the msg field
        }

        public ShortMessage(int message)
        {
            msg = message;
        }

        public ShortMessage(byte status, byte data1, byte data2)
        {
            message = new[] { status, data1, data2 };
            rawMessageBuilt = true;
            msg = BuildIntMessage(message);
        }

        private static byte[] BuildByteMessage(int intMessage)
        {
            unchecked
            {
                return new[]
                {
                    (byte)UnpackStatus(intMessage),
                    (byte)UnpackData1(intMessage),
                    (byte)UnpackData2(intMessage)
                };
            }
        }

        private static int BuildIntMessage(IReadOnlyList<byte> message)
        {
            var intMessage = 0;
            intMessage = PackStatus(intMessage, message[0]);
            intMessage = PackData1(intMessage, message[1]);
            intMessage = PackData2(intMessage, message[2]);
            return intMessage;
        }

        internal static int PackStatus(int message, int status)
        {
            #region Require

            if (status < 0 || status > StatusMaxValue)
                throw new ArgumentOutOfRangeException(nameof(status), status,
                    "Status value out of range.");

            #endregion

            return (message & StatusMask) | status;
        }

        internal static int PackData1(int message, int data1)
        {
            #region Require

            if (data1 < 0 || data1 > DataMaxValue)
                throw new ArgumentOutOfRangeException(nameof(data1), data1,
                    "Data 1 value out of range.");

            #endregion

            return (message & Data1Mask) | (data1 << Shift);
        }

        internal static int PackData2(int message, int data2)
        {
            #region Require

            if (data2 < 0 || data2 > DataMaxValue)
                throw new ArgumentOutOfRangeException(nameof(data2), data2,
                    "Data 2 value out of range.");

            #endregion

            return (message & Data2Mask) | (data2 << (Shift * 2));
        }

        internal static int UnpackStatus(int message)
        {
            return message & DataMask;
        }

        internal static int UnpackData1(int message)
        {
            return (message & ~Data1Mask) >> Shift;
        }

        internal static int UnpackData2(int message)
        {
            return (message & ~Data2Mask) >> (Shift * 2);
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
        ///     Gets the short message as a packed integer.
        /// </summary>
        /// <remarks>
        ///     The message is packed into an integer value with the low-order byte
        ///     of the low-word representing the status value. The high-order byte
        ///     of the low-word represents the first data value, and the low-order
        ///     byte of the high-word represents the second data value.
        /// </remarks>
        public int Message => msg;

        /// <summary>
        ///     Gets the messages's status value.
        /// </summary>
        public int Status => UnpackStatus(msg);

        public byte[] Bytes
        {
            get
            {
                if (rawMessageBuilt) return message;

                message = BuildByteMessage(msg);
                rawMessageBuilt = true;

                return message;
            }
        }

        public virtual MessageType MessageType => MessageType.Short;

        #endregion

        #endregion
    }
}