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
internal class ServerTimeWidget : BaseTimeWidget
{
    public override Element Element { get; } = CreateClock("ServerTimeWidget", -3);

    [ConfigVariable("Toolbar.Widget.ServerTime.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.ServerTime.Use24HourFormat", "ToolbarSettings", "ClockSettings")]
    private static bool Use24HourFormatOption { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.ServerTime.ShowSeconds", "ToolbarSettings", "ClockSettings")]
    private static bool ShowSecondsOption { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.ServerTime.AmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string AmSuffixOption { get; set; } = "am";

    [ConfigVariable("Toolbar.Widget.ServerTime.PmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string PmSuffixOption { get; set; } = "pm";

    protected override TimeType Type            => TimeType.ServerTime;
    protected override bool     IsVisible       => Enabled;
    protected override bool     Use24HourFormat => Use24HourFormatOption;
    protected override bool     ShowSeconds     => ShowSecondsOption;
    protected override string   AmSuffix        => AmSuffixOption;
    protected override string   PmSuffix        => PmSuffixOption;

    /// <inheritdoc/>
    protected override DateTime GetTime()
    {
        return DateTime.Now;
    }
}
