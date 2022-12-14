#region

using System.Diagnostics;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models
{
    internal sealed class ProcessModel
    {
        public Process Process { get; set; }

        public int ProcessID => Process?.Id ?? -1;

        public string ProcessName => Process?.ProcessName ?? string.Empty;
    }
}