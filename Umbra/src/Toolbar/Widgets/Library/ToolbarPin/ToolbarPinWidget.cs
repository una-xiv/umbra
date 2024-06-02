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
using Dalamud.Interface;
using Umbra.Common;

namespace Umbra.Widgets;

[ToolbarWidget("ToolbarPin", "Widget.ToolbarPin.Name", "Widget.ToolbarPin.Description")]
internal class ToolbarPinWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.OnClick += _ => { ConfigManager.Set("Toolbar.IsAutoHideEnabled", !Toolbar.IsAutoHideEnabled); };
    }

    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));

        Node.Style.Padding                            = new(0, 2);
        Node.QuerySelector("Label")!.Style.Font       = 2;
        Node.QuerySelector("Label")!.Style.TextOffset = new(0, GetConfigValue<int>("IconYOffset"));

        SetLabel(
            Toolbar.IsAutoHideEnabled
                ? FontAwesomeIcon.LockOpen.ToIconString()
                : FontAwesomeIcon.Lock.ToIconString()
        );
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.ToolbarPin.Config.Decorate.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.Decorate.Description"),
                true
            ),
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.ToolbarPin.Config.IconYOffset.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.IconYOffset.Description"),
                -1,
                -5,
                5
            )
        ];
    }
}
