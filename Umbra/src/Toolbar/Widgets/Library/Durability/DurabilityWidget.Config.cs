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

internal partial class DurabilityWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "HideWhenOkay",
                I18N.Translate("Widget.Durability.Config.HideWhenOkay.Name"),
                I18N.Translate("Widget.Durability.Config.HideWhenOkay.Description"),
                false
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.Durability.Config.Decorate.Name"),
                I18N.Translate("Widget.Durability.Config.Decorate.Description"),
                true
            ),
            new SelectWidgetConfigVariable(
                "DisplayMode",
                I18N.Translate("Widget.Durability.Config.DisplayMode.Name"),
                I18N.Translate("Widget.Durability.Config.DisplayMode.Description"),
                "Full",
                new() {
                    { "Full", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.Full") },
                    { "Short", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.Short") },
                    { "DurabilityOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.DurabilityOnly") },
                    { "SpiritbondOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.SpiritbondOnly") },
                    { "IconOnly", I18N.Translate("Widget.Durability.Config.DisplayMode.Option.IconOnly") }
                }
            ),
            new IntegerWidgetConfigVariable(
                "WarningThreshold",
                I18N.Translate("Widget.Durability.Config.WarningThreshold.Name"),
                I18N.Translate("Widget.Durability.Config.WarningThreshold.Description"),
                50,
                1,
                100
            ),
            new IntegerWidgetConfigVariable(
                "CriticalThreshold",
                I18N.Translate("Widget.Durability.Config.CriticalThreshold.Name"),
                I18N.Translate("Widget.Durability.Config.CriticalThreshold.Description"),
                25,
                0,
                100
            ),
            new SelectWidgetConfigVariable(
                "TextAlign",
                I18N.Translate("Widget.Durability.Config.TextAlign.Name"),
                I18N.Translate("Widget.Durability.Config.TextAlign.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Durability.Config.TextAlign.Option.Left") },
                    { "Center", I18N.Translate("Widget.Durability.Config.TextAlign.Option.Center") },
                    { "Right", I18N.Translate("Widget.Durability.Config.TextAlign.Option.Right") }
                }
            ),
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.Durability.Config.IconLocation.Name"),
                I18N.Translate("Widget.Durability.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.Flag.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.Flag.Config.IconLocation.Option.Right") }
                }
            )
        ];
    }
}
