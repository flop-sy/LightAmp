namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models.Structures
{
    internal sealed class StructuresContainer
    {
        public ChatLogPointers ChatLogPointers { get; set; } = new();

        public CurrentPlayer CurrentPlayer { get; set; } = new();

        public PartyMember PartyMember { get; set; } = new();

        public PerformanceInfo PerformanceInfo { get; set; } = new();

        public World World { get; set; } = new();

        public CharacterId CharacterId { get; set; } = new();
    }
}