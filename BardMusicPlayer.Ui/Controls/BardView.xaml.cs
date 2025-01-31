﻿#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Functions;
using Microsoft.Win32;
using Newtonsoft.Json;

#endregion

namespace BardMusicPlayer.Ui.Controls;

/// <summary>
///     Interaktionslogik für BardView.xaml
/// </summary>
public sealed partial class BardView
{
    public BardView()
    {
        InitializeComponent();

        DataContext = this;
        Bards = new ObservableCollection<Performer>();

        BmpMaestro.Instance.OnPerformerChanged += OnPerfomerChanged;
        BmpMaestro.Instance.OnTrackNumberChanged += OnTrackNumberChanged;
        BmpMaestro.Instance.OnOctaveShiftChanged += OnOctaveShiftChanged;
        BmpMaestro.Instance.OnSongLoaded += OnSongLoaded;
        BmpMaestro.Instance.OnPerformerUpdate += OnPerformerUpdate;
        BmpSeer.Instance.PlayerNameChanged += OnPlayerNameChanged;
        BmpSeer.Instance.InstrumentHeldChanged += OnInstrumentHeldChanged;
        BmpSeer.Instance.HomeWorldChanged += OnHomeWorldChanged;
        Globals.Globals.OnConfigReload += Globals_OnConfigReload;
        Globals_OnConfigReload(null, null);
    }

    public ObservableCollection<Performer> Bards { get; private set; }

    public Performer SelectedBard { get; set; }

    private void Globals_OnConfigReload(object sender, EventArgs e)
    {
        Autoequip_CheckBox.IsChecked = BmpPigeonhole.Instance.AutoEquipBards;
    }

    private void OnPerfomerChanged(object sender, bool e)
    {
        Bards = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
        Dispatcher.BeginInvoke(new Action(() => { BardsList.ItemsSource = Bards; }));
    }

    private void OnTrackNumberChanged(object sender, TrackNumberChangedEvent e)
    {
        UpdateList();
    }

    private void OnOctaveShiftChanged(object sender, OctaveShiftChangedEvent e)
    {
        UpdateList();
    }

    private void OnSongLoaded(object sender, SongLoadedEvent e)
    {
        UpdateList();
    }

    private void OnPerformerUpdate(object sender, PerformerUpdate e)
    {
        UpdateList();
    }

    private void OnPlayerNameChanged(PlayerNameChanged e)
    {
        UpdateList();
    }

    private void OnHomeWorldChanged(HomeWorldChanged e)
    {
        UpdateList();
    }

    private void OnInstrumentHeldChanged(InstrumentHeldChanged e)
    {
        UpdateList();
    }

    private void UpdateList()
    {
        Bards = new ObservableCollection<Performer>(BmpMaestro.Instance.GetAllPerformers());
        Dispatcher.BeginInvoke(new Action(() => { BardsList.ItemsSource = Bards; }));
    }

    private void RdyCheck_Click(object sender, RoutedEventArgs e)
    {
        BmpMaestro.Instance.StartEnsCheck();
    }

    private void OpenInstrumentButton_Click(object sender, RoutedEventArgs e)
    {
        BmpMaestro.Instance.EquipInstruments();
    }

    private void CloseInstrumentButton_Click(object sender, RoutedEventArgs e)
    {
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
        {
            PlaybackFunctions.PauseSong();
            Classic_MainView.CurrentInstance.Play_Button_State();
        }

        BmpMaestro.Instance.StopLocalPerformer();
        BmpMaestro.Instance.UnEquipInstruments();
    }

    private void BardsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine(BardsList.SelectedItem);
    }

    private void BardsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        SelectedBard = BardsList.SelectedItem as Performer;
    }

    /* Track UP/Down */
    private void TrackNumericUpDown_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TrackNumericUpDown ctl) ctl.OnValueChanged += OnValueChanged;
    }

    private static void OnValueChanged(object sender, int s)
    {
        var game = (sender as TrackNumericUpDown)?.DataContext as Performer;
        BmpMaestro.Instance.SetTracknumber(game, s);

        if (sender is TrackNumericUpDown ctl) ctl.OnValueChanged -= OnValueChanged;
    }

    /* Octave UP/Down */
    private void OctaveControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is OctaveNumericUpDown ctl) ctl.OnValueChanged += OnOctaveValueChanged;
    }

    private static void OnOctaveValueChanged(object sender, int s)
    {
        var performer = (sender as OctaveNumericUpDown)?.DataContext as Performer;
        BmpMaestro.Instance.SetOctaveshift(performer, s);

        if (sender is OctaveNumericUpDown ctl) ctl.OnValueChanged -= OnOctaveValueChanged;
    }

    private void HostChecker_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox ctl && (!ctl.IsChecked ?? false))
            return;

        var game = (sender as CheckBox)?.DataContext as Performer;
        BmpMaestro.Instance.SetHostBard(game);
    }

    private void PerfomerEnabledChecker_Checked(object sender, RoutedEventArgs e)
    {
        var game = ((CheckBox)sender).DataContext as Performer;
        if (sender is not CheckBox ctl) return;

        if (game != null) game.PerformerEnabled = ctl.IsChecked ?? false;
    }

    private void Bard_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount != 2) return;
        if (SelectedBard == null)
            return;

        var bardExtSettings = new BardExtSettingsWindow(SelectedBard);
        bardExtSettings.Activate();
        bardExtSettings.Visibility = Visibility.Visible;
    }

    private void Autoequip_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        BmpPigeonhole.Instance.AutoEquipBards = Autoequip_CheckBox.IsChecked ?? false;
        Globals.Globals.ReloadConfig();
    }

    /// <summary>
    ///     load the performer config file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Load_Performer_Settings(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Performer Config | *.cfg",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var memoryStream = new MemoryStream();
        var fileStream = File.Open(openFileDialog.FileName, FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        var data = memoryStream.ToArray();
        var pdatalist =
            JsonConvert.DeserializeObject<List<PerformerSettingData>>(new UTF8Encoding(true).GetString(data));

        foreach (var pconfig in pdatalist)
        {
            var p = Bards.Where(perf => perf.game.PlayerName.Equals(pconfig.Name));
            var performers = p as Performer[] ?? p.ToArray();
            if (!performers.Any())
                continue;

            performers.First().TrackNumber = pconfig.Track;
            if (pconfig.AffinityMask != 0)
                performers.First().game.SetAffinity(pconfig.AffinityMask);
        }

        //Set Thymms box, cuz if u use this function, you know what you are doing
        if (BmpPigeonhole.Instance.EnsembleKeepTrackSetting) return;

        BmpPigeonhole.Instance.EnsembleKeepTrackSetting = true;
        Globals.Globals.ReloadConfig();
    }

    /// <summary>
    ///     save the performer config file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Save_Performer_Settings(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = "Performer Config | *.cfg"
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var pdatalist = Bards.Select(static performer => new PerformerSettingData
        {
            Name = performer.game.PlayerName, Track = performer.TrackNumber,
            AffinityMask = (long)performer.game.GetAffinity()
        }).ToList();

        var t = JsonConvert.SerializeObject(pdatalist);
        var content = new UTF8Encoding(true).GetBytes(t);

        var fileStream = File.Create(openFileDialog.FileName);
        fileStream.Write(content, 0, content.Length);
        fileStream.Close();
    }

    private void GfxLow_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        foreach (var p in Bards.Where(p => p.game.GfxSettingsLow != GfxLow_CheckBox.IsChecked))
        {
            p.game.GfxSettingsLow = GfxLow_CheckBox.IsChecked ?? false;
            p.game.GfxSetLow(GfxLow_CheckBox.IsChecked ?? false);
        }
    }

    /// <summary>
    ///     Button context menu routine
    /// </summary>
    private void MenuButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
    {
        if (sender is not Button rectangle) return;

        var contextMenu = rectangle.ContextMenu;
        if (contextMenu == null) return;

        contextMenu.PlacementTarget = rectangle;
        contextMenu.Placement = PlacementMode.Bottom;
        contextMenu.IsOpen = true;
    }
}

/// <summary>
///     Helperclass
/// </summary>
public sealed class PerformerSettingData
{
    public string Name { get; set; } = "";
    public int Track { get; set; }
    public long AffinityMask { get; set; }
}