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

using Una.Drawing;

namespace Umbra.Widgets;

internal partial class ExperienceBarWidget : ToolbarWidget
{
    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                ".experience-bar",
                new() {
                    Flow            = Flow.Horizontal,
                    Anchor          = Anchor.MiddleLeft,
                    Size            = new(100, SafeHeight),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 5,
                    IsAntialiased   = false,
                }
            ),
            new(
                ".label",
                new() {
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleCenter,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(100, SafeHeight),
                    TextOffset   = new(0, 1),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".bar",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    Size            = new(50, SafeHeight - 8),
                    Margin          = new(4),
                    BorderRadius    = 4,
                    BackgroundColor = new("Window.AccentColor"),
                    IsAntialiased   = false,
                }
            )
        ]
    );
}
