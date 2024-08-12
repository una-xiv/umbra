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

[ToolbarWidget("MailIndicator", "Widget.MailIndicator.Name", "Widget.MailIndicator.Description")]
internal partial class MailIndicatorWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : IconToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetIcon(FontAwesomeIcon.Envelope);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        uint unreadMailCount = GetUnreadMailCount();

        SetDisabled(unreadMailCount == 0);

        Node.Tooltip         = I18N.Translate($"Widget.MailIndicator.Tooltip.{(unreadMailCount == 1 ? "Singular" : "Plural")}", unreadMailCount.ToString());
        Node.Style.IsVisible = GetConfigValue<bool>("AlwaysShow") || (unreadMailCount > 0);

        base.OnUpdate();
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "AlwaysShow",
                I18N.Translate("Widget.MailIndicator.Config.AlwaysShow.Name"),
                I18N.Translate("Widget.MailIndicator.Config.AlwaysShow.Description"),
                false
            ),
            ..DefaultIconToolbarWidgetConfigVariables,
        ];
    }
}
