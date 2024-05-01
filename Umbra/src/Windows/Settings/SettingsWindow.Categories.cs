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

using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static string _currentCategory = "";
    private static int    _categorySortIndex;

    private static void CreateCategory(string id, string label)
    {
        // Set the first category as the current category.
        if (_currentCategory == "") _currentCategory = id;

        _categorySortIndex++;

        Element button = new(
            id: $"Button_{id}",
            size: new(200, 24),
            padding: new(0, -8, 0, 8),
            sortIndex: _categorySortIndex,
            children: [
                new BackgroundElement(color: Theme.Color(ThemeColor.BackgroundDark), rounding: 4),
                new(
                    id: "Label",
                    text: label,
                    anchor: Anchor.MiddleRight,
                    padding: new(right: 16),
                    style: new() {
                        Font         = Font.Axis,
                        TextColor    = Theme.Color(ThemeColor.TextMuted),
                        TextAlign    = Anchor.MiddleRight,
                        TextOffset   = new(0, -1),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                    }
                )
            ]
        );

        button.OnClick += () => { _currentCategory = id; };

        button.OnMouseEnter += () => {
            button.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundLight);
            button.Get("Label").Style.TextColor   = Theme.Color(ThemeColor.TextLight);
        };

        button.OnMouseLeave += () => {
            button.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
            button.Get("Label").Style.TextColor   = Theme.Color(ThemeColor.TextMuted);
        };

        BackgroundElement bl = button.Get<BackgroundElement>();
        Element           lb = button.Get("Label");

        button.OnBeforeCompute += () => {
            bool isActive = button.IsMouseOver || _currentCategory == id;

            bl.Color = isActive ? Theme.Color(ThemeColor.BackgroundLight) : Theme.Color(ThemeColor.BackgroundDark);
            lb.Style.TextColor = isActive ? Theme.Color(ThemeColor.TextLight) : Theme.Color(ThemeColor.TextMuted);

            lb.Style.OutlineColor =
                isActive ? Theme.Color(ThemeColor.TextOutlineLight) : Theme.Color(ThemeColor.TextOutline);
        };

        NavButtonsElement.AddChild(button);
    }
}
