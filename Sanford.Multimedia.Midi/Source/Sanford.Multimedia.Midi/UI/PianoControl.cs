#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

#region

using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Sanford.Multimedia.Midi.UI;

public partial class PianoControl : Control
{
    private const int DefaultLowNoteID = 21;

    private const int DefaultHighNoteID = 109;

    private const double BlackKeyScale = 0.666666666;

    private static readonly Hashtable keyTable = new();

    private static readonly KeyType[] KeyTypeTable =
    {
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White,
        KeyType.White, KeyType.Black, KeyType.White, KeyType.Black, KeyType.White, KeyType.White, KeyType.Black,
        KeyType.White
    };

    private SynchronizationContext context;

    private int highNoteID = DefaultHighNoteID;

    private PianoKey[] keys;

    private int lowNoteID = DefaultLowNoteID;

    private readonly NoteMessageCallback noteOffCallback;

    private readonly NoteMessageCallback noteOnCallback;

    private Color noteOnColor = Color.SkyBlue;

    private int octaveOffset = 5;

    private int whiteKeyCount;

    static PianoControl()
    {
        keyTable.Add(Keys.A, 0);
        keyTable.Add(Keys.W, 1);
        keyTable.Add(Keys.S, 2);
        keyTable.Add(Keys.E, 3);
        keyTable.Add(Keys.D, 4);
        keyTable.Add(Keys.F, 5);
        keyTable.Add(Keys.T, 6);
        keyTable.Add(Keys.G, 7);
        keyTable.Add(Keys.Y, 8);
        keyTable.Add(Keys.Z, 8);
        keyTable.Add(Keys.H, 9);
        keyTable.Add(Keys.U, 10);
        keyTable.Add(Keys.J, 11);
        keyTable.Add(Keys.K, 12);
        keyTable.Add(Keys.O, 13);
        keyTable.Add(Keys.L, 14);
        keyTable.Add(Keys.P, 15);
    }

    public PianoControl()
    {
        CreatePianoKeys();
        InitializePianoKeys();

        context = SynchronizationContext.Current;

        noteOnCallback = delegate(ChannelMessage message)
        {
            if (message.Data2 > 0)
                keys[message.Data1 - lowNoteID].PressPianoKey();
            else
                keys[message.Data1 - lowNoteID].ReleasePianoKey();
        };

        noteOffCallback = delegate(ChannelMessage message) { keys[message.Data1 - lowNoteID].ReleasePianoKey(); };
    }

    public int LowNoteID
    {
        get { return lowNoteID; }
        set
        {
            #region Require

            if (value < 0 || value > ShortMessage.DataMaxValue)
                throw new ArgumentOutOfRangeException("LowNoteID", value,
                    "Low note ID out of range.");

            #endregion

            #region Guard

            if (value == lowNoteID) return;

            #endregion

            lowNoteID = value;

            if (lowNoteID > highNoteID) highNoteID = lowNoteID;

            CreatePianoKeys();
            InitializePianoKeys();
        }
    }

    public int HighNoteID
    {
        get { return highNoteID; }
        set
        {
            #region Require

            if (value < 0 || value > ShortMessage.DataMaxValue)
                throw new ArgumentOutOfRangeException("HighNoteID", value,
                    "High note ID out of range.");

            #endregion

            #region Guard

            if (value == highNoteID) return;

            #endregion

            highNoteID = value;

            if (highNoteID < lowNoteID) lowNoteID = highNoteID;

            CreatePianoKeys();
            InitializePianoKeys();
        }
    }

    public Color NoteOnColor
    {
        get { return noteOnColor; }
        set
        {
            #region Guard

            if (value == noteOnColor) return;

            #endregion

            noteOnColor = value;

            foreach (var key in keys) key.NoteOnColor = noteOnColor;
        }
    }

    public event EventHandler<PianoKeyEventArgs> PianoKeyDown;

    public event EventHandler<PianoKeyEventArgs> PianoKeyUp;

    private void CreatePianoKeys()
    {
        // If piano keys have already been created.
        if (keys != null)
            // Remove and dispose of current piano keys.
            foreach (var key in keys)
            {
                Controls.Remove(key);
                key.Dispose();
            }

        keys = new PianoKey[HighNoteID - LowNoteID];

        whiteKeyCount = 0;

        for (var i = 0; i < keys.Length; i++)
        {
            keys[i] = new PianoKey(this);
            keys[i].NoteID = i + LowNoteID;

            if (KeyTypeTable[keys[i].NoteID] == KeyType.White)
            {
                whiteKeyCount++;
            }
            else
            {
                keys[i].NoteOffColor = Color.Black;
                keys[i].BringToFront();
            }

            keys[i].NoteOnColor = NoteOnColor;

            Controls.Add(keys[i]);
        }
    }

    private void InitializePianoKeys()
    {
        #region Guard

        if (keys.Length == 0) return;

        #endregion

        var whiteKeyWidth = Width / whiteKeyCount;
        var blackKeyWidth = (int)(whiteKeyWidth * BlackKeyScale);
        var blackKeyHeight = (int)(Height * BlackKeyScale);
        var offset = whiteKeyWidth - blackKeyWidth / 2;
        var n = 0;
        var w = 0;

        var widthsum = 0; // Sum of white keys' width
        var LastWhiteWidth = 0; // Last white key width
        var remainder = Width % whiteKeyCount; // The remaining pixels
        var counter = 1;
        var step = remainder != 0
            ? whiteKeyCount / (double)remainder
            : 0; // The ternary operator prevents a division by zero

        while (n < keys.Length)
        {
            if (KeyTypeTable[keys[n].NoteID] == KeyType.White)
            {
                keys[n].Height = Height;
                keys[n].Width = whiteKeyWidth;

                if (remainder != 0 && counter <= whiteKeyCount && Convert.ToInt32(step * counter) == w)
                {
                    counter++;
                    keys[n].Width++;
                }

                // See the Location property of black keys to understand
                widthsum += LastWhiteWidth;
                LastWhiteWidth = keys[n].Width;
                keys[n].Location = new Point(widthsum, 0);

                w++;
                //n++; // Move?
            }
            else
            {
                keys[n].Height = blackKeyHeight;
                keys[n].Width = blackKeyWidth;
                keys[n].Location = new Point(widthsum + offset);
                //keys[n].Location = new Point(widthsum + offset - keys[n - 1].Width); // By this way, eliminates the LastWhiteWidth var
                keys[n].BringToFront();
                //n++; // Move?
            }

            n++; // Moved
        }
    }

    public void Send(ChannelMessage message)
    {
        switch (message.Command)
        {
            case ChannelCommand.NoteOn when message.Data1 >= LowNoteID && message.Data1 <= HighNoteID:
            {
                if (InvokeRequired)
                    BeginInvoke(noteOnCallback, message);
                else
                    noteOnCallback(message);

                break;
            }
            case ChannelCommand.NoteOff when message.Data1 >= LowNoteID && message.Data1 <= HighNoteID:
            {
                if (InvokeRequired)
                    BeginInvoke(noteOffCallback, message);
                else
                    noteOffCallback(message);

                break;
            }
        }
    }

    public void PressPianoKey(int noteID)
    {
        #region Require

        if (noteID < lowNoteID || noteID > highNoteID) throw new ArgumentOutOfRangeException();

        #endregion

        keys[noteID - lowNoteID].PressPianoKey();
    }

    public void ReleasePianoKey(int noteID)
    {
        #region Require

        if (noteID < lowNoteID || noteID > highNoteID) throw new ArgumentOutOfRangeException();

        #endregion

        keys[noteID - lowNoteID].ReleasePianoKey();
    }

    public void PressPianoKey(Keys k)
    {
        if (!Focused) return;

        if (keyTable.Contains(k))
        {
            var noteID = (int)keyTable[k] + 12 * octaveOffset;

            if (noteID < LowNoteID || noteID > HighNoteID) return;

            if (!keys[noteID - lowNoteID].IsPianoKeyPressed) keys[noteID - lowNoteID].PressPianoKey();
        }
        else
        {
            switch (k)
            {
                case Keys.D0:
                    octaveOffset = 0;
                    break;
                case Keys.D1:
                    octaveOffset = 1;
                    break;
                case Keys.D2:
                    octaveOffset = 2;
                    break;
                case Keys.D3:
                    octaveOffset = 3;
                    break;
                case Keys.D4:
                    octaveOffset = 4;
                    break;
                case Keys.D5:
                    octaveOffset = 5;
                    break;
                case Keys.D6:
                    octaveOffset = 6;
                    break;
                case Keys.D7:
                    octaveOffset = 7;
                    break;
                case Keys.D8:
                    octaveOffset = 8;
                    break;
                case Keys.D9:
                    octaveOffset = 9;
                    break;
            }
        }
    }

    public void ReleasePianoKey(Keys k)
    {
        #region Guard

        if (!keyTable.Contains(k)) return;

        #endregion

        var noteID = (int)keyTable[k] + 12 * octaveOffset;

        if (noteID >= LowNoteID && noteID <= HighNoteID) keys[noteID - lowNoteID].ReleasePianoKey();
    }

    protected override void OnResize(EventArgs e)
    {
        InitializePianoKeys();

        base.OnResize(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            foreach (var key in keys)
                key.Dispose();

        base.Dispose(disposing);
    }

    protected virtual void OnPianoKeyDown(PianoKeyEventArgs e)
    {
        var handler = PianoKeyDown;

        handler?.Invoke(this, e);
    }

    protected virtual void OnPianoKeyUp(PianoKeyEventArgs e)
    {
        var handler = PianoKeyUp;

        handler?.Invoke(this, e);
    }

    private enum KeyType
    {
        White,
        Black
    }

    private delegate void NoteMessageCallback(ChannelMessage message);
}