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

using System;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Time;

[Service]
internal class EorzeaTimeWidget : BaseTimeWidget
{
    public override Element Element { get; } = CreateClock("EorzeaTimeWidget", -1);

    [ConfigVariable("Toolbar.Widget.EorzeaTime.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.EorzeaTime.Use24HourFormat", "ToolbarSettings", "ClockSettings")]
    private static bool Use24HourFormatOption { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.EorzeaTime.ShowSeconds", "ToolbarSettings", "ClockSettings")]
    private static bool ShowSecondsOption { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.EorzeaTime.AmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string AmSuffixOption { get; set; } = "am";

    [ConfigVariable("Toolbar.Widget.EorzeaTime.PmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string PmSuffixOption { get; set; } = "pm";

    protected override TimeType Type            => TimeType.EorzeaTime;
    protected override bool     IsVisible       => Enabled;
    protected override bool     Use24HourFormat => Use24HourFormatOption;
    protected override bool     ShowSeconds     => ShowSecondsOption;
    protected override string   AmSuffix        => AmSuffixOption;
    protected override string   PmSuffix        => PmSuffixOption;

    /// <inheritdoc/>
    protected override unsafe DateTime GetTime()
    {
        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();

        if (fw == null) {
            return DateTime.MinValue;
        }

        long eorzeaTime = fw->ClientTime.EorzeaTime;
        long hours      = eorzeaTime / 3600 % 24;
        long minutes    = eorzeaTime / 60   % 60;
        long seconds    = eorzeaTime        % 60;

        return new(1, 1, 1, (int)hours, (int)minutes, (int)seconds);
    }
}
