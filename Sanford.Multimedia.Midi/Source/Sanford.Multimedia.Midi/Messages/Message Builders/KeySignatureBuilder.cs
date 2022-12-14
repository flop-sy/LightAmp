#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Builds key signature MetaMessages.
/// </summary>
public class KeySignatureBuilder : IMessageBuilder
{
    /// <summary>
    ///     Initializes a new instance of the KeySignatureBuilder class.
    /// </summary>
    public KeySignatureBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the KeySignatureBuilder class with
    ///     the specified key signature MetaMessage.
    /// </summary>
    /// <param name="message">
    ///     The key signature MetaMessage to use for initializing the
    ///     KeySignatureBuilder class.
    /// </param>
    public KeySignatureBuilder(MetaMessage message)
    {
        Initialize(message);
    }

    /// <summary>
    ///     Gets or sets the key.
    /// </summary>
    public Key Key { get; set; } = Key.CMajor;

    /// <summary>
    ///     The build key signature MetaMessage.
    /// </summary>
    public MetaMessage Result { get; private set; }

    #region IMessageBuilder Members

    /// <summary>
    ///     Builds the key signature MetaMessage.
    /// </summary>
    public void Build()
    {
        var data = new byte[MetaMessage.KeySigLength];

        unchecked
        {
            switch (Key)
            {
                case Key.CFlatMajor:
                    data[0] = (byte)-7;
                    data[1] = 0;
                    break;

                case Key.GFlatMajor:
                    data[0] = (byte)-6;
                    data[1] = 0;
                    break;

                case Key.DFlatMajor:
                    data[0] = (byte)-5;
                    data[1] = 0;
                    break;

                case Key.AFlatMajor:
                    data[0] = (byte)-4;
                    data[1] = 0;
                    break;

                case Key.EFlatMajor:
                    data[0] = (byte)-3;
                    data[1] = 0;
                    break;

                case Key.BFlatMajor:
                    data[0] = (byte)-2;
                    data[1] = 0;
                    break;

                case Key.FMajor:
                    data[0] = (byte)-1;
                    data[1] = 0;
                    break;

                case Key.CMajor:
                    data[0] = 0;
                    data[1] = 0;
                    break;

                case Key.GMajor:
                    data[0] = 1;
                    data[1] = 0;
                    break;

                case Key.DMajor:
                    data[0] = 2;
                    data[1] = 0;
                    break;

                case Key.AMajor:
                    data[0] = 3;
                    data[1] = 0;
                    break;

                case Key.EMajor:
                    data[0] = 4;
                    data[1] = 0;
                    break;

                case Key.BMajor:
                    data[0] = 5;
                    data[1] = 0;
                    break;

                case Key.FSharpMajor:
                    data[0] = 6;
                    data[1] = 0;
                    break;

                case Key.CSharpMajor:
                    data[0] = 7;
                    data[1] = 0;
                    break;

                case Key.AFlatMinor:
                    data[0] = (byte)-7;
                    data[1] = 1;
                    break;

                case Key.EFlatMinor:
                    data[0] = (byte)-6;
                    data[1] = 1;
                    break;

                case Key.BFlatMinor:
                    data[0] = (byte)-5;
                    data[1] = 1;
                    break;

                case Key.FMinor:
                    data[0] = (byte)-4;
                    data[1] = 1;
                    break;

                case Key.CMinor:
                    data[0] = (byte)-3;
                    data[1] = 1;
                    break;

                case Key.GMinor:
                    data[0] = (byte)-2;
                    data[1] = 1;
                    break;

                case Key.DMinor:
                    data[0] = (byte)-1;
                    data[1] = 1;
                    break;

                case Key.AMinor:
                    data[0] = 1;
                    data[1] = 0;
                    break;

                case Key.EMinor:
                    data[0] = 1;
                    data[1] = 1;
                    break;

                case Key.BMinor:
                    data[0] = 2;
                    data[1] = 1;
                    break;

                case Key.FSharpMinor:
                    data[0] = 3;
                    data[1] = 1;
                    break;

                case Key.CSharpMinor:
                    data[0] = 4;
                    data[1] = 1;
                    break;

                case Key.GSharpMinor:
                    data[0] = 5;
                    data[1] = 1;
                    break;

                case Key.DSharpMinor:
                    data[0] = 6;
                    data[1] = 1;
                    break;

                case Key.ASharpMinor:
                    data[0] = 7;
                    data[1] = 1;
                    break;
            }
        }

        Result = new MetaMessage(MetaType.KeySignature, data);
    }

    #endregion

    /// <summary>
    ///     Initializes the KeySignatureBuilder with the specified MetaMessage.
    /// </summary>
    /// <param name="message">
    ///     The key signature MetaMessage to use for initializing the
    ///     KeySignatureBuilder.
    /// </param>
    public void Initialize(MetaMessage message)
    {
        #region Require

        if (message == null) throw new ArgumentNullException(nameof(message));

        if (message.MetaType != MetaType.KeySignature)
            throw new ArgumentException("Wrong meta event type.", "messaege");

        #endregion

        var b = (sbyte)message[0];

        // If the key is major.
        if (message[1] == 0)
            Key = b switch
            {
                -7 => Key.CFlatMajor,
                -6 => Key.GFlatMajor,
                -5 => Key.DFlatMajor,
                -4 => Key.AFlatMajor,
                -3 => Key.EFlatMajor,
                -2 => Key.BFlatMajor,
                -1 => Key.FMajor,
                0 => Key.CMajor,
                1 => Key.GMajor,
                2 => Key.DMajor,
                3 => Key.AMajor,
                4 => Key.EMajor,
                5 => Key.BMajor,
                6 => Key.FSharpMajor,
                7 => Key.CSharpMajor,
                _ => Key
            };
        // Else the key is minor.
        else
            Key = b switch
            {
                -7 => Key.AFlatMinor,
                -6 => Key.EFlatMinor,
                -5 => Key.BFlatMinor,
                -4 => Key.FMinor,
                -3 => Key.CMinor,
                -2 => Key.GMinor,
                -1 => Key.DMinor,
                0 => Key.AMinor,
                1 => Key.EMinor,
                2 => Key.BMinor,
                3 => Key.FSharpMinor,
                4 => Key.CSharpMinor,
                5 => Key.GSharpMinor,
                6 => Key.DSharpMinor,
                7 => Key.ASharpMinor,
                _ => Key
            };
    }
}