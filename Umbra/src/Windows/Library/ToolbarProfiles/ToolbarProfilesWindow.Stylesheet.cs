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

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class ToolbarProfilesWindow
{
    private static Stylesheet ToolbarProfilesWindowStylesheet { get; } = new(
        [
            new(
                ".toolbar-profiles-window",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                ".toolbar-profiles-list--wrapper",
                new() {
                    Flow                      = Flow.Vertical,
                    ScrollbarTrackColor       = new(0),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            ),
            new(
                ".toolbar-profiles-body",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 15,
                    Padding = new(15),
                }
            ),
            new(
                ".toolbar-profiles-footer",
                new() {
                    Flow            = Flow.Horizontal,
                    Gap             = 15,
                    Padding         = new(0, 15),
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderWidth     = new() { Top = 1 },
                    BorderColor     = new() { Top = new("Window.Border") },
                    IsAntialiased   = false,
                }
            ),
            new(
                ".toolbar-profiles-footer--buttons",
                new() {
                    Anchor = Anchor.MiddleRight,
                    Gap    = 15,
                }
            ),
            new(
                ".toolbar-profiles-panel",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 10,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    Padding         = new(10),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".toolbar-profiles-panel-header",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".toolbar-profiles-panel--chevron",
                new() {
                    Font       = 2,
                    FontSize   = 16,
                    Padding    = new(2, 0),
                    Color      = new("Window.Text"),
                    Anchor     = Anchor.MiddleLeft,
                    TextAlign  = Anchor.MiddleLeft,
                    TextOffset = new(0, -1),
                    Opacity    = 0.65f,
                }
            ),
            new(
                ".toolbar-profiles-panel--chevron:hover",
                new() {
                    Color   = new("Window.TextLight"),
                    Opacity = 0.75f,
                }
            ),
            new(
                ".toolbar-profiles-panel--label",
                new() {
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    FontSize     = 16,
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    Padding      = new() { Right = 100 },
                }
            ),
            new(
                ".toolbar-profiles-panel--label:hover",
                new() {
                    Color = new("Window.TextLight"),
                }
            ),
            new(
                ".toolbar-profiles-list.in-panel",
                new() {
                    Flow    = Flow.Vertical,
                    Padding = new() { Top = 10 },
                    Gap     = 8,
                }
            ),
            new(
                ".job-profile-select",
                new() {
                    Flow    = Flow.Horizontal,
                    Size    = new(300, 0),
                    Gap     = 10,
                    Padding = new() { Left = 32 },
                }
            ),
            new(
                ".job-profile-select--label",
                new() {
                    Anchor       = Anchor.TopLeft,
                    Flow         = Flow.Horizontal,
                    Size         = new(100, 0),
                    Color        = new("Window.Text"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    FontSize     = 13,
                    TextOverflow = false,
                    WordWrap     = false,
                }
            )
        ]
    );
}
