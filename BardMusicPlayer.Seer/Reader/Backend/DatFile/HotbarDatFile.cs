﻿#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile
{
    internal sealed class HotbarDatFile : IDisposable
    {
        private readonly string _filePath;
        private readonly HotbarData _hotbarData = new();

        internal bool Fresh = true;

        internal HotbarDatFile(string filePath)
        {
            _filePath = filePath;
        }

        public void Dispose()
        {
            _hotbarData?.Dispose();
        }

        internal bool Load()
        {
            if (string.IsNullOrEmpty(_filePath)) throw new FileFormatException("No path to HOTBAR.DAT file provided.");
            if (!File.Exists(_filePath)) throw new FileFormatException("Missing HOTBAR.DAT file.");

            using var fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var memStream = new MemoryStream();
            if (fileStream.CanRead && fileStream.CanSeek) fileStream.CopyTo(memStream);

            fileStream.Dispose();
            if (memStream.Length == 0)
            {
                memStream.Dispose();
                return false;
            }

            using var reader = new BinaryReader(memStream);
            reader.BaseStream.Seek(0x04, SeekOrigin.Begin);

            var fileSize = XorTools.ReadXorInt32(reader);
            var dataSize = XorTools.ReadXorInt32(reader) + 16;

            var sourceSize = reader.BaseStream.Length;

            if (sourceSize - fileSize != 32)
            {
                reader.Dispose();
                memStream.Dispose();
                throw new FileFormatException("Invalid HOTBAR.DAT size.");
            }

            reader.BaseStream.Seek(0x60, SeekOrigin.Begin);
            try
            {
                reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                while (reader.BaseStream.Position < dataSize)
                {
                    var ac = ParseSection(reader);
                    if (ac.Job != 0x17 && ac.Job != 0) continue;

                    if (ac.Type == 0x1D) _hotbarData[ac.Hotbar][ac.Slot][ac.Job] = ac;
                }
            }
            catch (Exception ex)
            {
                throw new FileFormatException("Invalid HOTBAR.DAT format: " + ex.Message);
            }
            finally
            {
                reader.Dispose();
                memStream.Dispose();
            }

            return true;
        }

        public IEnumerable<HotbarSlot> GetSlotsFromType(SlotType type)
        {
            return GetSlotsFromType((int)type);
        }

        public IEnumerable<HotbarSlot> GetSlotsFromType(int type)
        {
            return (from row in _hotbarData.Rows.Values
                from jobSlot in row.Slots.Values
                from slot in jobSlot.JobSlots.Values
                where slot.Type == type
                select slot).ToList();
        }

        public List<HotbarSlot> GetBRDSlots()
        {
            return (from row in _hotbarData.Rows.Values
                from jobSlot in row.Slots.Values
                from slot in jobSlot.JobSlots.Values
                where slot.Job == 0x17
                select slot).ToList();
        }

        public List<HotbarSlot> GetGlobalSlots()
        {
            return (from row in _hotbarData.Rows.Values
                from jobSlot in row.Slots.Values
                from slot in jobSlot.JobSlots.Values
                where slot.Job == 0
                select slot).ToList();
        }

        public string GetInstrumentToneKeyMap(InstrumentTone instrumentTone)
        {
            var slots = GetSlotsFromType(SlotType.InstrumentTone);
            foreach (var slot in slots.Where(slot => slot.Action == instrumentTone)) return slot.ToString();

            return string.Empty;
        }

        public string GetInstrumentKeyMap(Instrument instrument)
        {
            var slots = GetSlotsFromType(SlotType.Instrument);
            //read only the bard
            foreach (var slot in slots.Where(slot => slot.Action == instrument && slot.Job == 0x17))
                return slot.ToString();

            return string.Empty;
        }

        private static HotbarSlot ParseSection(BinaryReader stream)
        {
            const byte xor = 0x31;
            var ac = new HotbarSlot
            {
                Action = XorTools.ReadXorByte(stream, xor),
                Flag = XorTools.ReadXorByte(stream, xor),
                Unk1 = XorTools.ReadXorByte(stream, xor),
                Unk2 = XorTools.ReadXorByte(stream, xor),
                Job = XorTools.ReadXorByte(stream, xor),
                Hotbar = XorTools.ReadXorByte(stream, xor),
                Slot = XorTools.ReadXorByte(stream, xor),
                Type = XorTools.ReadXorByte(stream, xor)
            };
            return ac;
        }

        ~HotbarDatFile()
        {
            Dispose();
        }

        internal enum SlotType
        {
            Unknown,
            Instrument = 0x1D,

            InstrumentTone =
                Unknown // Leaving this as unknown as we can just use 'Instrument' for now, they have the same id in hex.
        }
    }
}