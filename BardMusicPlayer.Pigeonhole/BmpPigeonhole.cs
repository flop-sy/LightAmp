#region

using System.Drawing;
using BardMusicPlayer.Pigeonhole.JsonSettings.Autosave;
using BardMusicPlayer.Quotidian;

#endregion

namespace BardMusicPlayer.Pigeonhole;

public class BmpPigeonhole : JsonSettings.JsonSettings
{
    private static BmpPigeonhole _instance;

    /// <summary>
    /// </summary>
    public static bool Initialized => _instance != null;

    /// <summary>
    ///     Gets this pigeonhole instance
    /// </summary>
    public static BmpPigeonhole Instance =>
        _instance ?? throw new BmpException("This pigeonhole must be initialized first.");

    /// <summary>
    ///     Sets PlayAllTracks
    /// </summary>
    public virtual bool PlayAllTracks { get; set; }

    /// <summary>
    ///     Sets PlaylistDelay
    /// </summary>
    public virtual float PlaylistDelay { get; set; } = 1;

    /// <summary>
    ///     Sets PlayAllTracks
    /// </summary>
    public virtual bool PlaylistAutoPlay { get; set; } = true;

    /// <summary>
    ///     last loaded song
    /// </summary>
    public virtual string LastLoadedCatalog { get; set; } = "";

    /// <summary>
    ///     last loaded song
    /// </summary>
    public virtual string SongDirectory { get; set; } = "songs/";

    /// <summary>
    ///     hold long notes
    /// </summary>
    public virtual bool HoldNotes { get; set; } = true;

    /// <summary>
    ///     save the chatlog
    /// </summary>
    public virtual bool SaveChatLog { get; set; } = false;

    /// <summary>
    ///     Sets the autostart method
    /// </summary>
    public virtual int AutostartMethod { get; set; } = 2;

    /// <summary>
    ///     Sets UnequipPause
    /// </summary>
    public virtual bool UnequipPause { get; set; } = true;

    /// <summary>
    ///     last selected midi input device
    /// </summary>
    public virtual int MidiInputDev { get; set; } = -1;

    /// <summary>
    ///     brings the bmp to front
    /// </summary>
    public virtual bool LiveMidiPlayDelay { get; set; }

    /// <summary>
    ///     force the playback
    /// </summary>
    public virtual bool ForcePlayback { get; set; }

    /// <summary>
    ///     brings the game to front
    /// </summary>
    public virtual bool BringGametoFront { get; set; } = true;

    /// <summary>
    ///     brings the bmp to front
    /// </summary>
    public virtual bool BringBMPtoFront { get; set; } = true;

    /// <summary>
    ///     unkown but used
    /// </summary>
    public virtual bool SigIgnore { get; set; } = false;

    /// <summary>
    ///     LastCharId
    /// </summary>
    public virtual string LastCharId { get; set; } = "";

    /// <summary>
    ///     BMP window location
    /// </summary>
    public virtual Point BmpLocation { get; set; } = Point.Empty;

    public virtual Size BmpSize { get; set; } = Size.Empty;

    /// <summary>
    ///     The Ui version which should be used
    /// </summary>
    public virtual bool ClassicUi { get; set; } = true;

    /// <summary>
    ///     Sets/Gets last used skin
    /// </summary>
    public virtual string LastSkin { get; set; } = "";

    /// <summary>
    ///     Sets/Gets skin directory
    /// </summary>
    public virtual string SkinDirectory { get; set; } = "";

    /// <summary>
    ///     open local orchestra after hooking new proc
    /// </summary>
    public virtual bool LocalOrchestra { get; set; }

    /// <summary>
    ///     Enable the 16 voice limit in Synthesizer
    /// </summary>
    public virtual bool EnableSynthVoiceLimiter => true;

    /// <summary>
    ///     milliseconds till ready check confirmation.
    /// </summary>
    public virtual int EnsembleReadyDelay => 500;

    /// <summary>
    ///     autoequip bards after song loaded
    /// </summary>
    public virtual bool AutoEquipBards { get; set; }

    /// <summary>
    ///     keep the ensmble track settings
    /// </summary>
    public virtual bool EnsembleKeepTrackSetting { get; set; }

    /// <summary>
    ///     ignores the progchange
    /// </summary>
    public virtual bool IgnoreProgChange { get; set; }

    /// <summary>
    ///     milliseconds between game process scans / seer scanner startups.
    /// </summary>
    public virtual int SeerGameScanCooldown => 20;

    /// <summary>
    ///     Contains the last path of an opened midi file
    /// </summary>
    public virtual string LastOpenedMidiPath { get; set; } = "";

    /// <summary>
    ///     Contains the delay used for note pressing. This should be no less then 1 and no greater then 25.
    /// </summary>
    public virtual int NoteKeyDelay => 1;

    /// <summary>
    ///     Contains the delay used for tone pressing. This should be no less then 1 and no greater then 25.
    /// </summary>
    public virtual int ToneKeyDelay { get; set; } = 3;

    /// <summary>
    ///     Compatmode for MidiBard
    /// </summary>
    public virtual bool MidiBardCompatMode { get; set; }

    /// <summary>
    ///     Use the Hypnotoad for instruemtn eq
    /// </summary>
    public virtual bool UsePluginForInstrumentOpen { get; set; }

    /// <summary>
    ///     Defaults to log level Info
    /// </summary>
    public virtual BmpLog.Verbosity DefaultLogLevel { get; set; } = BmpLog.Verbosity.Info;

    public virtual bool SkinnedUi_UseExtendedView { get; set; }

    /// <summary>
    ///     Initializes the pigeonhole file
    /// </summary>
    /// <param name="filename">full path to the json pigeonhole file</param>
    public static void Initialize(string filename)
    {
        if (Initialized) return;

        _instance = Load<BmpPigeonhole>(filename).EnableAutosave();
    }
}