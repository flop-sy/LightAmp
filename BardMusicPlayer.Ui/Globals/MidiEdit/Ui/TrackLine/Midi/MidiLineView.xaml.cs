#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Ui.MidiEdit.Managers;
using BardMusicPlayer.Ui.MidiEdit.Utils.TrackExtensions;
using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Ui.TrackLine;

public partial class MidiLineView
{
    #region CTOR

    public MidiLineControl Ctrl { get; set; }
    public MidiLineModel Model { get; set; }

    public MidiLineView(Track track)
    {
        Model = new MidiLineModel(track);
        DataContext = Model;
        InitializeComponent();
        Ctrl = new MidiLineControl(Model, this);
        Model.Ctrl = Ctrl;
        Loaded += MyWindow_Loaded;
        TrackBody.MouseWheel += MouseWheel;
        //TrackHeader.MouseWheel += MouseWheeled;
    }

    private void MyWindow_Loaded(object sender, RoutedEventArgs e)
    {
        BodyScroll.ScrollToVerticalOffset(BodyScroll.ScrollableHeight / 2);
    }

    #endregion

    #region SHOW BORDERS ON FOCUS

    private void Grid_GotFocus(object sender, RoutedEventArgs e)
    {
        Border.BorderThickness = Model.SelectedBorderThickness;
        TrackHeader.Background = new SolidColorBrush(Colors.LightGray);
        Ctrl.TrackGotFocus(sender, e);
    }

    private void Grid_LostFocus(object sender, RoutedEventArgs e)
    {
        Border.BorderThickness = Model.UnselectedBorderThickness;
        TrackHeader.Background = new SolidColorBrush(Colors.Gray);
    }

    private void MergeUp_Click(object sender, RoutedEventArgs e)
    {
        Border.BorderThickness = Model.SelectedBorderThickness;
        Ctrl.TrackGotFocus(sender, e);
        Ctrl.MergeUp(sender, e);
    }

    private void MergeDown_Click(object sender, RoutedEventArgs e)
    {
        Border.BorderThickness = Model.SelectedBorderThickness;
        Ctrl.TrackGotFocus(sender, e);
        Ctrl.MergeDown(sender, e);
    }

    #endregion

    #region MOUSE GESTION

    private void TrackBody_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount <= 1) return;
        Model.mouseDragStartPoint = e.GetPosition((Canvas)sender);
        var point = Model.mouseDragStartPoint.X / Model.CellWidth;
        var noteIndex = 127 - (int)(Model.mouseDragStartPoint.Y / Model.CellHeigth);
        Ctrl.InsertNote(PreviousFirstPosition(point), NextFirstPosition(point), noteIndex);
    }

    private static double NextFirstPosition(double point)
    {
        return MidiLineModel.PlotReso * (1 + (int)(point / MidiLineModel.PlotReso));
    }

    private static double PreviousFirstPosition(double point)
    {
        return MidiLineModel.PlotReso * (int)(point / MidiLineModel.PlotReso);
    }

    private void TrackBody_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    private new static void MouseWheel(object sender, MouseWheelEventArgs e)
    {
        UiManager.Instance.mainWindow.HandleWheel(sender, e);
    }

    #endregion

    #region HEADER

    // TODO color picker
    private void TrackColor_Click(object sender, RoutedEventArgs e)
    {
        var rnd = new Random();
        var color = Color.FromRgb(
            (byte)rnd.Next(0, 255),
            (byte)rnd.Next(0, 255),
            (byte)rnd.Next(0, 255)
        );
        Model.Track.SetColor(color);
        Model.TColor = new SolidColorBrush(color);
    }

    private void InstrumentBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        MidiManager.Instance.ChangeInstrument(Model.Track, ComboInstruments.SelectedIndex);

        if (!ComboInstruments.IsDropDownOpen) return;
        ComboInstruments.IsDropDownOpen = false;
        MidiManager.Instance.SetInstrument(Model.Track.Id(), ComboInstruments.SelectedIndex);
        MidiManager.Instance.SetTrackName(Model.Track.Id(),
            Instrument.ParseByProgramChange(ComboInstruments.SelectedIndex).Name);
        UiManager.Instance.mainWindow.Ctrl.InitTracks();
    }

    #endregion
}