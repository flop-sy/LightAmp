﻿#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile;

internal sealed class KeybindDatFile : IDisposable
{
    private readonly string _filePath;
    public readonly Dictionary<string, Keybind> KeybindList = new();
    internal bool Fresh = true;

    internal KeybindDatFile(string filePath)
    {
        _filePath = filePath;
    }

    public Keybind this[string key] => !KeybindList.ContainsKey(key) ? new Keybind() : KeybindList[key];

    public void Dispose()
    {
        if (KeybindList == null) return;

        foreach (var keyBind in KeybindList.Values) keyBind?.Dispose();

        KeybindList.Clear();
    }

    internal bool Load()
    {
        if (string.IsNullOrEmpty(_filePath)) throw new FileFormatException("No path to KEYBIND.DAT file provided.");

        if (!File.Exists(_filePath)) throw new FileFormatException("Missing KEYBIND.DAT file.");

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
            throw new FileFormatException("Invalid KEYBIND.DAT size.");
        }

        reader.BaseStream.Seek(0x60, SeekOrigin.Begin);
        try
        {
            reader.BaseStream.Seek(0x11, SeekOrigin.Begin);
            while (reader.BaseStream.Position < dataSize)
            {
                var command = ParseSection(reader);
                var keybind = ParseSection(reader);

                var key = Encoding.UTF8.GetString(command.Data);
                key = key.Substring(0, key.Length - 1); // Trim off \0
                var dat = Encoding.UTF8.GetString(keybind.Data);
                var datKeys = dat.Split(',');
                if (datKeys.Length != 3) continue;

                var key1 = datKeys[0].Split('.');
                var key2 = datKeys[1].Split('.');
                KeybindList.Add(key, new Keybind
                {
                    MainKey1 = int.Parse(key1[0], NumberStyles.HexNumber),
                    MainKey2 = int.Parse(key2[0], NumberStyles.HexNumber),
                    ModKey1 = int.Parse(key1[1], NumberStyles.HexNumber),
                    ModKey2 = int.Parse(key2[1], NumberStyles.HexNumber)
                });
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

    public Keys GetKeybindFromKeyString(string nk)
    {
        return !string.IsNullOrEmpty(nk) ? this[nk].GetKey() : Keys.None;
    }

    private static KeybindSection ParseSection(BinaryReader stream)
    {
        var headerBytes = XorTools.ReadXorBytes(stream, 3, 0x73);
        var section = new KeybindSection
        {
            Type = headerBytes[0],
            Size = headerBytes[1]
        };
        section.Data = XorTools.ReadXorBytes(stream, section.Size, 0x73);
        Array.Reverse(section.Data);
        return section;
    }

    ~KeybindDatFile()
    {
        Dispose();
    }
}