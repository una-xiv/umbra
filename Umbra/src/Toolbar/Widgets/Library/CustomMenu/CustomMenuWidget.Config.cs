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

internal sealed partial class CustomMenuWidget
{
    private const int MaxButtons = 16;

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        List<IWidgetConfigVariable> buttonVariables = [];

        for (var i = 0; i < MaxButtons; i++) {
            buttonVariables.AddRange(CreateButtonVariables(i));
        }

        return [
            ..ToolbarWidgetVariables,
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..buttonVariables
        ];
    }

    private static IEnumerable<IWidgetConfigVariable> ToolbarWidgetVariables => [
        new StringWidgetConfigVariable(
            "Label",
            I18N.Translate("Widget.CustomMenu.Config.Label.Name"),
            I18N.Translate("Widget.CustomMenu.Config.Label.Description"),
            "My Menu",
            32
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new StringWidgetConfigVariable(
            "Tooltip",
            I18N.Translate("Widget.CustomMenu.Config.Tooltip.Name"),
            I18N.Translate("Widget.CustomMenu.Config.Tooltip.Description"),
            ""
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new BooleanWidgetConfigVariable(
            "HideLabel",
            I18N.Translate("Widget.CustomMenu.Config.HideLabel.Name"),
            I18N.Translate("Widget.CustomMenu.Config.HideLabel.Description"),
            false
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "LeftIconId",
            I18N.Translate("Widget.CustomMenu.Config.LeftIconId.Name"),
            I18N.Translate("Widget.CustomMenu.Config.LeftIconId.Description"),
            0,
            0
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "RightIconId",
            I18N.Translate("Widget.CustomMenu.Config.RightIconId.Name"),
            I18N.Translate("Widget.CustomMenu.Config.RightIconId.Description"),
            0,
            0
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new BooleanWidgetConfigVariable(
            "DesaturateMenuIcons",
            I18N.Translate("Widget.CustomMenu.Config.DesaturateMenuIcons.Name"),
            I18N.Translate("Widget.CustomMenu.Config.DesaturateMenuIcons.Description"),
            false
        ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        new BooleanWidgetConfigVariable(
            "InverseOrder",
            I18N.Translate("Widget.CustomMenu.Config.InverseOrder.Name"),
            I18N.Translate("Widget.CustomMenu.Config.InverseOrder.Description"),
            false
        ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        new BooleanWidgetConfigVariable(
            "CloseOnClick",
            I18N.Translate("Widget.CustomMenu.Config.CloseOnClick.Name"),
            I18N.Translate("Widget.CustomMenu.Config.CloseOnClick.Description"),
            true
        ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
    ];

    private static IEnumerable<IWidgetConfigVariable> CreateButtonVariables(int buttonIndex)
    {
        return [
            new SelectWidgetConfigVariable(
                $"ButtonMode_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Description"),
                "Command",
                new() {
                    { "Command", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.Command") },
                    { "URL", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.URL") },
                    { "Item", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.Item") },
                    { "Separator", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.Separator") }
                }
            ) { Category = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) },
            new StringWidgetConfigVariable(
                $"ButtonLabel_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonLabel.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonLabel.Description"),
                "",
                32
            ) { Category = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) },
            new StringWidgetConfigVariable(
                $"ButtonAltLabel_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonAltLabel.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonAltLabel.Description"),
                "",
                32
            ) { Category = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) },
            new IntegerWidgetConfigVariable(
                $"ButtonIconId_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonIconId.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonIconId.Description"),
                0
            ) { Category = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) },
            new StringWidgetConfigVariable(
                $"ButtonCommand_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonCommand.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonCommand.Description"),
                ""
            ) { Category = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) },
            new SelectWidgetConfigVariable(
                $"ButtonItemUsage_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Description"),
                "HqBeforeNq",
                new() {
                    { "HqBeforeNq", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.HqBeforeNq") },
                    { "NqBeforeHq", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.NqBeforeHq") },
                    { "HqOnly", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.HqOnly") },
                    { "NqOnly", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.NqOnly") }
                }
            ) { Category = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) },
        ];
    }
}
