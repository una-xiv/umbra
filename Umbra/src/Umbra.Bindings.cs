﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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
using System.Linq;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Windows;
using Umbra.Windows.Oobe;
using Umbra.Windows.Settings;
using Una.Drawing;

namespace Umbra;

[Service]
internal sealed class UmbraBindings : IDisposable
{
    [ConfigVariable("General.UiScale", "General", null, 50, 250)]
    public static int UiScale { get; set; } = 100;

    [ConfigVariable("IsFirstTimeStart")]
    public static bool IsFirstTimeStart { get; set; } = true;

    private readonly IChatGui        _chatGui;
    private readonly ICommandManager _commandManager;
    private readonly WindowManager   _windowManager;

    public UmbraBindings(IChatGui chatGui, ICommandManager commandManager, WindowManager windowManager)
    {
        _chatGui        = chatGui;
        _commandManager = commandManager;
        _windowManager  = windowManager;

        _commandManager.AddHandler(
            "/umbra",
            new(HandleUmbraCommand) {
                HelpMessage = "Opens the Umbra settings window.",
                ShowInHelp  = true,
            }
        );

        _commandManager.AddHandler(
            "/umbra-toggle",
            new(HandleUmbraCommand) {
                HelpMessage = "Toggles a specific Umbra setting. Usage: /umbra-toggle <setting>. For a list of settings, use /umbra-toggle without arguments.",
                ShowInHelp  = true,
            }
        );

        Framework.DalamudPlugin.UiBuilder.OpenConfigUi += () => _windowManager.Present("UmbraSettings", new SettingsWindow());
        Framework.DalamudPlugin.UiBuilder.OpenMainUi   += () => _windowManager.Present("UmbraSettings", new SettingsWindow());

        Node.ScaleFactor = 1.0f;

        #if DEBUG
        //_windowManager.Present("UmbraSettings", new SettingsWindow());
        #endif

        if (IsFirstTimeStart) {
            _windowManager.Present("OOBE", new OobeWindow());
        }
    }

    public void Dispose()
    {
        _commandManager.RemoveHandler("/umbra");
        _commandManager.RemoveHandler("/umbra-toggle");
    }

    [OnTick(interval: 60)]
    private void OnTick()
    {
        Node.ScaleFactor = (float)Math.Round(Math.Clamp(UiScale / 100f, 0.5f, 3.0f), 2);
    }

    private void HandleUmbraCommand(string command, string args)
    {
        switch (command.ToLower()) {
            case "/umbra":
                _windowManager.Present("UmbraSettings", new SettingsWindow());
                break;
            case "/umbra-toggle":
                string arg  = args.Trim();

                if (arg == string.Empty) {
                    ShowCvarToggleHelp();
                    return;
                }

                Cvar?  cvar = ConfigManager.GetCvar(arg);

                if (cvar is not { Default: bool }) {
                    _chatGui.PrintError($"Invalid setting: \"{arg}\".");
                    return;
                }

                ConfigManager.Set(cvar.Id, !(bool)cvar.Value!);
                break;
        }
    }

    private void ShowCvarToggleHelp()
    {
        SeStringBuilder builder = new();

        foreach (string category in ConfigManager.GetCategories()) {
            if (!I18N.Has($"CVAR.Group.{category}")) continue;

            var cvars = ConfigManager.GetVariablesFromCategory(category).Where(c => c.Default is bool && I18N.Has($"CVAR.{c.Id}.Name")).ToList();
            if (cvars.Count == 0) continue;

            builder.AddText("Category: ").AddUiForeground(I18N.Translate($"CVAR.Group.{category}"), 42).AddText("\n");

            foreach (var cvar in cvars) {
                builder.AddText("    \"").AddUiForeground(cvar.Id, 32).AddText("\" - ").AddUiForeground(I18N.Translate($"CVAR.{cvar.Id}.Name"), 4).AddText("\n");
            }
        }

        builder.AddText("Usage: ").AddUiForeground("/umbra-toggle <setting>", 32);
        _chatGui.Print(builder.Build());
    }
}
