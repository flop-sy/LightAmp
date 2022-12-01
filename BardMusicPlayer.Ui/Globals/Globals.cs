using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Ui.Globals
{
    public static class Globals
    {
        public static string FileFilters = "MMSong files|*.mmsong|MIDI files|*.mid;*.midi|MML files|*.mml|GP files|*.gp*|All files|*.mid;*.midi;*.mmsong;*.mml;*.gp*";
        public static string MusicCatalogFilters = "Amp Catalog file|*.db";
        public static string DataPath;
        public enum Autostart_Types
        {
            NONE = 0,
            VIA_CHAT,
            VIA_METRONOME,
            UNUSED
        }

        public static event EventHandler OnConfigReload;
        public static void ReloadConfig()
        {
            OnConfigReload?.Invoke(null, null);
        }

    }
}
