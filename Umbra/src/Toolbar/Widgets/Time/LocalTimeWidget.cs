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
internal class LocalTimeWidget : BaseTimeWidget
{
    public override Element Element { get; } = CreateClock("LocalTimeWidget", -2);

    [ConfigVariable("Toolbar.Widget.LocalTime.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.LocalTime.Use24HourFormat", "ToolbarSettings", "ClockSettings")]
    private static bool Use24HourFormatOption { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.LocalTime.ShowSeconds", "ToolbarSettings", "ClockSettings")]
    private static bool ShowSecondsOption { get; set; } = false;

    [ConfigVariable("Toolbar.Widget.LocalTime.AmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string AmSuffixOption { get; set; } = "am";

    [ConfigVariable("Toolbar.Widget.LocalTime.PmSuffix", "ToolbarSettings", "ClockSettings")]
    private static string PmSuffixOption { get; set; } = "pm";

    protected override TimeType Type            => TimeType.LocalTime;
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
