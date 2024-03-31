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
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Interface;
using Umbra.Windows.ConfigWindow;

namespace Umbra;

[Service]
internal sealed class UmbraBindings : IDisposable
{
    private readonly ICommandManager _commandManager;
    private readonly WindowManager   _windowManager;

    public UmbraBindings(ICommandManager commandManager, WindowManager windowManager)
    {
        _commandManager = commandManager;
        _windowManager  = windowManager;

        _commandManager.AddHandler(
            "/umbra",
            new(HandleUmbraCommand) {
                HelpMessage = "Opens the Umbra settings window.",
                ShowInHelp  = true,
            }
        );

        Framework.DalamudPlugin.UiBuilder.OpenConfigUi += () => _windowManager.CreateWindow<ConfigWindow>();
        Framework.DalamudPlugin.UiBuilder.OpenMainUi   += () => _windowManager.CreateWindow<ConfigWindow>();
    }

    public void Dispose()
    {
        _commandManager.RemoveHandler("/umbra");
    }

    private void HandleUmbraCommand(string command, string args)
    {
        switch (command.ToLower()) {
            case "/umbra":
                _windowManager.CreateWindow<ConfigWindow>();
                break;
        }
    }
}
