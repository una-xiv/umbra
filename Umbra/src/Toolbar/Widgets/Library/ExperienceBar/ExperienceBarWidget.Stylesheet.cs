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
                    Size            = new(150, SafeHeight),
                    BackgroundColor = new("Widget.Background"),
                    StrokeColor     = new("Widget.Border"),
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    BorderRadius    = 5,
                    IsAntialiased   = false,
                    Padding         = new(0, 4),
                }
            ),
            new(
                ".experience-bar:ghost",
                new() {
                    BackgroundColor = new(0),
                    StrokeColor     = new(0),
                }
            ),
            new(
                ".sanctuary-icon",
                new() {
                    Font         = 2,
                    FontSize     = 14,
                    Color        = new("Widget.Text"),
                    Padding      = new(0, 4),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, SafeHeight),
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                }
            ),
            new(
                ".sync-icon",
                new() {
                    Font         = 1,
                    FontSize     = 14,
                    Color        = new("Widget.Text"),
                    Padding      = new(0, 0, 0, 3),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, SafeHeight),
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, 3),
                }
            ),
            new(
                ".label",
                new() {
                    FontSize     = 13,
                    Color        = new("Widget.Text"),
                    OutlineColor = new("Widget.TextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, SafeHeight),
                    TextOverflow = false,
                    WordWrap     = false,
                    Padding      = new(0, 4),
                }
            ),
            new(
                ".label.left",
                new() {
                    Anchor    = Anchor.TopLeft,
                    TextAlign = Anchor.MiddleLeft,
                }
            ),
            new(
                ".label.right",
                new() {
                    Anchor    = Anchor.TopRight,
                    TextAlign = Anchor.MiddleRight,
                }
            ),
            new(
                ".bar",
                new() {
                    Size          = new(50, SafeHeight - 8),
                    BorderRadius  = 4,
                    IsAntialiased = false,
                }
            ),
            new(
                ".bar.normal",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    BackgroundColor = new("Misc.ExperienceBar"),
                }
            ),
            new(
                ".bar.rested",
                new() {
                    Anchor          = Anchor.MiddleLeft,
                    BackgroundColor = new("Misc.ExperienceBarRested"),
                }
            )
        ]
    );
}
