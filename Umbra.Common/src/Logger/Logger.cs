/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can        \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;

namespace Umbra.Common;

public static class Logger
{
    private static readonly List<ILogTarget> Targets = [];

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    public static void Debug(string? message)
    {
        Targets.ForEach(target => target.Log(LogLevel.Debug, message));
    }

    /// <summary>
    /// Logs an informative message.
    /// </summary>
    public static void Info(string? message)
    {
        Targets.ForEach(target => target.Log(LogLevel.Info, message));
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public static void Warning(string? message)
    {
        Targets.ForEach(target => target.Log(LogLevel.Warning, message));
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public static void Error(string? message)
    {
        Targets.ForEach(target => target.Log(LogLevel.Error, message));
    }

    /// <summary>
    /// Adds a logger implementation.
    /// </summary>
    /// <param name="logger"></param>
    internal static void AddLogTarget(ILogTarget logger)
    {
        Targets.Add(logger);
    }

    internal static void RemoveLoggerImpl(ILogTarget logger)
    {
        Targets.Remove(logger);
    }
}
