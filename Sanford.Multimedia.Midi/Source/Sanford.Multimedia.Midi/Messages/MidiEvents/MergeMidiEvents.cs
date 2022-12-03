﻿#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Takes a number of MidiEvents and merges them into a new single MidiEvent source
    /// </summary>
    public class MergeMidiEvents : MidiEvents
    {
        private readonly List<MidiEvents> FMidiEventsList = new List<MidiEvents>();

        public MergeMidiEvents(IEnumerable<MidiEvents> midiEvents)
        {
            foreach (var elem in midiEvents.Where(elem => elem != null))
                FMidiEventsList.Add(elem);
        }

        public IEnumerable<MidiEvents> EventSources => FMidiEventsList;
        public int DeviceID => -3;

        public void Dispose()
        {
        }

        public event MidiMessageEventHandler MessageReceived
        {
            add
            {
                foreach (var elem in FMidiEventsList) elem.MessageReceived += value;
            }
            remove
            {
                foreach (var elem in FMidiEventsList) elem.MessageReceived -= value;
            }
        }

        public event EventHandler<ShortMessageEventArgs> ShortMessageReceived
        {
            add
            {
                foreach (var elem in FMidiEventsList) elem.ShortMessageReceived += value;
            }
            remove
            {
                foreach (var elem in FMidiEventsList) elem.ShortMessageReceived -= value;
            }
        }

        public event EventHandler<ChannelMessageEventArgs> ChannelMessageReceived
        {
            add
            {
                foreach (var elem in FMidiEventsList) elem.ChannelMessageReceived += value;
            }
            remove
            {
                foreach (var elem in FMidiEventsList) elem.ChannelMessageReceived -= value;
            }
        }

        public event EventHandler<SysExMessageEventArgs> SysExMessageReceived
        {
            add
            {
                foreach (var elem in FMidiEventsList) elem.SysExMessageReceived += value;
            }
            remove
            {
                foreach (var elem in FMidiEventsList) elem.SysExMessageReceived -= value;
            }
        }

        public event EventHandler<SysCommonMessageEventArgs> SysCommonMessageReceived
        {
            add
            {
                foreach (var elem in FMidiEventsList) elem.SysCommonMessageReceived += value;
            }
            remove
            {
                foreach (var elem in FMidiEventsList) elem.SysCommonMessageReceived -= value;
            }
        }

        public event EventHandler<SysRealtimeMessageEventArgs> SysRealtimeMessageReceived
        {
            add
            {
                foreach (var elem in FMidiEventsList) elem.SysRealtimeMessageReceived += value;
            }
            remove
            {
                foreach (var elem in FMidiEventsList) elem.SysRealtimeMessageReceived -= value;
            }
        }
    }
}