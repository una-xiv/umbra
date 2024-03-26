/* Umbra.Drawing | (c) 2024 by Una      ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Drawing is free software: you can       \/     \/             \/
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using Umbra.Common;

namespace Umbra.Drawing;

[Service]
public sealed partial class WindowManager : IDisposable
{
    private readonly Dictionary<Type, Window> _instanceList = [];
    private readonly ClipRectSolver _clipRectSolver;
    private readonly IKeyState _keyState;
    private readonly ICommandManager _commandManager;
    private readonly IChatGui _chatGui;

    private const uint SfxOpen  = 23u;
    private const uint SfxClose = 24u;

    private static readonly List<ChatCommand> ChatCommands = [];

    [ConfigVariable("WindowManager.PlaySoundEffects", "Window Settings", "Play sound effects when opening and closing windows", "Plays the default UI sound effects when opening and closing Umbra windows.")]
    private static bool PlaySoundEffects { get; set; } = true;

    [ConfigVariable("WindowManager.HonorEscapeKey", "Window Settings", "Allow pressing ESC to close focused windows", "Intercepts the escape key when an Umbra window is focused to close it.")]
    private static bool HonorEscapeKey { get; set; } = true;

    public WindowManager(
        ClipRectSolver clipRectSolver,
        IKeyState keyState,
        IGameInteropProvider interopProvider,
        ICommandManager commandManager,
        IChatGui chatGui
    )
    {
        _clipRectSolver     = clipRectSolver;
        _keyState           = keyState;
        _commandManager     = commandManager;
        _chatGui            = chatGui;
        _atkGlobalEventHook = CreateAtkUnitBaseReceiveGlobalEventHook(interopProvider);

        commandManager.AddHandler("/umbra", new CommandInfo(OnChatCommand));
    }

    public void Dispose()
    {
        foreach (var window in _instanceList.Values)
        {
            window.Close();
        }

        _atkGlobalEventHook.Dispose();
        _commandManager.RemoveHandler("/umbra");
    }

    /// <summary>
    /// Returns true if any window is currently focused.
    /// </summary>
    /// <returns></returns>
    public bool HasFocusedWindows()
    {
        foreach (var window in _instanceList.Values)
        {
            if (window.IsFocused) return true;
        }

        return false;
    }

    /// <summary>
    /// Creates a new window instance of the given type.
    /// </summary>
    /// <remarks>
    /// If a window of the given type already exists, it will be returned instead.
    /// </remarks>
    public T CreateWindow<T>() where T : Window, new()
    {
        return (T) CreateWindow(typeof(T));
    }

    public Window CreateWindow(Type type)
    {
        if (_instanceList.TryGetValue(type, out Window? value)) {
            return value;
        }

        if (Framework.InstantiateWithDependencies(type) is not Window instance)
        {
            throw new InvalidOperationException($"Failed to instantiate window {type.Name}. The instantiated object is not a Window type.");
        }

        instance.OnClose += () => DisposeWindow(type);

        _instanceList.Add(type, instance);

        if (PlaySoundEffects) UIModule.PlaySound(SfxOpen);

        return instance;
    }

    /// <summary>
    /// Disposes the window of the given type.
    /// </summary>
    public void DisposeWindow<T>() where T : Window
    {
        DisposeWindow(typeof(T));
    }

    /// <summary>
    /// Disposes the window of the given type.
    /// </summary>
    public void DisposeWindow(Type type)
    {
        if (_instanceList.TryGetValue(type, out Window? value)) {
            _instanceList.Remove(type);

            if (PlaySoundEffects) UIModule.PlaySound(SfxClose);

            if (value is IDisposable disposable)
                disposable.Dispose();
        }
    }

    [OnDraw]
    public void OnDraw()
    {
        foreach (var window in _instanceList.Values)
        {
            window.Draw(_clipRectSolver);

            if (HonorEscapeKey && _keyState[VirtualKey.ESCAPE] && window.IsFocused) {
                window.Close();
            }
        }
    }

    [WhenFrameworkCompiling]
    public static void GatherCommandInvokableTypes()
    {
        Framework.Assemblies.SelectMany(asm => asm.GetTypes())
            .Where(type => type.GetCustomAttributes<ChatCommandInvokableAttribute>().Any())
            .ToList()
            .ForEach(type => {
                // The type must extend from Window.
                if (!typeof(Window).IsAssignableFrom(type)) {
                    throw new InvalidOperationException($"Chat command invokable type {type.Name} must extend from Window.");
                }

                type.GetCustomAttributes<ChatCommandInvokableAttribute>().ToList().ForEach(attr => {
                    ChatCommands.Add(new ChatCommand(type, attr.Command, attr.Description ?? ""));
                });
            });
    }

    private readonly struct ChatCommand(Type type, string name, string description)
    {
        public Type Type { get; } = type;
        public string Name { get; } = name;
        public string Description { get; } = description;
    }

    private void OnChatCommand(string cmd, string subCommand)
    {
        if (cmd != "/umbra") return;

        var args = subCommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0) {
            SeStringBuilder builder = new();

            _chatGui.Print(builder.AddText("Available commands:").Build(), "Umbra", 28);

            foreach (var command in ChatCommands) {
                builder = new();
                builder.AddText("   ")
                    .AddIcon(BitmapFontIcon.GreenDot)
                    .AddUiForeground(24).AddUiGlow(7).AddText(command.Name).AddUiGlowOff().AddUiForegroundOff();

                if (command.Description.Length > 0) {
                    builder.AddItalicsOn().AddUiForeground(4).AddText($" - {command.Description}").AddUiForegroundOff().AddItalicsOff();
                }

                _chatGui.Print(builder.Build());
            }

            return;
        }

        var c = ChatCommands.FirstOrDefault(c => c.Name == args[0]);
        if (c.Type != null) {
            CreateWindow(c.Type);
            return;
        }

        _chatGui.PrintError("Unknown command.", "Umbra", 28);
        OnChatCommand("/umbra", "");
    }
}
