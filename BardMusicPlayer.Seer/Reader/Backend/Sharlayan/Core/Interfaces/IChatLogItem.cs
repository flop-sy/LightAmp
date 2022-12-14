#region

using System;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Interfaces;

internal interface IChatLogItem
{
    byte[] Bytes { get; set; }

    string Code { get; set; }

    string Combined { get; set; }

    bool JP { get; set; }

    string Line { get; set; }

    string Raw { get; set; }

    DateTime TimeStamp { get; set; }
}