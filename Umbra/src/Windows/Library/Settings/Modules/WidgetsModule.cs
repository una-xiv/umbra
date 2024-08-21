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
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Library.WidgetConfig;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class WidgetsModule : SettingsModule, IDisposable
{
    public override string Id   => "WidgetsModule";
    public override string Name { get; } = I18N.Translate("Settings.WidgetsModule.Name");

    public WidgetsModule()
    {
        var wm = Framework.Service<WidgetManager>();
        wm.OnWidgetCreated   += OnWidgetInstanceCreated;
        wm.OnWidgetRemoved   += OnWidgetInstanceRemoved;
        wm.OnWidgetRelocated += OnWidgetInstanceRelocated;

        Node.QuerySelector("#Left")!.AppendChild(CreateAddNewButton("Left"));
        Node.QuerySelector("#Center")!.AppendChild(CreateAddNewButton("Center"));
        Node.QuerySelector("#Right")!.AppendChild(CreateAddNewButton("Right"));

        Node.QuerySelector("#ManageProfiles")!.OnMouseUp += _ => {
            Framework.Service<WindowManager>().Present("ToolbarProfilesWindow", new ToolbarProfilesWindow());
        };

        foreach (var widget in wm.GetWidgetInstances()) OnWidgetInstanceCreated(widget);
    }

    protected override void OnDisposed()
    {
        var wm = Framework.Service<WidgetManager>();
        wm.OnWidgetCreated   -= OnWidgetInstanceCreated;
        wm.OnWidgetRemoved   -= OnWidgetInstanceRemoved;
        wm.OnWidgetRelocated -= OnWidgetInstanceRelocated;
    }

    public override void OnOpen()
    {
    }

    public override void OnUpdate()
    {
        Node.QuerySelector(".module-header--profile-name")!.NodeValue =
            I18N.Translate("Settings.WidgetsModule.Profile", Framework.Service<WidgetManager>().GetActiveProfileName());

        UpdateNodeSizes();
    }

    private void OnWidgetInstanceCreated(ToolbarWidget widget)
    {
        if (widget.Location == "aux") return;
        var column = GetColumn(widget.Location);
        column.AppendChild(CreateWidgetInstanceNode(widget));
    }

    private void OnWidgetInstanceRemoved(ToolbarWidget widget)
    {
        if (widget.Location == "aux") return;
        var column = GetColumn(widget.Location);
        var node   = column.QuerySelector($"#widget-{widget.Id}");

        if (node != null) column.RemoveChild(node);
    }

    private void OnWidgetInstanceRelocated(ToolbarWidget widget, string previousLocation)
    {
        if (widget.Location == "aux" || previousLocation == "aux") return;
        var oldColumn = GetColumn(previousLocation);
        var newColumn = GetColumn(widget.Location);
        var node      = oldColumn.QuerySelector($"#widget-{widget.Id}");

        if (node != null) {
            oldColumn.RemoveChild(node);
            newColumn.AppendChild(node);
        }
    }
}
