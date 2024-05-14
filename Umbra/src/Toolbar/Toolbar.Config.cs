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

using Umbra.Common;

namespace Umbra;

internal partial class Toolbar
{
    [ConfigVariable("Toolbar.Enabled", "ToolbarSettings")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.IsTopAligned", "ToolbarSettings")]
    public static bool IsTopAligned { get; set; } = false;

    [ConfigVariable("Toolbar.IsStretched", "ToolbarSettings")]
    public static bool IsStretched { get; set; } = true;

    [ConfigVariable("Toolbar.IsAutoHideEnabled", "ToolbarSettings")]
    public static bool IsAutoHideEnabled { get; set; } = false;

    [ConfigVariable("Toolbar.EnableShadow", "ToolbarSettings")]
    public static bool EnableShadow { get; set; } = true;

    [ConfigVariable("Toolbar.ItemSpacing", "ToolbarSettings", "ToolbarCustomization", min: 1, max: 32)]
    private static int ItemSpacing { get; set; } = 6;

    [ConfigVariable("Toolbar.MarginLeft", "ToolbarSettings", "ToolbarCustomization", min: -1, max: 16384)]
    private static int ToolbarLeftMargin { get; set; } = 0;

    [ConfigVariable("Toolbar.MarginRight", "ToolbarSettings", "ToolbarCustomization", min: -1, max: 16384)]
    private static int ToolbarRightMargin { get; set; } = 0;

    [ConfigVariable("Toolbar.YOffset", "ToolbarSettings", "ToolbarCustomization", min: -16384, max: 16384)]
    public static int YOffset { get; set; } = 0;
}
