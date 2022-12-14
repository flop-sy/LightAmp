#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

#region

using System;

#endregion

namespace Sanford.Multimedia.Midi.UI;

public sealed class PianoKeyEventArgs : EventArgs
{
    public PianoKeyEventArgs(int noteID)
    {
        NoteID = noteID;
    }

    public int NoteID { get; }
}