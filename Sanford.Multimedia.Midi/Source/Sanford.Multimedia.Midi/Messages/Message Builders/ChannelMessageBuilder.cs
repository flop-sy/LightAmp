#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Provides functionality for building ChannelMessages.
    /// </summary>
    public sealed class ChannelMessageBuilder : IMessageBuilder
    {
        #region IMessageBuilder Members

        /// <summary>
        ///     Builds a ChannelMessageEventArgs.
        /// </summary>
        public void Build()
        {
            Result = (ChannelMessage)messageCache[Message];

            // If the message does not exist.
            if (Result != null) return;

            Result = new ChannelMessage(Message);

            // Add message to cache.
            messageCache.Add(Message, Result);
        }

        #endregion

        #region ChannelMessageBuilder Members

        #region Class Fields

        // Stores the ChannelMessages.
        private static readonly Hashtable messageCache = Hashtable.Synchronized(new Hashtable());

        #endregion

        #region Fields

        // The channel message as a packed integer.

        // The built ChannelMessage

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the ChannelMessageBuilder class.
        /// </summary>
        public ChannelMessageBuilder()
        {
            Command = ChannelCommand.Controller;
            MidiChannel = 0;
            Data1 = (int)ControllerType.AllSoundOff;
            Data2 = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the ChannelMessageBuilder class with
        ///     the specified ChannelMessageEventArgs.
        /// </summary>
        /// <param name="message">
        ///     The ChannelMessageEventArgs to use for initializing the ChannelMessageBuilder.
        /// </param>
        /// <remarks>
        ///     The ChannelMessageBuilder uses the specified ChannelMessageEventArgs to
        ///     initialize its property values.
        /// </remarks>
        public ChannelMessageBuilder(ChannelMessage message)
        {
            Initialize(message);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the ChannelMessageBuilder with the specified
        ///     ChannelMessageEventArgs.
        /// </summary>
        /// <param name="message">
        ///     The ChannelMessageEventArgs to use for initializing the ChannelMessageBuilder.
        /// </param>
        public void Initialize(ChannelMessage message)
        {
            Message = message.Message;
        }

        /// <summary>
        ///     Clears the ChannelMessageEventArgs cache.
        /// </summary>
        public static void Clear()
        {
            messageCache.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the number of messages in the ChannelMessageEventArgs cache.
        /// </summary>
        public static int Count => messageCache.Count;

        /// <summary>
        ///     Gets the built ChannelMessageEventArgs.
        /// </summary>
        public ChannelMessage Result { get; private set; }

        /// <summary>
        ///     Gets or sets the ChannelMessageEventArgs as a packed integer.
        /// </summary>
        internal int Message { get; set; }

        /// <summary>
        ///     Gets or sets the Command value to use for building the
        ///     ChannelMessageEventArgs.
        /// </summary>
        public ChannelCommand Command
        {
            get => ChannelMessage.UnpackCommand(Message);
            set => Message = ChannelMessage.PackCommand(Message, value);
        }

        /// <summary>
        ///     Gets or sets the MIDI channel to use for building the
        ///     ChannelMessageEventArgs.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     MidiChannel is set to a value less than zero or greater than 15.
        /// </exception>
        public int MidiChannel
        {
            get => ChannelMessage.UnpackMidiChannel(Message);
            set => Message = ChannelMessage.PackMidiChannel(Message, value);
        }

        /// <summary>
        ///     Gets or sets the first data value to use for building the
        ///     ChannelMessageEventArgs.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Data1 is set to a value less than zero or greater than 127.
        /// </exception>
        public int Data1
        {
            get => ShortMessage.UnpackData1(Message);
            set => Message = ShortMessage.PackData1(Message, value);
        }

        /// <summary>
        ///     Gets or sets the second data value to use for building the
        ///     ChannelMessageEventArgs.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Data2 is set to a value less than zero or greater than 127.
        /// </exception>
        public int Data2
        {
            get => ShortMessage.UnpackData2(Message);
            set => Message = ShortMessage.PackData2(Message, value);
        }

        #endregion

        #endregion
    }
}