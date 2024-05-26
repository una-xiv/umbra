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

public partial class GearsetSwitcherWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new SelectWidgetConfigVariable(
                "DisplayMode",
                I18N.Translate("Widget.GearsetSwitcher.Config.DisplayMode.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.DisplayMode.Description"),
                "TextAndIcon",
                new() {
                    { "TextAndIcon", I18N.Translate("Widget.GearsetSwitcher.Config.DisplayMode.Option.TextAndIcon") },
                    { "TextOnly", I18N.Translate("Widget.GearsetSwitcher.Config.DisplayMode.Option.TextOnly") },
                    { "IconOnly", I18N.Translate("Widget.GearsetSwitcher.Config.DisplayMode.Option.IconOnly") }
                }
            ),
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.GearsetSwitcher.Config.IconLocation.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.GearsetSwitcher.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.GearsetSwitcher.Config.IconLocation.Option.Right") }
                }
            ),
            new SelectWidgetConfigVariable(
                "TextAlign",
                I18N.Translate("Widget.GearsetSwitcher.Config.TextAlign.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.TextAlign.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.GearsetSwitcher.Config.TextAlign.Option.Left") },
                    { "Center", I18N.Translate("Widget.GearsetSwitcher.Config.TextAlign.Option.Center") },
                    { "Right", I18N.Translate("Widget.GearsetSwitcher.Config.TextAlign.Option.Right") }
                }
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.GearsetSwitcher.Config.Decorate.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.Decorate.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "UseAlternateIconWidget",
                I18N.Translate("Widget.GearsetSwitcher.Config.UseAlternateIconWidget.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.UseAlternateIconWidget.Description"),
                false
            ),
            new BooleanWidgetConfigVariable(
                "UseAlternateIconHeader",
                I18N.Translate("Widget.GearsetSwitcher.Config.UseAlternateIconHeader.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.UseAlternateIconHeader.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "UseAlternateIconButton",
                I18N.Translate("Widget.GearsetSwitcher.Config.UseAlternateIconButton.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.UseAlternateIconButton.Description"),
                false
            ),
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.IconYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.IconYOffset.Description"),
                1,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "HeaderIconYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.HeaderIconYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.HeaderIconYOffset.Description"),
                0,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "ButtonIconYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonIconYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonIconYOffset.Description"),
                1,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "NameTextYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.NameTextYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.NameTextYOffset.Description"),
                0,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "InfoTextYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoTextYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoTextYOffset.Description"),
                -1,
                -5,
                5
            ),

            // Role Configuration
            ..AddRoleOptionsFor("Tank",           "LeftColumn",   0, 3),
            ..AddRoleOptionsFor("Healer",         "LeftColumn",   1, 2),
            ..AddRoleOptionsFor("Melee",          "LeftColumn",   2, 5),
            ..AddRoleOptionsFor("PhysicalRanged", "MiddleColumn", 2, 5),
            ..AddRoleOptionsFor("MagicalRanged",  "MiddleColumn", 2, 5),
            ..AddRoleOptionsFor("Crafter",        "RightColumn",  1, 7),
            ..AddRoleOptionsFor("Gatherer",       "RightColumn",  0, 3)
        ];
    }

    private static IEnumerable<IWidgetConfigVariable> AddRoleOptionsFor(
        string name, string column, int sortIndex, int maxItems
    )
    {
        string role = I18N.Translate($"Widget.GearsetSwitcher.Role.{name}");

        return [
            new BooleanWidgetConfigVariable(
                $"Show{name}",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRole.Name",        role),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRole.Description", role),
                true
            ),
            new SelectWidgetConfigVariable(
                $"{name}RoleLocation",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Name",        role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Description", role),
                column,
                new() {
                    { "LeftColumn", I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Option.LeftColumn") }, {
                        "MiddleColumn", I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Option.MiddleColumn")
                    },
                    { "RightColumn", I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Option.RightColumn") }
                }
            ),
            new IntegerWidgetConfigVariable(
                $"{name}RoleSortIndex",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleSortIndex.Name",        role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleSortIndex.Description", role),
                sortIndex,
                0,
                10
            ),
            new IntegerWidgetConfigVariable(
                $"{name}MaxItems",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleMaxItems.Name",        role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleMaxItems.Description", role),
                maxItems,
                1,
                10
            )
        ];
    }
}
