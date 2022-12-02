﻿#region

using System;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core;

#endregion

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ChatLog : SeerEvent
    {
        internal ChatLog(EventSource readerBackendType, Game game, ChatLogItem item) : base(readerBackendType)
        {
            EventType = GetType();
            ChatLogGame = game;
            ChatLogTimeStamp = item.TimeStamp;
            ChatLogCode = item.Code;
            ChatLogLine = item.Line;
        }

        public Game ChatLogGame { get; }
        public DateTime ChatLogTimeStamp { get; }
        public string ChatLogCode { get; }
        public string ChatLogLine { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}