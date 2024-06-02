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

internal partial class WidgetConfigWindow
{
    private static Stylesheet WidgetConfigWindowStylesheet { get; }= new([
        new(
            ".widget-config-window",
            new() {
                Flow = Flow.Vertical,
            }
        ),
        new(
            ".widget-config-list--wrapper",
            new() {
                Flow                      = Flow.Vertical,
                ScrollbarTrackColor       = new(0),
                ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
            }
        ),
        new(
            ".widget-config-list",
            new() {
                Flow    = Flow.Vertical,
                Gap     = 15,
                Padding = new(15),
            }
        ),
        new(
            ".widget-config-footer",
            new() {
                Flow            = Flow.Horizontal,
                Gap             = 15,
                Padding         = new(0, 15),
                BackgroundColor = new("Window.BackgroundLight"),
            }
        ),
        new(
            ".widget-config-footer--buttons",
            new() {
                Anchor = Anchor.MiddleRight,
                Gap    = 15,
            }
        ),
    ]);
}
