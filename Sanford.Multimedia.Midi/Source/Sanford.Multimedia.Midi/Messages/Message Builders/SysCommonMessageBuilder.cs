#region

using System;
using System.Collections;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Provides functionality for building SysCommonMessages.
    /// </summary>
    public sealed class SysCommonMessageBuilder : IMessageBuilder
    {
        #region IMessageBuilder Members

        /// <summary>
        ///     Builds a SysCommonMessage.
        /// </summary>
        public void Build()
        {
            Result = (SysCommonMessage)messageCache[Message];

            if (Result != null) return;

            Result = new SysCommonMessage(Message);

            messageCache.Add(Message, Result);
        }

        #endregion

        #region SysCommonMessageBuilder Members

        #region Class Fields

        // Stores the SystemCommonMessages.
        private static readonly Hashtable messageCache = Hashtable.Synchronized(new Hashtable());

        #endregion

        #region Fields

        // The SystemCommonMessage as a packed integer.

        // The built SystemCommonMessage.

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the SysCommonMessageBuilder class.
        /// </summary>
        public SysCommonMessageBuilder()
        {
            Type = SysCommonType.TuneRequest;
        }

        /// <summary>
        ///     Initializes a new instance of the SysCommonMessageBuilder class
        ///     with the specified SystemCommonMessage.
        /// </summary>
        /// <param name="message">
        ///     The SysCommonMessage to use for initializing the
        ///     SysCommonMessageBuilder.
        /// </param>
        /// <remarks>
        ///     The SysCommonMessageBuilder uses the specified SysCommonMessage to
        ///     initialize its property values.
        /// </remarks>
        public SysCommonMessageBuilder(SysCommonMessage message)
        {
            Initialize(message);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the SysCommonMessageBuilder with the specified
        ///     SysCommonMessage.
        /// </summary>
        /// <param name="message">
        ///     The SysCommonMessage to use for initializing the
        ///     SysCommonMessageBuilder.
        /// </param>
        public void Initialize(SysCommonMessage message)
        {
            Message = message.Message;
        }

        /// <summary>
        ///     Clears the SysCommonMessageBuilder cache.
        /// </summary>
        public static void Clear()
        {
            messageCache.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the number of messages in the SysCommonMessageBuilder cache.
        /// </summary>
        public static int Count => messageCache.Count;

        /// <summary>
        ///     Gets the built SysCommonMessage.
        /// </summary>
        public SysCommonMessage Result { get; private set; }

        /// <summary>
        ///     Gets or sets the SysCommonMessage as a packed integer.
        /// </summary>
        internal int Message { get; set; }

        /// <summary>
        ///     Gets or sets the type of SysCommonMessage.
        /// </summary>
        public SysCommonType Type
        {
            get => (SysCommonType)ShortMessage.UnpackStatus(Message);
            set => Message = ShortMessage.PackStatus(Message, (int)value);
        }

        /// <summary>
        ///     Gets or sets the first data value to use for building the
        ///     SysCommonMessage.
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
        ///     SysCommonMessage.
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