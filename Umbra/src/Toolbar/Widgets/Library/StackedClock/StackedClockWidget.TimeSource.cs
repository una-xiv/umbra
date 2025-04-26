﻿using System;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace Umbra.Widgets.Library.StackedClock;

internal partial class StackedClockWidget
{
    /// <summary>
    /// Returns a <see cref="DateTime"/> object based on the configured time
    /// source.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private DateTime GetTime(string timeSource)
    {
        return timeSource switch {
            "LT" => DateTime.Now,
            "ST" => GetServerTime(),
            "ET" => GetEorzeaTime(),
            _    => throw new NotImplementedException($"Unknown time source: {timeSource}")
        };
    }

    /// <summary>
    /// Returns the current server time.
    /// </summary>
    /// <returns></returns>
    private static DateTime GetServerTime()
    {
        long serverTime = Framework.GetServerTime();
        long hours      = serverTime / 3600 % 24;
        long minutes    = serverTime / 60   % 60;
        long seconds    = serverTime        % 60;

        return new(1, 1, 1, (int)hours, (int)minutes, (int)seconds);
    }

    /// <summary>
    /// Returns the current Eorzea time.
    /// </summary>
    /// <returns></returns>
    private static unsafe DateTime GetEorzeaTime()
    {
        var fw = Framework.Instance();

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
