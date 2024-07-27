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
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("Spacer", "Widget.Spacer.Name", "Widget.Spacer.Description")]
internal class SpacerWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup { get; } = null;

    public override Node Node { get; } = new() {
        ClassList = ["toolbar-widget-spacer"],
        Style = new() {
            Anchor = Anchor.MiddleLeft,
            Size   = new(2, SafeHeight),
        },
    };

    protected override void Initialize()
    {
    }

    protected override void OnUpdate()
    {
        Node.IsDisabled = true;
        Node.Style.Size = new(GetConfigValue<int>("Width"), SafeHeight);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new IntegerWidgetConfigVariable(
                "Width",
                I18N.Translate("Widget.Spacer.Config.Width.Name"),
                I18N.Translate("Widget.Spacer.Config.Width.Description"),
                10,
                0,
                2000
            )
        ];
    }
}
