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

internal partial class ExperienceBarWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DisplayAtMaxLevel",
                I18N.Translate("Widget.ExperienceBar.Config.DisplayAtMaxLevel.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.DisplayAtMaxLevel.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowExperience",
                I18N.Translate("Widget.ExperienceBar.Config.ShowExperience.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowExperience.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowLevel",
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevel.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevel.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowSanctuaryIcon",
                I18N.Translate("Widget.ExperienceBar.Config.ShowSanctuaryIcon.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowSanctuaryIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowLevelSyncIcon",
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevelSyncIcon.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevelSyncIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowPreciseExperience",
                I18N.Translate("Widget.ExperienceBar.Config.ShowPreciseExperience.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowPreciseExperience.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "WidgetWidth",
                I18N.Translate("Widget.ExperienceBar.Config.WidgetWidth.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.WidgetWidth.Description"),
                100,
                30,
                500
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffset.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "MoonYOffset",
                I18N.Translate("Widget.ExperienceBar.Config.MoonYOffset.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.MoonYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "SyncYOffset",
                I18N.Translate("Widget.ExperienceBar.Config.SyncYOffset.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.SyncYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
