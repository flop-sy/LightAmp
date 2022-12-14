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