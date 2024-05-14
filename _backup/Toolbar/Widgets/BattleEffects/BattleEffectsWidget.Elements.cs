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

using Dalamud.Interface;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.MainMenu;

internal sealed partial class BattleEffectsWidget
{
    public Element Element { get; } = new(
        id: "BattleEffectsWidget",
        size: new(28, 28),
        anchor: Anchor.MiddleRight,
        sortIndex: 2,
        children: [
            new BackgroundElement(color: Theme.Color(ThemeColor.BackgroundDark), edgeColor: Theme.Color(ThemeColor.BorderDark), edgeThickness: 1, rounding: 4),
            new BorderElement(color: Theme.Color(ThemeColor.Border), rounding: 3, padding: new(1)),
            new(
                id: "Icon",
                anchor: Anchor.None,
                text: FontAwesomeIcon.WandSparkles.ToIconString(),
                style: new() {
                    Font         = Font.FontAwesome,
                    TextAlign    = Anchor.MiddleCenter,
                    TextColor    = Theme.Color(ThemeColor.Text),
                    OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                    OutlineWidth = 1,
                    TextOffset   = new(0, -1)
                }
            ),
        ]
    );
}
