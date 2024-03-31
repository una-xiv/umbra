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
        id: "BattleEffectsToolbarWidget",
        size: new(28, 28),
        anchor: Anchor.MiddleRight,
        sortIndex: int.MinValue + 2,
        children: [
            new BackgroundElement(color: 0xFF1A1A1A, edgeColor: 0xFF101010, edgeThickness: 1, rounding: 4),
            new BorderElement(color: 0xFF3F3F3F, rounding: 3, padding: new(1)),
            new(
                id: "Icon",
                anchor: Anchor.None,
                text: FontAwesomeIcon.WandSparkles.ToIconString(),
                style: new() {
                    Font         = Font.FontAwesome,
                    TextAlign    = Anchor.MiddleCenter,
                    TextColor    = 0xFFC0C0C0,
                    OutlineColor = 0xFF000000,
                    OutlineWidth = 1,
                    TextOffset   = new(0, -1)
                }
            ),
        ]
    );
}
