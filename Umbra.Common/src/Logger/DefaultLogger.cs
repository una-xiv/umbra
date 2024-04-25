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

using System;
using System.Diagnostics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace Umbra.Common;

public sealed class DefaultLogTarget(IPluginLog pluginLog, IChatGui chatGui) : ILogTarget
{
    public void Log(LogLevel level, object? message)
    {
        if (null == message) return;

        LogToChat(level, message);

        switch (level)
        {
            case LogLevel.Debug:
                pluginLog.Debug($"{message}");
                break;
            case LogLevel.Info:
                pluginLog.Info($"{message}");
                break;
            case LogLevel.Warning:
                pluginLog.Warning($"{message}");
                break;
            case LogLevel.Error:
                pluginLog.Error($"{message}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    [Conditional("DEBUG")]
    private void LogToChat(LogLevel level, object? message)
    {
        switch (level) {
            case LogLevel.Info:
                chatGui.Print($"{message}", "Umbra", 27);
                break;
            case LogLevel.Warning:
                chatGui.PrintError(new SeStringBuilder().AddIcon(BitmapFontIcon.Warning).AddText($"{message}").Build(), "Umbra", 28);
                break;
            case LogLevel.Error:
                chatGui.PrintError(new SeStringBuilder().AddIcon(BitmapFontIcon.NoCircle).AddText($"{message}").Build(), "Umbra", 28);
                break;
            case LogLevel.Debug:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
