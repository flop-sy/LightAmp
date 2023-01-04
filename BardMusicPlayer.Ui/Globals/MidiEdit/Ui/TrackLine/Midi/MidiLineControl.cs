#region

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Ui.MidiEdit.Managers;
using BardMusicPlayer.Ui.MidiEdit.Utils.TrackExtensions;
using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Ui.TrackLine;

public class MidiLineControl
{
    #region DRAW GRID

    public void DrawPianoRoll()
    {
        view.TrackNotes.Children.Clear();
        Brush currentColor = Brushes.White;
        for (var i = 0; i < 128; i++)
        {
            // identify note
            var noteWithoutOctave = i % 12;

            currentColor = i switch
            {
                48 => Brushes.Red,
                60 => Brushes.Yellow,
                69 => Brushes.Cyan,
                // choose note color
                _ => noteWithoutOctave switch
                {
                    0 => Brushes.White,
                    1 => Brushes.Black,
                    2 => Brushes.White,
                    3 => Brushes.Black,
                    4 => Brushes.White,
                    5 => Brushes.White,
                    6 => Brushes.Black,
                    7 => Brushes.White,
                    8 => Brushes.Black,
                    9 => Brushes.White,
                    10 => Brushes.Black,
                    11 => Brushes.White,
                    _ => currentColor
                }
            };
            // make rectangle
            var rec = new Rectangle
            {
                Width = 15,
                Height = model.CellHeigth,
                Fill = currentColor,
                Stroke = Brushes.Gray,
                StrokeThickness = .5f
            };
            // place rectangle
            Canvas.SetLeft(rec, 0);
            Canvas.SetTop(rec, (127 - i) * model.CellHeigth);
            // piano toucn on rectangle
            var j = i;
            rec.MouseLeftButtonDown += (s, e) => MidiManager.Instance.Playback(true, j);
            rec.MouseLeftButtonUp += (s, e) => MidiManager.Instance.Playback(false, j);
            rec.MouseLeave += (s, e) => MidiManager.Instance.Playback(false, j);
            // add it to the control
            view.TrackNotes.Children.Add(rec);
            view.TrackNotes.Height = 127 * model.CellHeigth;
            view.TrackBody.Height = 127 * model.CellHeigth;
        }
    }

    #endregion

    #region CTOR

    public MidiLineView view { get; set; }
    private MidiLineModel model { get; }

    public MidiLineControl(MidiLineModel model, MidiLineView view)
    {
        this.model = model;
        this.view = view;
        Init();
    }

    private void Init()
    {
        // track header
        FillInstrumentBox();

        view.ComboInstruments.SelectedIndex = MidiManager.Instance.GetInstrument(model.Track.Id());
        view.ChannelId.Content = MidiManager.Instance.GetChannelNumber(model.Track.Id()) + 1;
        //Check if the instrument is "None"
        if (view.ComboInstruments.Items.GetItemAt(view.ComboInstruments.SelectedIndex) is ComboBoxItem it)
            if (it.Content.ToString() == "None")
            {
                var trackName = MidiManager.Instance.GetTrackName(model.Track.Id());
                var rex = new Regex(@"^([A-Za-z _]+)([-+]\d)?");
                if (rex.Match(trackName) is { } match)
                    if (!string.IsNullOrEmpty(match.Groups[1].Value))
                    {
                        var num = Instrument.Parse(match.Groups[1].Value).MidiProgramChangeCode;
                        view.ComboInstruments.SelectedIndex = num;
                        MidiManager.Instance.SetInstrument(model.Track.Id(), num);
                    }
            }

        if (!Instrument.ParseByProgramChange(view.ComboInstruments.SelectedIndex).Equals(Instrument.None))
            MidiManager.Instance.SetTrackName(model.Track.Id(),
                Instrument.ParseByProgramChange(view.ComboInstruments.SelectedIndex).Name);

        //Check if we got a drum track
        if (view.ChannelId.Content.ToString() == "10")
            view.TrackName.Content = MidiManager.Instance.GetTrackName(model.Track.Id()) + " or Drums";
        else
            view.TrackName.Content = MidiManager.Instance.GetTrackName(model.Track.Id());

        // track body
        DrawPianoRoll();
        DrawMidiEvents();
    }

    private void FillInstrumentBox()
    {
        var instlist = new Dictionary<int, string>();
        for (var i = 0; i != 128; i++)
            instlist.Add(i, "None");

        foreach (var instrument in Instrument.All) instlist[instrument.MidiProgramChangeCode] = instrument.Name;

        foreach (var instrument in instlist)
            view.ComboInstruments.Items.Add(
                new ComboBoxItem
                {
                    Content = instrument.Value
                }
            );
        view.ComboInstruments.SelectedIndex = 0;
    }

    public event EventHandler<int> TrackFocused;
    public event EventHandler<Track> TrackMergeUp;
    public event EventHandler<Track> TrackMergeDown;

    public static readonly DependencyProperty AttachedNoteOnProperty =
        DependencyProperty.RegisterAttached(
            "AttachedNoteOn",
            typeof(MidiEvent),
            typeof(MidiLineControl)
        );

    public static readonly DependencyProperty AttachedNoteOffProperty =
        DependencyProperty.RegisterAttached(
            "AttachedNoteOff",
            typeof(MidiEvent),
            typeof(MidiLineControl)
        );

    #endregion

    #region INTERACTIONS

    internal void TrackGotFocus(object sender, RoutedEventArgs e)
    {
        TrackFocused?.Invoke(sender, model.Track.Id());
    }

    internal void MergeUp(object sender, RoutedEventArgs e)
    {
        TrackFocused?.Invoke(sender, model.Track.Id());
        TrackMergeUp?.Invoke(sender, model.Track);
    }

    internal void MergeDown(object sender, RoutedEventArgs e)
    {
        TrackFocused?.Invoke(sender, model.Track.Id());
        TrackMergeDown?.Invoke(sender, model.Track);
    }

    internal void InsertNote(double start, double end, int noteIndex)
    {
        /*
        if (MidiManager.Instance.IsPlaying) return;
        // Generate Midi Note
        const int channel = 0;
        var velocity = UiManager.Instance.plotVelocity;
        var msgs = MidiManager.CreateNote(
            channel,
            noteIndex,
            model.Track,
            start,
            end,
            velocity);
        // Draw it on MidiRoll
        DrawNote(start, end, noteIndex, msgs.Item1, msgs.Item2);
*/
    }

    #endregion

    #region DRAW MIDI EVENT

    public void DrawMidiEvents()
    {
        view.TrackBody.Children.Clear();
        foreach (var midiEvent in model.Track.Iterator())
            if (midiEvent.MidiMessage.MessageType == MessageType.Channel)
                DrawChannelMsg(midiEvent);
    }

    private void DrawChannelMsg(MidiEvent midiEvent)
    {
        var status = midiEvent.MidiMessage.Status;
        var position = midiEvent.AbsoluteTicks;
        switch (status)
        {
            // NOTE OFF
            case >= (int)ChannelCommand.NoteOff and <= (int)ChannelCommand.NoteOff + ChannelMessage.MidiChannelMaxValue:
            {
                int noteIndex = midiEvent.MidiMessage.GetBytes()[1];
                if (model.LastNotesOn.TryGetValue(noteIndex, out var onPosition))
                {
                    DrawNote(
                        onPosition.Item1 / model.DAWhosReso,
                        position / model.DAWhosReso,
                        noteIndex,
                        onPosition.Item2,
                        midiEvent
                    );
                    model.LastNotesOn.Remove(noteIndex);
                }

                break;
            }
            // NOTE ON
            case >= (int)ChannelCommand.NoteOn and <= (int)ChannelCommand.NoteOn + ChannelMessage.MidiChannelMaxValue:
            {
                int noteIndex = midiEvent.MidiMessage.GetBytes()[1];
                int velocity = midiEvent.MidiMessage.GetBytes()[2];
                if (velocity > 0)
                {
                    model.LastNotesOn[noteIndex] = new Tuple<int, MidiEvent>(position, midiEvent);
                }
                else
                {
                    if (model.LastNotesOn.TryGetValue(noteIndex, out var onPosition))
                    {
                        DrawNote(onPosition.Item1, position, noteIndex, onPosition.Item2, midiEvent);
                        model.LastNotesOn.Remove(noteIndex);
                    }
                }

                break;
            }
            // ProgramChange
            case >= (int)ChannelCommand.ProgramChange
                and <= (int)ChannelCommand.ProgramChange + ChannelMessage.MidiChannelMaxValue:
                model.MidiInstrument = midiEvent.MidiMessage.GetBytes()[1];
                break;
        }
    }

    private void DrawNote(double start, double end, int noteIndex, MidiEvent messageOn, MidiEvent messageOff)
    {
        var rec = new Rectangle();
        try
        {
            rec.Width = (end - start) * model.CellWidth;
        }
        catch
        {
            rec.Width = 1;
        }

        rec.Height = model.CellHeigth;
        rec.Fill = Brushes.Red; //DarkSeaGreen;
        rec.Stroke = Brushes.DarkRed; // DarkGreen;
        rec.StrokeThickness = .5f;
        Canvas.SetLeft(rec, start * model.CellWidth);
        Canvas.SetTop(rec, (127 - noteIndex) * model.CellHeigth);
        rec.MouseLeftButtonDown += NoteLeftDown;
        rec.MouseRightButtonDown += NoteRightDown;
        rec.SetValue(AttachedNoteOnProperty, messageOn);
        rec.SetValue(AttachedNoteOffProperty, messageOff);
        view.TrackBody.Children.Add(rec);
    }

    private void NoteLeftDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (e.ClickCount <= 1) return;
        if (MidiManager.Instance.IsPlaying) return;
        var rec = (Rectangle)sender;
        var noteOn = (MidiEvent)rec.GetValue(AttachedNoteOnProperty);
        var noteOff = (MidiEvent)rec.GetValue(AttachedNoteOffProperty);
        view.TrackBody.Children.Remove(rec);
        model.Track.Remove(noteOn);
        model.Track.Remove(noteOff);
    }

    private static void NoteRightDown(object sender, MouseButtonEventArgs e)
    {
    }

    #endregion
}