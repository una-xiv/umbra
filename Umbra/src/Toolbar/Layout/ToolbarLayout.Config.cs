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
using Newtonsoft.Json;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar;

internal partial class ToolbarLayout
{
    [ConfigVariable("Toolbar.WidgetLayoutConfig")]
    private static string WidgetLayoutConfig { get; set; } = "{}";

    private LayoutConfigData LayoutConfig { get; set; } = new();

    private void LoadWidgetLayout()
    {
        if (string.IsNullOrEmpty(WidgetLayoutConfig) || WidgetLayoutConfig == "{}") {
            return;
        }

        try {
            var config = JsonConvert.DeserializeObject<LayoutConfigData>(WidgetLayoutConfig);
            if (null == config) return;

            foreach ((string id, Anchor anchor) in config.Anchors) {
                if (Widgets.TryGetValue(id, out var widget)) {
                    LayoutConfig.Anchors[id] = anchor;
                    widget.Element.Anchor = anchor;
                }
            }

            foreach ((string id, int index) in config.Indices) {
                if (Widgets.TryGetValue(id, out var widget)) {
                    LayoutConfig.Indices[id] = index;
                    widget.Element.SortIndex = index;
                }
            }
        } catch (Exception e) {
            Logger.Warning($"Failed to restore widget layout from config: {e.Message}");
        }
    }

    private void SaveWidgetLayout()
    {
        WidgetLayoutConfig = JsonConvert.SerializeObject(LayoutConfig);
        ConfigManager.Set("Toolbar.WidgetLayoutConfig", WidgetLayoutConfig);
    }

    private class LayoutConfigData
    {
        public Dictionary<string, Anchor> Anchors { get; } = [];
        public Dictionary<string, int>    Indices { get; } = [];
    }
}
