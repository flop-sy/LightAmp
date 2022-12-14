#region

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using BardMusicPlayer.Seer.Events;

#endregion

// Fixes and colors chat accordingly

namespace BardMusicPlayer.Ui;

internal static class BmpChatParser
{
    public static string Fixup(ChatLog item)
    {
        var rgx = new Regex("^([^ ]+ [^:]+):(.+)");
        var format = rgx.Replace(item.ChatLogLine, "$1: $2");

        switch (item.ChatLogCode)
        {
            case "000E":
            {
                // Party
                var pid = (format[0] & 0xF) + 1;
                format = $"[{pid}] {format.Substring(1)}";
                break;
            }
            case "000D":
            {
                // PM receive
                if (format.IndexOf(": ", StringComparison.Ordinal) != -1) format = format.Replace(": ", " >> ");

                break;
            }
            case "000C":
            {
                // PM Send
                if (format.IndexOf(": ", StringComparison.Ordinal) != -1) format = ">> " + format;

                break;
            }
            case "001B":
            {
                // Novice Network
                format = "[NN] " + format;
                break;
            }
            case "001C":
            {
                // Custom Emote
                if (format.IndexOf(": ", StringComparison.Ordinal) != -1) format = format.Replace(": ", "");

                break;
            }
            case "001D":
            {
                // Standard Emote
                if (format.IndexOf(": ", StringComparison.Ordinal) != -1)
                    format = format.Substring(format.IndexOf(": ", StringComparison.Ordinal) + 2);

                break;
            }
            case "0018":
            {
                // FC
                format = $"<FC> {format}";
                break;
            }
            case "0010":
            case "0011":
            case "0012":
            case "0013":
            case "0014":
            case "0015":
            case "0016":
            case "0017":
            {
                break;
            }
        }

        return format;
    }

    public static KeyValuePair<string, Color> FormatChat(ChatLog item)
    {
        var format = Fixup(item);
        var timestamp = item.ChatLogTimeStamp.ToShortTimeString();

        var col = Color.FromRgb(255, 255, 255);
        switch (item.ChatLogCode)
        {
            case "000E":
            {
                // Party
                col = Color.FromArgb(255, 150, 150, 250);
                break;
            }
            case "000D":
            {
                // PM receive
                col = Color.FromArgb(255, 150, 150, 250);
                break;
            }
            case "000C":
            {
                // PM Send
                col = Color.FromArgb(255, 150, 150, 250);
                break;
            }
            case "001D":
            {
                // Emote
                col = Color.FromArgb(255, 250, 150, 150);
                break;
            }
            case "001C":
            {
                // Custom emote
                col = Color.FromArgb(255, 250, 150, 150);
                break;
            }
            case "000A":
            {
                // Say
                col = Color.FromArgb(255, 240, 240, 240);
                break;
            }
            case "0839":
            {
                // System
                col = Color.FromArgb(255, 20, 20, 20);
                break;
            }
            case "0018":
            {
                // FC
                col = Color.FromArgb(255, 150, 200, 150);
                break;
            }
            case "0010":
            case "0011":
            case "0012":
            case "0013":
            case "0014":
            case "0015":
            case "0016":
            case "0017":
            {
                col = Color.FromArgb(255, 200, 200, 150);
                break;
            }
            default:
            {
                col = Color.FromArgb(255, 200, 200, 200);
                break;
            }
        }

        format = $"[{timestamp}] {format}";
        return new KeyValuePair<string, Color>(format, col);
    }

    public static void AppendText(this RichTextBox box, ChatLog ev)
    {
        var bc = new BrushConverter();
        var tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
        var t = FormatChat(ev);
        tr.Text = t.Key;
        try
        {
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(t.Value));
        }
        catch (FormatException)
        {
        }

        box.AppendText("\r");
    }
}