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
using Dalamud.Plugin.Services;
using Umbra.Common;

namespace Umbra.Widgets;

[ToolbarWidget("Accessibility", "Widget.Accessibility.Name", "Widget.Accessibility.Description")]
internal sealed partial class AccessibilityWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : IconToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override AccessibilityWidgetPopup Popup { get; } = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetIcon(FontAwesomeIcon.Wheelchair);
    }
}
