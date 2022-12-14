#region

using System;
using System.Text;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Provides functionality for building meta text messages.
/// </summary>
public sealed class MetaTextBuilder : IMessageBuilder
{
    #region IMessageBuilder Members

    /// <summary>
    ///     Builds the text MetaMessage.
    /// </summary>
    public void Build()
    {
        // If the text has changed since the last time this method was
        // called.
        if (!changed) return;
        //
        // Build text MetaMessage.
        //

        var encoding = new ASCIIEncoding();
        var data = encoding.GetBytes(text);
        Result = new MetaMessage(Type, data);
        changed = false;
    }

    #endregion

    #region MetaTextBuilder Members

    #region Fields

    // The text represented by the MetaMessage.
    private string text;

    // The MetaMessage type - must be one of the text based types.
    private MetaType type = MetaType.Text;

    // The built MetaMessage.

    // Indicates whether or not the text has changed since the message was
    // last built.
    private bool changed = true;

    #endregion

    #region Construction

    /// <summary>
    ///     Initializes a new instance of the MetaMessageTextBuilder class.
    /// </summary>
    public MetaTextBuilder()
    {
        text = string.Empty;
    }

    /// <summary>
    ///     Initializes a new instance of the MetaMessageTextBuilder class with the
    ///     specified type.
    /// </summary>
    /// <param name="type">
    ///     The type of MetaMessage.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     If the MetaMessage type is not a text based type.
    /// </exception>
    /// <remarks>
    ///     The MetaMessage type must be one of the following text based
    ///     types:
    ///     <list>
    ///         <item>
    ///             Copyright
    ///         </item>
    ///         <item>
    ///             Cuepoint
    ///         </item>
    ///         <item>
    ///             DeviceName
    ///         </item>
    ///         <item>
    ///             InstrumentName
    ///         </item>
    ///         <item>
    ///             Lyric
    ///         </item>
    ///         <item>
    ///             Marker
    ///         </item>
    ///         <item>
    ///             ProgramName
    ///         </item>
    ///         <item>
    ///             Text
    ///         </item>
    ///         <item>
    ///             TrackName
    ///         </item>
    ///     </list>
    ///     If the MetaMessage is not a text based type, an exception
    ///     will be thrown.
    /// </remarks>
    public MetaTextBuilder(MetaType type)
    {
        #region Require

        if (!IsTextType(type))
            throw new ArgumentException("Not text based meta message type.",
                "message");

        #endregion

        text = string.Empty;
    }

    /// <summary>
    ///     Initializes a new instance of the MetaMessageTextBuilder class with the
    ///     specified type.
    /// </summary>
    /// <param name="type">
    ///     The type of MetaMessage.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     If the MetaMessage type is not a text based type.
    /// </exception>
    /// <remarks>
    ///     The MetaMessage type must be one of the following text based
    ///     types:
    ///     <list>
    ///         <item>
    ///             Copyright
    ///         </item>
    ///         <item>
    ///             Cuepoint
    ///         </item>
    ///         <item>
    ///             DeviceName
    ///         </item>
    ///         <item>
    ///             InstrumentName
    ///         </item>
    ///         <item>
    ///             Lyric
    ///         </item>
    ///         <item>
    ///             Marker
    ///         </item>
    ///         <item>
    ///             ProgramName
    ///         </item>
    ///         <item>
    ///             Text
    ///         </item>
    ///         <item>
    ///             TrackName
    ///         </item>
    ///     </list>
    ///     If the MetaMessage is not a text based type, an exception
    ///     will be thrown.
    /// </remarks>
    public MetaTextBuilder(MetaType type, string text)
    {
        #region Require

        if (!IsTextType(type))
            throw new ArgumentException("Not text based meta message type.",
                "message");

        #endregion

        this.type = type;

        this.text = text ?? string.Empty;
    }


    /// <summary>
    ///     Initializes a new instance of the MetaMessageTextBuilder class with the
    ///     specified MetaMessage.
    /// </summary>
    /// <param name="message">
    ///     The MetaMessage to use for initializing the MetaMessageTextBuilder.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     If the MetaMessage is not a text based type.
    /// </exception>
    /// <remarks>
    ///     The MetaMessage must be one of the following text based types:
    ///     <list>
    ///         <item>
    ///             Copyright
    ///         </item>
    ///         <item>
    ///             Cuepoint
    ///         </item>
    ///         <item>
    ///             DeviceName
    ///         </item>
    ///         <item>
    ///             InstrumentName
    ///         </item>
    ///         <item>
    ///             Lyric
    ///         </item>
    ///         <item>
    ///             Marker
    ///         </item>
    ///         <item>
    ///             ProgramName
    ///         </item>
    ///         <item>
    ///             Text
    ///         </item>
    ///         <item>
    ///             TrackName
    ///         </item>
    ///     </list>
    ///     If the MetaMessage is not a text based type, an exception will be
    ///     thrown.
    /// </remarks>
    public MetaTextBuilder(MetaMessage message)
    {
        Initialize(message);
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Initializes the MetaMessageTextBuilder with the specified MetaMessage.
    /// </summary>
    /// <param name="message">
    ///     The MetaMessage to use for initializing the MetaMessageTextBuilder.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     If the MetaMessage is not a text based type.
    /// </exception>
    public void Initialize(MetaMessage message)
    {
        #region Require

        if (!IsTextType(message.MetaType))
            throw new ArgumentException("Not text based meta message.",
                nameof(message));

        #endregion

        var encoding = new UTF8Encoding();

        text = encoding.GetString(message.GetBytes());
        type = message.MetaType;
    }

    /// <summary>
    ///     Indicates whether or not the specified MetaType is a text based
    ///     type.
    /// </summary>
    /// <param name="type">
    ///     The MetaType to test.
    /// </param>
    /// <returns>
    ///     <b>true</b> if the MetaType is a text based type;
    ///     otherwise, <b>false</b>.
    /// </returns>
    private static bool IsTextType(MetaType type)
    {
        bool result;

        if (type == MetaType.Copyright ||
            type == MetaType.CuePoint ||
            type == MetaType.DeviceName ||
            type == MetaType.InstrumentName ||
            type == MetaType.Lyric ||
            type == MetaType.Marker ||
            type == MetaType.ProgramName ||
            type == MetaType.Text ||
            type == MetaType.TrackName)
            result = true;
        else
            result = false;

        return result;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Gets or sets the text for the MetaMessage.
    /// </summary>
    public string Text
    {
        get => text;
        set
        {
            text = value ?? string.Empty;

            changed = true;
        }
    }

    /// <summary>
    ///     Gets or sets the MetaMessage type.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     If the type is not a text based type.
    /// </exception>
    public MetaType Type
    {
        get { return type; }
        set
        {
            #region Require

            if (!IsTextType(value))
                throw new ArgumentException("Not text based meta message type.",
                    "message");

            #endregion

            type = value;

            changed = true;
        }
    }

    /// <summary>
    ///     Gets the built MetaMessage.
    /// </summary>
    public MetaMessage Result { get; private set; }

    #endregion

    #endregion
}