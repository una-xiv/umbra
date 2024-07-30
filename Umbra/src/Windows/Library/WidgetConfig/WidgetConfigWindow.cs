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

using Lumina.Misc;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class WidgetConfigWindow : Window
{
    protected override string  Title       { get; }
    protected override Vector2 MinSize     { get; } = new(512, 300);
    protected override Vector2 MaxSize     { get; } = new(720, 1200);
    protected override Vector2 DefaultSize { get; } = new(512, 300);

    private WidgetManager Manager  { get; }
    private ToolbarWidget Instance { get; }

    public WidgetConfigWindow(string guid)
    {
        Manager  = Framework.Service<WidgetManager>();
        Instance = Manager.GetInstance(guid);
        Title    = Instance.Info.Name;
    }

    protected override void OnOpen()
    {
        Manager.OnWidgetRemoved       += OnWidgetRemoved;
        Instance.OnConfigValueChanged += OnConfigValueChanged;

        Node.QuerySelector("#CloseButton")!.OnMouseUp        += CloseWindow;
        Node.QuerySelector("#CopyToClipboard")!.OnMouseUp    += CopyToClipboard;
        Node.QuerySelector("#PasteFromClipboard")!.OnMouseUp += PasteFromClipboard;

        Node.QuerySelector("#PasteFromClipboard")!.IsDisabled = !Manager.HasInstanceClipboardData(Instance);

        Dictionary<string, Node> groups = new();

        foreach (var opt in Instance.GetConfigVariableList()) {
            if (string.IsNullOrEmpty(opt.Category)) {
                RenderControl(opt, ControlsListNode);
                continue;
            }

            if (!groups.TryGetValue(opt.Category, out Node? categoryNode)) {
                categoryNode = RenderCategory(opt.Category);
                groups[opt.Category] = categoryNode;
            }

            RenderControl(opt, categoryNode);
        }
    }

    protected override void OnClose()
    {
        Manager.OnWidgetRemoved -= OnWidgetRemoved;

        Node.QuerySelector("#CloseButton")!.OnMouseUp        -= CloseWindow;
        Node.QuerySelector("#CopyToClipboard")!.OnMouseUp    -= CopyToClipboard;
        Node.QuerySelector("#PasteFromClipboard")!.OnMouseUp -= PasteFromClipboard;
    }

    protected override void OnUpdate(int instanceId)
    {
        UpdateNodeSizes();
    }

    private void OnWidgetRemoved(ToolbarWidget widget)
    {
        if (widget == Instance) {
            Close();
        }
    }

    private void OnConfigValueChanged(IWidgetConfigVariable cvar)
    {
        var ctrl = Node.QuerySelector($"#control-{Crc32.Get(cvar.Id)}");
        if (ctrl == null) return;

        if (cvar is IntegerWidgetConfigVariable i) {
            ((IntegerInputNode)ctrl).Value = Instance.GetConfigValue<int>(i.Id);
        } else if (cvar is FloatWidgetConfigVariable f) {
            ((FloatInputNode)ctrl).Value = Instance.GetConfigValue<float>(f.Id);
        } else if (cvar is StringWidgetConfigVariable s) {
            ((StringInputNode)ctrl).Value = Instance.GetConfigValue<string>(s.Id);
        } else if (cvar is SelectWidgetConfigVariable l && l.Options.ContainsKey(Instance.GetConfigValue<string>(l.Id))) {
            ((SelectNode)ctrl).Value = l.Options[Instance.GetConfigValue<string>(l.Id)];
        } else if (cvar is BooleanWidgetConfigVariable b) {
            ((CheckboxNode)ctrl).Value = Instance.GetConfigValue<bool>(b.Id);
        }
    }

    private void CloseWindow(Node _)
    {
        Close();
    }

    private void CopyToClipboard(Node _)
    {
        Instance.CopyInstanceSettingsToClipboard();
    }

    private void PasteFromClipboard(Node _)
    {
        Instance.PasteInstanceSettingsFromClipboard();
    }
}
