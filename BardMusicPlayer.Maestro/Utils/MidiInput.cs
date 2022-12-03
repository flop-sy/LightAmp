#region

using System.Collections.Generic;
using Sanford.Multimedia.Midi;

#endregion

namespace BardMusicPlayer.Maestro.Utils
{
    public static class MidiInput
    {
        public static Dictionary<int, string> ReloadMidiInputDevices()
        {
            var midiInputs = new Dictionary<int, string> { { -1, "None" } };
            for (var i = 0; i < InputDevice.DeviceCount; i++)
            {
                var cap = InputDevice.GetDeviceCapabilities(i);
                midiInputs.Add(i, cap.name);
            }

            return midiInputs;
        }

        public struct MidiInputDescription
        {
            public string name;
            public int id;

            public MidiInputDescription(string n, int i)
            {
                name = n;
                id = i;
            }
        }
    }
}