#region

using System;
using System.Windows;
using BardMusicPlayer.Ui.MidiEdit.Ui;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Managers;

public class UiManager : IDisposable
{
    private static UiManager instance;
    private static readonly object padlock = new();

    private UiManager()
    {
    }

    public static UiManager Instance
    {
        get
        {
            lock (padlock)
            {
                return instance ??= new UiManager();
            }
        }
    }

    public MidiEditWindow mainWindow { get; set; }

    // track config
    public int TrackHeightDefault { get; set; } = 100;
    public int TrackHeightMin { get; set; } = 100;
    public int TrackHeightMax { get; set; } = 500;

    // input config
    public double noteLengthDivider { get; set; } = 4;
    public double plotDivider { get; set; } = 4;
    public int plotVelocity { get; set; } = 100;

    public void Dispose()
    {
        mainWindow = null;
    }

    ~UiManager()
    {
        Dispose();
    }

    public static void ThrowError(string message)
    {
        MessageBox.Show(
            message,
            "Error",
            MessageBoxButton.OK
        );
    }
}