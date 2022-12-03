#region

using System;
using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Events
{
    internal sealed class SignaturesFoundEvent : EventArgs
    {
        public SignaturesFoundEvent(object sender, Dictionary<string, Signature> signatures, long processingTime)
        {
            Sender = sender;
            Signatures = signatures;
            ProcessingTime = processingTime;
        }

        public long ProcessingTime { get; set; }

        public object Sender { get; set; }

        public Dictionary<string, Signature> Signatures { get; }
    }
}