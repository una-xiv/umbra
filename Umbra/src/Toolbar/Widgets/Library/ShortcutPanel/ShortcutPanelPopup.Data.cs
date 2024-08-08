using Dalamud.Plugin.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup
{
    private Dictionary<byte, Dictionary<int, string>> _shortcuts    = [];
    private string                                    _shortcutData = "{}";

    private void AssignShortcut(byte category, int index, string? shortcut)
    {
        if (!_shortcuts.TryGetValue(category, out Dictionary<int, string>? slots)) {
            slots                = [];
            _shortcuts[category] = slots;
        }

        if (string.IsNullOrEmpty(shortcut)) {
            slots.Remove(index);
            EncodeShortcutData();
            return;
        }

        slots[index] = shortcut;
        EncodeShortcutData();
    }

    private string? GetShortcutData(byte category, int index)
    {
        if (_shortcuts.TryGetValue(category, out Dictionary<int, string>? slots)) {
            if (slots.TryGetValue(index, out string? shortcut)) {
                return shortcut;
            }
        }

        return null;
    }

    private void EncodeShortcutData()
    {
        string oldData = _shortcutData;
        _shortcutData = $"SPD|{Encode(JsonConvert.SerializeObject(_shortcuts))}";
        if (oldData != _shortcutData) OnShortcutsChanged?.Invoke(_shortcutData);
    }

    private void DecodeShortcutData(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        string[] parts = data.Split('|');

        if (parts.Length != 2) throw new("Invalid shortcut data. Missing header.");
        if (parts[0] != "SPD") throw new("Invalid shortcut data. Header mismatch.");

        _shortcutData = data;
        _shortcuts = JsonConvert.DeserializeObject<Dictionary<byte, Dictionary<int, string>>>(Decode(parts[1])) ?? [];
    }

    private static string Encode(string text)
    {
        byte[]    bytes  = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(bytes, 0, bytes.Length);
        }

        bytes = output.ToArray();

        return Convert.ToBase64String(bytes);
    }

    private static string Decode(string text)
    {
        if (string.IsNullOrEmpty(text)) return "{}";

        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}
