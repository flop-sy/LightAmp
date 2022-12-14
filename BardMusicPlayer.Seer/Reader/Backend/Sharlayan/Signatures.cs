#region

#region

using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
    internal static class Signatures
    {
        public static string CharacterMapKey => "CHARMAP";

        public static string ChatLogKey => "CHATLOG";

        public static string PartyCountKey => "PARTYCOUNT";

        public static string PartyMapKey => "PARTYMAP";

        public static string PlayerInformationKey => "PLAYERINFO";

        public static string PerformanceStatusKey => "PERFSTATUS";

        public static string CharacterIdKey => "CHARID";

        public static string ChatInputKey => "CHATINPUT";

        public static string WorldKey => "WORLD";

        public static IEnumerable<Signature> Resolve(MemoryHandler memoryHandler)
        {
            return new APIHelper(memoryHandler).GetSignatures();
        }
    }
}