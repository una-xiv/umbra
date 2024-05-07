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
using Dalamud.Game.Text;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Time;

internal abstract class BaseTimeWidget : IToolbarWidget
{
    /// <inheritdoc/>
    public virtual Element Element => null!;

    /// <summary>
    /// Whether this widget is currently visible.
    /// </summary>
    protected virtual bool IsVisible => true;

    /// <summary>
    /// Returns the type of time this widget displays.
    /// </summary>
    protected virtual TimeType Type => TimeType.EorzeaTime;

    /// <summary>
    /// Used to determine if the time should be displayed in 24-hour format.
    /// </summary>
    protected virtual bool Use24HourFormat => false;

    /// <summary>
    /// Whether to show seconds in the time display.
    /// </summary>
    protected virtual bool ShowSeconds => false;

    /// <summary>
    /// The suffix to display for AM times.
    /// </summary>
    protected virtual string AmSuffix => "AM";

    /// <summary>
    /// The suffix to display for PM times.
    /// </summary>
    protected virtual string PmSuffix => "PM";

    /// <summary>
    /// Returns a DateTime object representing the current time that should be
    /// displayed by this widget.
    /// </summary>
    protected abstract DateTime GetTime();

    public void OnUpdate() { }

    public void OnDraw()
    {
        Element.IsVisible = IsVisible;
        if (!IsVisible) return;

        Element prefix = Element.Get("Prefix");
        Element time   = Element.Get("Time");
        Element suffix = Element.Get("Suffix");

        string timeFormat = (Use24HourFormat ? "HH:mm" : "h:mm") + (ShowSeconds ? ":ss" : "");

        prefix.Text = GetPrefixIcon();
        time.Text   = GetTime().ToString(timeFormat);

        if (Use24HourFormat) {
            suffix.IsVisible = false;
            time.Padding     = new(right: 4);
        } else {
            time.Padding     = new();
            suffix.IsVisible = true;
            suffix.Text      = GetTime().Hour < 12 ? AmSuffix : PmSuffix;
        }
    }

    protected static Element CreateClock(string id, int defaultSortIndex)
    {
        return new(
            id: id,
            flow: Flow.Horizontal,
            anchor: Anchor.MiddleRight,
            sortIndex: defaultSortIndex,
            size: new(0, 28),
            gap: 4,
            children: [
                new BackgroundElement(color: Theme.Color(ThemeColor.BackgroundDark), rounding: 4),
                new BorderElement(color: Theme.Color(ThemeColor.Border), rounding: 3, padding: new(1)),
                new(
                    "Prefix",
                    text: "",
                    size: new(0, 28),
                    padding: new(left: 8, right: -4),
                    style: new() {
                        Font         = Font.Axis,
                        TextAlign    = Anchor.MiddleLeft,
                        TextOffset   = new(0, -1),
                        TextColor    = Theme.Color(ThemeColor.TextMuted),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                    }
                ),
                new(
                    "Time",
                    text: "00:00",
                    size: new(0, 28),
                    style: new() {
                        Font         = Font.Monospace,
                        TextAlign    = Anchor.MiddleLeft,
                        TextOffset   = new(0, -1),
                        TextColor    = Theme.Color(ThemeColor.Text),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                    }
                ),
                new(
                    "Suffix",
                    text: "",
                    size: new(0, 28),
                    padding: new(right: 4),
                    style: new() {
                        Font         = Font.Axis,
                        TextAlign    = Anchor.MiddleLeft,
                        TextOffset   = new(0, -1),
                        TextColor    = Theme.Color(ThemeColor.TextMuted),
                        OutlineColor = Theme.Color(ThemeColor.TextOutline),
                        OutlineWidth = 1,
                    }
                ),
            ]
        );
    }

    private string GetPrefixIcon()
    {
        return Framework.DalamudPlugin.UiLanguage switch {
            "de" => Type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeDe.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeDe.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeDe.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            },
            "fr" => Type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeFr.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeFr.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeFr.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            },
            "jp" => Type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeJa.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeJa.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeJa.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            },
            _ => Type switch {
                TimeType.EorzeaTime => SeIconChar.EorzeaTimeEn.ToIconString(),
                TimeType.LocalTime  => SeIconChar.LocalTimeEn.ToIconString(),
                TimeType.ServerTime => SeIconChar.ServerTimeEn.ToIconString(),
                _                   => throw new ArgumentOutOfRangeException()
            }
        };
    }
}
