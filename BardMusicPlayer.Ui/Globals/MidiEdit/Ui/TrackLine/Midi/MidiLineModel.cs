#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using BardMusicPlayer.Ui.MidiEdit.Managers;
using BardMusicPlayer.Ui.MidiEdit.Utils;
using BardMusicPlayer.Ui.MidiEdit.Utils.TrackExtensions;
using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Ui.TrackLine;

public class MidiLineModel : HandleBinding
{
    public readonly Thickness SelectedBorderThickness = new(.5f);
    public readonly Thickness UnselectedBorderThickness = new(0);
    public bool isDragging = false;
    public Point mouseDragEndPoint;
    public Point mouseDragStartPoint;

    #region CTOR

    public MidiLineControl Ctrl { get; set; }
    public Track Track { get; }

    public MidiLineModel(Track track)
    {
        Track = track;
        var color = track.Color();
        tColor = new SolidColorBrush(color);
        LastNotesOn = new Dictionary<int, Tuple<int, MidiEvent>>();
    }

    #endregion

    #region ATRB

    private SolidColorBrush tColor;

    public SolidColorBrush TColor
    {
        get => tColor;
        set
        {
            tColor = value;
            RaisePropertyChanged("TColor");
        }
    }

    public int MidiInstrument { get; internal set; }

    public Dictionary<int, Tuple<int, MidiEvent>> LastNotesOn { get; set; }

    #region ZOOM

#pragma warning disable S3237

    public float CellWidth
    {
        get => UiManager.Instance.mainWindow.Model.XZoom;
        set
        {
            RaisePropertyChanged("CellWidth");
            RaisePropertyChanged("CellHeigth");
            Ctrl.DrawPianoRoll();
            Ctrl.DrawMidiEvents();
        }
    }

    public float CellHeigth
    {
        get => UiManager.Instance.mainWindow.Model.YZoom;
        set
        {
            RaisePropertyChanged("CellHeigth");
            RaisePropertyChanged("CellWidth");
            Ctrl.DrawPianoRoll();
            Ctrl.DrawMidiEvents();
        }
    }

#pragma warning restore S3237

    #endregion

    private double xOffset;

    public double XOffset
    {
        get => xOffset;
        set
        {
            xOffset = value;
            Ctrl.view.TrackBody.Margin = new Thickness(-XOffset, 0, 0, 0);
        }
    }

    public double DAWhosReso => 1;

    public static double PlotReso => 1 / UiManager.Instance.plotDivider;

    #endregion
}