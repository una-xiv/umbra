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

using Umbra.Style;
using Una.Drawing;

namespace Umbra.Widgets;

public partial class DtrBarWidget
{
    public override WidgetPopup? Popup { get; } = null;

    public override Node Node { get; } = new() {
        Stylesheet = WidgetStyles.DefaultWidgetStylesheet,
        ClassList  = ["dtr-bar"],
        Style = new() {
            Anchor = Anchor.MiddleLeft,
            Flow   = Flow.Horizontal,
            Size   = new(0, 28),
            Gap    = Toolbar.ItemSpacing,
        }
    };
}
