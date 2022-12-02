#region

using System;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal class Keybind : IDisposable
    {
        public int MainKey1 { get; set; } = 0;

        public int ModKey1 { get; set; } = 0;

        public int MainKey2 { get; set; } = 0;

        public int ModKey2 { get; set; } = 0;

        public void Dispose()
        {
        }

        public Keys GetKey()
        {
            return GetKey1() != Keys.None ? GetKey1() : GetKey2();
        }

        public Keys GetKey1()
        {
            return GetMain(MainKey1) | GetMod(ModKey1);
        }

        public Keys GetKey2()
        {
            return GetMain(MainKey2) | GetMod(ModKey2);
        }

        private static Keys GetMain(int key)
        {
            if (key < 130)
                return (Keys)key;
            if (KeyDictionary.MainKeyMap.ContainsKey(key))
                return (Keys)KeyDictionary.MainKeyMap[key];

            return Keys.None;
        }

        private static Keys GetMod(int mod)
        {
            var modKeys = Keys.None;
            if ((mod & 1) != 0) modKeys |= Keys.Shift;
            if ((mod & 2) != 0) modKeys |= Keys.Control;
            if ((mod & 4) != 0) modKeys |= Keys.Alt;
            return modKeys;
        }

        public override string ToString()
        {
            var key = GetKey();
            if (key == Keys.None) return string.Empty;

            var str = key.ToString();
            if (KeyDictionary.OemKeyFix.ContainsKey(str)) str = KeyDictionary.OemKeyFix[str];
            return str;
        }

        ~Keybind()
        {
            Dispose();
        }
    }
}