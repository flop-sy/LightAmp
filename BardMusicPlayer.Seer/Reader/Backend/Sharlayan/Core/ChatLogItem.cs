#region

using System;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Interfaces;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core
{
    internal class ChatLogItem : IChatLogItem
    {
        public byte[] Bytes { get; set; }

        public string Code { get; set; }

        public string Combined { get; set; }

        public bool JP { get; set; }

        public string Line { get; set; }

        public string Raw { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}