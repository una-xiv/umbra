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

using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
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

        Node.QuerySelector("#CloseButton")!.OnMouseUp += _ => Close();
    }

    protected override void OnOpen()
    {
        Manager.OnWidgetRemoved += OnWidgetRemoved;

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
}
