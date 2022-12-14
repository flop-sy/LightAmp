#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

/// <summary>
///     Converts a MIDI note number to its corresponding frequency.
/// </summary>
public static class MidiNoteConverter
{
    /// <summary>
    ///     The minimum value a note ID can have.
    /// </summary>
    public const int NoteIDMinValue = 0;

    /// <summary>
    ///     The maximum value a note ID can have.
    /// </summary>
    public const int NoteIDMaxValue = 127;

    // Table for holding frequency values.
    private static readonly double[] NoteToFrequencyTable = new double[NoteIDMaxValue + 1];

    static MidiNoteConverter()
    {
        // The number of notes per octave.
        const int notesPerOctave = 12;

        // Reference frequency used for calculations.
        const double referenceFrequency = 440;

        // The note ID of the reference frequency.
        const int referenceNoteID = 69;

        // Fill table with the frequencies of all MIDI notes.
        for (var i = 0; i < NoteToFrequencyTable.Length; i++)
        {
            var exponent = (double)(i - referenceNoteID) / notesPerOctave;

            NoteToFrequencyTable[i] = referenceFrequency * Math.Pow(2.0, exponent);
        }
    }

    // Prevents instances of this class from being created - no need for
    // an instance to be created since this class only has static methods.

    /// <summary>
    ///     Converts the specified note to a frequency.
    /// </summary>
    /// <param name="noteID">
    ///     The ID of the note to convert.
    /// </param>
    /// <returns>
    ///     The frequency of the specified note.
    /// </returns>
    public static double NoteToFrequency(int noteID)
    {
        #region Require

        if (noteID < NoteIDMinValue || noteID > NoteIDMaxValue)
            throw new ArgumentOutOfRangeException("Note ID out of range.");

        #endregion

        return NoteToFrequencyTable[noteID];
    }

    /// <summary>
    ///     Converts the specified frequency to a note.
    /// </summary>
    /// <param name="frequency">
    ///     The frequency to convert.
    /// </param>
    /// <returns>
    ///     The ID of the note closest to the specified frequency.
    /// </returns>
    public static int FrequencyToNote(double frequency)
    {
        var noteID = 0;
        var found = false;

        // Search for the note with a frequency near the specified frequency.
        for (var i = 0; i < NoteIDMaxValue && !found; i++)
        {
            noteID = i;

            // If the specified frequency is less than the frequency of
            // the next note.
            if (frequency < NoteToFrequency(noteID + 1))
                // Indicate that the note ID for the specified frequency
                // has been found.
                found = true;
        }

        // If the note is not the first or last note, narrow the results.
        if (noteID <= 0 || noteID >= NoteIDMaxValue) return noteID;
        // Get the frequency of the previous note.
        var previousFrequncy = NoteToFrequency(noteID - 1);
        // Get the frequency of the next note.
        var nextFrequency = NoteToFrequency(noteID + 1);

        // If the next note is closer in frequency than the previous note.
        if (nextFrequency - frequency < frequency - previousFrequncy)
            // Move to the next note.
            noteID++;

        return noteID;
    }
}