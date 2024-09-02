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
[ToolbarWidgetTags(["pin", "lock", "unlock", "autohide"])]
internal class ToolbarPinWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : IconToolbarWidget(info, guid, configValues)
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
        SetIcon(Toolbar.IsAutoHideEnabled
            ? GetConfigValue<FontAwesomeIcon>("UnlockIcon")
            : GetConfigValue<FontAwesomeIcon>("LockIcon")
        );

        base.OnUpdate();
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new FaIconWidgetConfigVariable(
                "LockIcon",
                I18N.Translate("Widget.ToolbarPin.Config.LockIcon.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.LockIcon.Description"),
                FontAwesomeIcon.Lock
            ),
            new FaIconWidgetConfigVariable(
                "UnlockIcon",
                I18N.Translate("Widget.ToolbarPin.Config.UnlockIcon.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.UnlockIcon.Description"),
                FontAwesomeIcon.LockOpen
            ),
            ..DefaultIconToolbarWidgetConfigVariables,
        ];
    }
}
