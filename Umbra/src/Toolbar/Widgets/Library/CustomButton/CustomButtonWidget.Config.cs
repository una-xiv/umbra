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

internal partial class CustomButtonWidget
{
    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable(
                "Label",
                I18N.Translate("Widget.CustomButton.Config.Label.Name"),
                I18N.Translate("Widget.CustomButton.Config.Label.Description"),
                "My button",
                32
            ),
            new StringWidgetConfigVariable(
                "Command",
                I18N.Translate("Widget.CustomButton.Config.Command.Name"),
                I18N.Translate("Widget.CustomButton.Config.Command.Description"),
                "/echo Hello, world!"
            ),
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.CustomButton.Config.Decorate.Name"),
                I18N.Translate("Widget.CustomButton.Config.Decorate.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "DesaturateIcon",
                I18N.Translate("Widget.CustomButton.Config.DesaturateIcon.Name"),
                I18N.Translate("Widget.CustomButton.Config.DesaturateIcon.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "LeftIconId",
                I18N.Translate("Widget.CustomButton.Config.LeftIconId.Name"),
                I18N.Translate("Widget.CustomButton.Config.LeftIconId.Description"),
                0,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "RightIconId",
                I18N.Translate("Widget.CustomButton.Config.RightIconId.Name"),
                I18N.Translate("Widget.CustomButton.Config.RightIconId.Description"),
                0,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.CustomButton.Config.TextYOffset.Name"),
                I18N.Translate("Widget.CustomButton.Config.TextYOffset.Description"),
                -1,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.CustomButton.Config.IconYOffset.Name"),
                I18N.Translate("Widget.CustomButton.Config.IconYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
