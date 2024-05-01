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
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.DtrBar;

[Service]
internal partial class DtrBarWidget : IToolbarWidget, IDisposable
{
    [ConfigVariable("Toolbar.Widget.DtrBar.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.ItemSpacing", "ToolbarSettings", "ToolbarCustomization", min: 1, max: 32)]
    private static int ItemSpacing { get; set; } = 6;

    [ConfigVariable("Toolbar.Widget.DtrBar.HideNative", "ToolbarSettings", "ServerInfoBarSettings")]
    private static bool HideNativeDtrBar { get; set; } = true;

    private readonly Dictionary<string, Element> _elements = [];
    private readonly IGameGui                    _gameGui;

    public DtrBarWidget(IDtrBarEntryRepository repository, IGameGui gameGui)
    {
        _gameGui = gameGui;

        repository.OnEntryAdded   += OnEntryAdded;
        repository.OnEntryRemoved += OnEntryRemoved;
        repository.OnEntryUpdated += OnEntryUpdated;
    }

    public void OnDraw() { }

    public void OnUpdate()
    {
        Element.IsVisible = Enabled;
        ToggleNativeServerInfoBarVisibility(!HideNativeDtrBar || (HideNativeDtrBar && !Element.IsVisible));
    }

    public void Dispose()
    {
        ToggleNativeServerInfoBarVisibility(true);
    }

    private void OnEntryAdded(DtrBarEntry entry)
    {
        var element = CreateEntry(entry);
        Element.AddChild(element);

        _elements[entry.Name] = element;
    }

    private void OnEntryRemoved(DtrBarEntry entry)
    {
        if (!_elements.TryGetValue(entry.Name, out var el)) return;

        el.Parent!.RemoveChild(el);
        _elements.Remove(entry.Name);
    }

    private void OnEntryUpdated(DtrBarEntry entry)
    {
        if (!_elements.TryGetValue(entry.Name, out var el)) return;

        el.SortIndex  = -entry.SortIndex;
        el.IsVisible  = entry.IsVisible;
        el.IsDisabled = !entry.IsInteractive;

        string? tooltipText = entry.TooltipText?.TextValue;
        el.Tooltip = !string.IsNullOrEmpty(tooltipText) ? tooltipText : null;

        el.Get<SeStringElement>().SeString = entry.Text;
    }

    private unsafe void ToggleNativeServerInfoBarVisibility(bool isVisible)
    {
        var dtrBar = (AtkUnitBase*) _gameGui.GetAddonByName("_DTR");
        if (dtrBar != null && dtrBar->IsVisible != isVisible) {
            dtrBar->IsVisible = isVisible;
        }
    }
}
