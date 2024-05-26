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

namespace Umbra.Widgets;

public partial class WorldNameWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "HideOnHomeWorld",
                I18N.Translate("Widget.WorldName.Config.HideOnHomeWorld.Name"),
                I18N.Translate("Widget.WorldName.Config.HideOnHomeWorld.Description"),
                false
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.WorldName.Config.Decorate.Name"),
                I18N.Translate("Widget.WorldName.Config.Decorate.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowIcon",
                I18N.Translate("Widget.WorldName.Config.ShowIcon.Name"),
                I18N.Translate("Widget.WorldName.Config.ShowIcon.Description"),
                true
            ),
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.WorldName.Config.IconLocation.Name"),
                I18N.Translate("Widget.WorldName.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.WorldName.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.WorldName.Config.IconLocation.Option.Right") },
                }
            ),
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.WorldName.Config.TextYOffset.Name"),
                I18N.Translate("Widget.WorldName.Config.TextYOffset.Description"),
                0,
                -5,
                5
            )
        ];
    }
}
