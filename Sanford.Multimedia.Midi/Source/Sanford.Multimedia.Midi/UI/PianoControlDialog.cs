#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

#region

using System;
using System.Windows.Forms;

#endregion

namespace Sanford.Multimedia.Midi.UI;

public partial class PianoControlDialog : Form
{
    private int highNoteID;
    private int lowNoteID;

    public PianoControlDialog()
    {
        InitializeComponent();

        UpdateProperties();
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

            lowNoteID = value;

            lowNoteIDNumericUpDown.Value = value;

            if (lowNoteID <= highNoteID) return;

            highNoteID = lowNoteID;
            highNoteIDNumericUpDown.Value = highNoteID;
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

            highNoteID = value;

            highNoteIDNumericUpDown.Value = value;

            if (highNoteID >= lowNoteID) return;

            lowNoteID = highNoteID;
            lowNoteIDNumericUpDown.Value = highNoteID;
        }
    }

    private void UpdateProperties()
    {
        lowNoteID = (int)lowNoteIDNumericUpDown.Value;
        highNoteID = (int)highNoteIDNumericUpDown.Value;
    }

    private void lowNoteIDNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        if (lowNoteIDNumericUpDown.Value > highNoteIDNumericUpDown.Value)
            highNoteIDNumericUpDown.Value = lowNoteIDNumericUpDown.Value;
    }

    private void highNoteIDNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        if (highNoteIDNumericUpDown.Value < lowNoteIDNumericUpDown.Value)
            lowNoteIDNumericUpDown.Value = highNoteIDNumericUpDown.Value;
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        UpdateProperties();

        Close();
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        Close();
    }
}