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
    [ConfigVariable("Toolbar.Enabled", "General", "Toolbar")]
    public static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.IsAutoHideEnabled", "General", "Toolbar")]
    public static bool IsAutoHideEnabled { get; set; } = false;

    [ConfigVariable("Toolbar.AutoHideDuringCutscenes", "General", "Toolbar")]
    private static bool AutoHideDuringCutscenes { get; set; } = false;

    [ConfigVariable("Toolbar.AutoHideDuringDuty", category: "General", subCategory: "Toolbar")]
    private static bool AutoHideDuringDuty { get; set; } = false;

    [ConfigVariable("Toolbar.AutoHideDuringPvp", category: "General", subCategory: "Toolbar")]
    private static bool AutoHideDuringPvp { get; set; } = false;

    [ConfigVariable("Toolbar.IsTopAligned", "General", "Toolbar")]
    public static bool IsTopAligned { get; set; } = false;

    [ConfigVariable("Toolbar.IsStretched", "General", "Toolbar")]
    public static bool IsStretched { get; set; } = true;

    [ConfigVariable("Toolbar.EnableShadow", "General", "Toolbar")]
    private static bool EnableShadow { get; set; } = true;

    [ConfigVariable("Toolbar.EnableInactiveColors", "General", "Toolbar")]
    private static bool EnableInactiveColors { get; set; } = false;

    [ConfigVariable("Toolbar.Height", "General", "Toolbar", min: 26, max: 64)]
    public static int Height { get; set; } = 32;

    [ConfigVariable("Toolbar.ItemSpacing", "General", "Toolbar", min: 0, max: 1000)]
    public static int ItemSpacing { get; set; } = 6;

    [ConfigVariable("Toolbar.MarginLeft", "General", "Toolbar", min: -16384, max: 16384)]
    private static int ToolbarLeftMargin { get; set; } = 0;

    [ConfigVariable("Toolbar.MarginRight", "General", "Toolbar", min: -16384, max: 16384)]
    private static int ToolbarRightMargin { get; set; } = 0;

    [ConfigVariable("Toolbar.YOffset", "General", "Toolbar", min: -16384, max: 16384)]
    private static int YOffset { get; set; } = 0;

    [ConfigVariable("Toolbar.AuxBar.Enabled")]
    public static bool AuxBarEnabled { get; set; } = false;

    [ConfigVariable("Toolbar.AuxBar.Decorate")]
    public static bool AuxBarDecorate { get; set; } = true;

    [ConfigVariable("Toolbar.AuxBar.XPos", min: -10000, max: 10000)]
    public static int AuxBarXPos { get; set; } = 0;

    [ConfigVariable("Toolbar.AuxBar.YPos", min: -10000, max: 10000)]
    public static int AuxBarYPos { get; set; } = 0;

    [ConfigVariable("Toolbar.AuxBar.XAlign", options: ["Left", "Center", "Right"])]
    public static string AuxBarXAlign { get; set; } = "Center";

    [ConfigVariable("Toolbar.AuxBar.EnableShadow")]
    public static bool AuxEnableShadow { get; set; } = true;
}
