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

internal partial class GearsetSwitcherWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        Dictionary<string, string> iconTypeChoices = new() {
            { "Default", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Default") },
            { "Framed", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Framed") },
            { "Gearset", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Gearset") },
            { "Glowing", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Glowing") },
            { "Light", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Light") },
            { "Dark", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Dark") },
            { "Gold", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Gold") },
            { "Orange", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Orange") },
            { "Red", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Red") },
            { "Purple", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Purple") },
            { "Blue", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Blue") },
            { "Green", I18N.Translate("Widget.GearsetSwitcher.Config.IconType.Option.Green") }
        };

        Dictionary<string, string> infoTypeChoices = new() {
            { "None", I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Option.None") },
            { "Auto", I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Option.Auto") },
            { "ItemLevel", I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Option.ItemLevel") },
            { "JobLevel", I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Option.JobLevel") },
        };

        return [
            ..DefaultToolbarWidgetConfigVariables,
            new SelectWidgetConfigVariable(
                "InfoType",
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Description"),
                "Auto",
                infoTypeChoices
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "InfoTypeMaxLevel",
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoTypeMaxLevel.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoTypeMaxLevel.Description"),
                "Auto",
                infoTypeChoices
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowSyncedLevelInInfo",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowSyncedLevelInInfo.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowSyncedLevelInInfo.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "WidgetButtonIconType",
                I18N.Translate("Widget.GearsetSwitcher.Config.WidgetButtonIconType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.WidgetButtonIconType.Description"),
                "Default",
                iconTypeChoices
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..SingleLabelTextOffsetVariables,
            ..TwoLabelTextOffsetVariables,
            new BooleanWidgetConfigVariable(
                "AutoCloseOnChange",
                I18N.Translate("Widget.GearsetSwitcher.Config.AutoCloseOnChange.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.AutoCloseOnChange.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "EnableRoleScrolling",
                I18N.Translate("Widget.GearsetSwitcher.Config.EnableRoleScrolling.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.EnableRoleScrolling.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "PopupHeaderIconType",
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupHeaderIconType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupHeaderIconType.Description"),
                "Default",
                iconTypeChoices
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "PopupButtonIconType",
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupButtonIconType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupButtonIconType.Description"),
                "Default",
                iconTypeChoices
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "HeaderIconYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.HeaderIconYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.HeaderIconYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "ButtonIconYOffset",
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonIconYOffset.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonIconYOffset.Description"),
                1,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowRoleNames",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRoleNames.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRoleNames.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowWarningIcon",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowWarningIcon.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowWarningIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowExperienceBar",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowExperienceBar.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowExperienceBar.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowButtonItemLevel",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowButtonItemLevel.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowButtonItemLevel.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowCurrentJobGradient",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowCurrentJobGradient.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowCurrentJobGradient.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "GearsetButtonBackgroundType",
                I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Description"),
                "GradientV",
                new() {
                    { "None", I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Option.None") }, {
                        "GradientV",
                        I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Option.GradientV")
                    }, {
                        "GradientVI",
                        I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Option.GradientVI")
                    }, {
                        "GradientH",
                        I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Option.GradientH")
                    }, {
                        "GradientHI",
                        I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Option.GradientHI")
                    }, {
                        "Plain",
                        I18N.Translate("Widget.GearsetSwitcher.Config.GearsetButtonBackgroundType.Option.Plain")
                    },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },

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
            ) { Category = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role) },
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
            ) { Category = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role) },
            new IntegerWidgetConfigVariable(
                $"{name}RoleSortIndex",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleSortIndex.Name",        role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleSortIndex.Description", role),
                sortIndex,
                0,
                10
            ) { Category = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role) },
            new IntegerWidgetConfigVariable(
                $"{name}MaxItems",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleMaxItems.Name",        role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleMaxItems.Description", role),
                maxItems,
                1,
                10
            ) { Category = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role) },
        ];
    }
}
