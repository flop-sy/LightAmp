#region

using System.Text.RegularExpressions;

#endregion

namespace BardMusicPlayer.Seer.Events
{
    public sealed class HomeWorldChanged : SeerEvent
    {
        internal HomeWorldChanged(EventSource readerBackendType, string homeWorld) : base(readerBackendType)
        {
            EventType = GetType();
            HomeWorld = homeWorld;
        }

        public string HomeWorld { get; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(HomeWorld) && Regex.IsMatch(HomeWorld, @"^[a-zA-Z]+$");
        }
    }
}