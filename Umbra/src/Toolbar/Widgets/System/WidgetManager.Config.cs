/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Umbra.Common;

namespace Umbra.Widgets.System;

internal partial class WidgetManager
{
    [ConfigVariable("Toolbar.WidgetData")]
    private static string WidgetConfigData { get; set; } = "";

    private readonly Dictionary<string, WidgetConfigStruct> _widgetState = [];

    private bool    _isLoadingState;

    public void SaveState()
    {
        ConfigManager.Set("Toolbar.WidgetData", Encode(JsonConvert.SerializeObject(_widgetState)));
    }

    public void LoadState()
    {
        if (string.IsNullOrEmpty(WidgetConfigData)) {
            if (_instances.Count > 0) {
                foreach (var widget in _instances.Values) {
                    RemoveWidget(widget.Id);
                }
            }
            return;
        }

        string json = Decode(WidgetConfigData);
        var    data = JsonConvert.DeserializeObject<Dictionary<string, WidgetConfigStruct>>(json);
        if (data is null) return;

        _isLoadingState = true;

        foreach ((string guid, WidgetConfigStruct config) in data) {
            if (_instances.ContainsKey(guid)) continue;
            if (!_widgetTypes.ContainsKey(config.Name)) continue;

            _widgetState[guid] = config;
            CreateWidget(config.Name, config.Location, config.SortIndex, guid, config.Config, false);
        }

        // Dispose of any widgets that were not loaded in the config.
        foreach (var widget in _instances.Values) {
            if (_widgetState.Keys.All(guid => guid != widget.Id)) {
                RemoveWidget(widget.Id);
            }
        }

        _isLoadingState = false;
    }

    public void SaveWidgetState(string guid)
    {
        if (!_instances.TryGetValue(guid, out ToolbarWidget? widget)) return;

        _widgetState[guid] = new() {
            Name      = widget.Info.Id,
            Location  = widget.Location,
            SortIndex = widget.SortIndex,
            Config    = widget.GetUserConfig()
        };
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
        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}
