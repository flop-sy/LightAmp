#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song;
using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Managers;

public partial class MidiManager : IDisposable
{
    private static MidiManager instance;
    private static readonly object padlock = new();
    private bool isDisposing;

    private MidiManager()
    {
    }

    public static MidiManager Instance
    {
        get
        {
            lock (padlock)
            {
                return instance ??= new MidiManager();
            }
        }
    }

    public void Dispose()
    {
        if (isDisposing)
            return;

        isDisposing = true;

        sequencer.Stop();
        sequencer.PlayingCompleted -= HandlePlayingCompleted;
        sequencer.ChannelMessagePlayed -= HandleChannelMessagePlayed;
        sequencer.SysExMessagePlayed -= HandleSysExMessagePlayed;
        sequencer.Chased -= HandleChased;
        sequencer.Stopped -= HandleStopped;
        sequence.LoadProgressChanged -= HandleLoadProgressChanged;

        if (outDevice != null)
        {
            outDevice.Reset();
            outDevice.Dispose();
        }

        sequencer.Dispose();
        sequence.Dispose();
        sequencer = null;
        sequence = null;
        GC.SuppressFinalize(this);
        instance = null;
    }

    ~MidiManager()
    {
        Dispose();
    }

    #region ATRB

    // IO
    private OutputDevice outDevice;

    //private InputDevice inDevice;
    // Midi Msg Gen
    private readonly ChannelMessageBuilder cmBuilder = new();

    private SysCommonMessageBuilder scBuilder = new();

    // Midi sequencing
    private Sequencer sequencer;
    private Sequence sequence;

    #endregion

    #region CTOR

    public void Init()
    {
        InitSequencer();
        if (CheckMidiOutput())
            InitOutputDevice();
    }

    private static bool CheckMidiOutput()
    {
        if (OutputDeviceBase.DeviceCount != 0) return true;
        UiManager.ThrowError("No MIDI output devices available.");
        return false;
    }

    private void InitOutputDevice()
    {
        try
        {
            outDevice = new OutputDevice(0);
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }
    }

    private void InitSequencer()
    {
        // create
        sequencer = new Sequencer();
        sequence = new Sequence();
        // configure
        sequencer.Position = 0;
        sequencer.Sequence = sequence;
        sequencer.PlayingCompleted += HandlePlayingCompleted;
        sequencer.ChannelMessagePlayed += HandleChannelMessagePlayed;
        sequencer.SysExMessagePlayed += HandleSysExMessagePlayed;
        sequencer.Chased += HandleChased;
        sequencer.Stopped += HandleStopped;
        sequence.LoadProgressChanged += HandleLoadProgressChanged;
    }

    #endregion

    #region MIDI EVENTS

    private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
    {
        if (UiManager.Instance.mainWindow.Model.Closing) return;

        outDevice.Send(e.Message);
        //UiManager.Instance.mainWindow.Ctrl.Update(null, null);
    }

    private void HandleChased(object sender, ChasedEventArgs e)
    {
        foreach (ChannelMessage message in e.Messages) outDevice.Send(message);
        //UiManager.Instance.mainWindow.Ctrl.Update(null, null);
    }

    private static void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
    {
        //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
    }

    private void HandleStopped(object sender, StoppedEventArgs e)
    {
        foreach (ChannelMessage message in e.Messages) outDevice.Send(message);
    }

    private void HandlePlayingCompleted(object sender, EventArgs e)
    {
        Stop();
    }

    #endregion

    #region MTDS

    #region PLAY/PAUSE GESTION

    public bool IsPlaying { get; private set; }

    public int CurrentTime
    {
        get => sequencer.Position;
        set => sequencer.Position = value;
    }

    public int Tempo
    {
        get => sequencer.Tempo;
        set
        {
            if (value < 1) value = 1;
            //sequencer.InternalClock.Tempo = value;
        }
    }

    internal void Start()
    {
        try
        {
            IsPlaying = true;
            sequencer.Start();
            //Timer.Start();
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }
    }

    internal void Stop()
    {
        try
        {
            IsPlaying = false;
            sequencer.Stop();
            //Timer.Stop();
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }
    }

    internal void Continue()
    {
        try
        {
            IsPlaying = true;
            //sequencer.Pause();
            //Timer.Start();
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }
    }

    internal void Playback(bool v, int noteID)
    {
        if (IsPlaying) return;
        try
        {
            outDevice.Send(
                new ChannelMessage(v ? ChannelCommand.NoteOn : ChannelCommand.NoteOff, 0, noteID, 127)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine("MidiRollDebug : " + e);
        }
    }

    #endregion

    #region TRACK GESTION

    private static IEnumerable<T> ToIEnumerable<T>(IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext()) yield return enumerator.Current;
    }

    public IEnumerable<Track> Tracks => ToIEnumerable(sequence.GetEnumerator());

    internal void ChangeInstrument(Track track, int instrument)
    {
        // TODO

        cmBuilder.Command = ChannelCommand.ProgramChange;
        if (outDevice == null) return;
        cmBuilder.Data1 = instrument;
        //cmBuilder.MidiChannel = track.GetMidiEvent(0).MidiMessage.MessageType.;
        cmBuilder.Build();
        outDevice.Send(cmBuilder.Result);
    }

    /// <summary>
    ///     Create a prog change event
    /// </summary>
    /// <param name="selectedTrack"></param>
    /// <param name="instrument"></param>
    /// <param name="atStart"></param>
    internal void CreateProgChange(int selectedTrack, int instrument, bool atStart = false)
    {
        var t = ToIEnumerable(sequence.GetEnumerator());
        var track = t.ElementAt(selectedTrack);

        var x = track
            .Iterator().First(ev => ev.MidiMessage is ChannelMessage { Command: ChannelCommand.NoteOn });
        var cMsg = x.MidiMessage as ChannelMessage;

        var ct = 0;
        if (atStart)
        {
            foreach (var ev in track.Iterator())
                if (ev.MidiMessage is ChannelMessage { Command: ChannelCommand.ProgramChange } chanMsg)
                    if (ev.AbsoluteTicks < 50)
                    {
                        track.Remove(ev);
                        cMsg = chanMsg;
                        break;
                    }
        }
        else
        {
            foreach (var ev in track.Iterator())
                if (ev.MidiMessage is ChannelMessage { Command: ChannelCommand.NoteOn, Data2: > 0 } chanMsg)
                {
                    cMsg = chanMsg;
                    ct = ev.DeltaTicks - 10; //set some ticks before
                    break;
                }
        }

        cmBuilder.Command = ChannelCommand.ProgramChange;
        cmBuilder.Data1 = instrument;
        cmBuilder.Data2 = 64;
        cmBuilder.MidiChannel = cMsg.MidiChannel;
        cmBuilder.Build();
        track.Insert(ct, cmBuilder.Result);

        //Set the track name if we are at the first change
        if (!atStart)
            return;

        foreach (var ev in track.Iterator())
            if (ev.MidiMessage is MetaMessage { MetaType: MetaType.TrackName } metaMsg)
            {
                track.Remove(ev);
                var builder = new MetaTextBuilder(metaMsg)
                {
                    Text = Instrument.ParseByProgramChange(instrument).Name,
                    Type = MetaType.TrackName
                };
                builder.Build();
                track.Insert(ev.AbsoluteTicks, builder.Result);
                UiManager.Instance.mainWindow.Ctrl.InitTracks();
                break;
            }
    }

    #endregion

    #region PLOT GESTION

    internal static Tuple<MidiEvent, MidiEvent> CreateNote(int channel, int noteIndex, Track Track, double start,
        double end,
        int velocity)
    {
        /*cmBuilder.Command = ChannelCommand.NoteOn;
        cmBuilder.Data1 = noteIndex;
        cmBuilder.Data2 = velocity;
        cmBuilder.MidiChannel = channel;
        cmBuilder.Build();
        MidiEvent me1 = Track.Insert((int)(start* DAWhosReso), cmBuilder.Result);
        cmBuilder.Command = ChannelCommand.NoteOff;
        cmBuilder.Data1 = noteIndex;
        cmBuilder.Data2 = 0;
        cmBuilder.MidiChannel = channel;
        cmBuilder.Build();
        MidiEvent me2 = Track.Insert((int)(end* DAWhosReso), cmBuilder.Result);
        return new Tuple<MidiEvent, MidiEvent>(me1, me2);*/
        return null;
    }

    #endregion

    #region DATA

    internal int GetLength()
    {
        return sequence.GetLength();
    }

    internal void SaveFile(string fileName)
    {
        Stop();
        try
        {
            sequence.SaveAsync(fileName);
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }
        // on success
        //UiManager.Instance.mainWindow.DisableUserInterractions();
    }

    internal void OpenFile(string fileName)
    {
        Stop();
        try
        {
            // LOAD MIDI FILE
            var bmpSong = BmpSong.OpenFile(fileName).Result;
            OpenFile(bmpSong.GetExportMidi());
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }
    }

    internal void OpenFile(MemoryStream midiFile)
    {
        UiManager.Instance.mainWindow.DisableUserInterractions();

        try
        {
            // LOAD MIDI FILE
            sequence.Load(midiFile);
            midiFile.Close();
            midiFile.Dispose();
        }
        catch (Exception ex)
        {
            UiManager.ThrowError(ex.Message);
        }

        UiManager.Instance.mainWindow.EnableUserInterractions();

        UiManager.Instance.mainWindow.ProgressionBar.Value = 0;
        UiManager.Instance.mainWindow.MasterScroller.Value = 0;
        UiManager.Instance.mainWindow.MasterScroller.Maximum =
            sequence.GetLength() * UiManager.Instance.mainWindow.Model.XZoom;
        UiManager.Instance.mainWindow.Model.Tempo =
            sequencer.Tempo; // TODO tempo doesnt seem to be loaded from midi file that easy
        UiManager.Instance.mainWindow.Ctrl.InitTracks();
    }

    private static void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        UiManager.Instance.mainWindow.ProgressionBar.Value = e.ProgressPercentage;
    }

    /// <summary>
    ///     Converts the sanford midi to MemoryStream
    /// </summary>
    /// <returns></returns>
    private MemoryStream GetMidiStreamFromSanford()
    {
        var stream = new MemoryStream();
        sequence.Save(stream);
        stream.Rewind();
        return stream;
    }

    #endregion

    #endregion
}