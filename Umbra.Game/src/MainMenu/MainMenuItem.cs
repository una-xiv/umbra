/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Runtime.InteropServices;
using Dalamud.Game.Text;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;

namespace Umbra.Game;

public sealed class MainMenuItem : IDisposable
{
    public string           Name        { get; set; }
    public string           Shortkey    { get; } = string.Empty;
    public MainMenuItemType Type        { get; set; }
    public string?          ChatCommand { get; set; }
    public Action?          Callback    { get; set; }
    public uint?            CommandId   { get; set; }
    public short            SortIndex   { get; set; }
    public bool             IsDisabled  { get; set; }
    public object?          Icon        { get; set; }
    public uint?            IconColor   { get; set; }
    public Action?          OnUpdate;
    public Action?          OnDisposed;
    public string?          MetadataKey { get; set; }

    public string Id => $"{Type}-{SortIndex}-{Name.Replace(" ", "")}";

    public void Dispose()
    {
        OnDisposed?.Invoke();
    }

    public MainMenuItem(short sortIndex)
    {
        Type      = MainMenuItemType.Separator;
        Name      = "";
        SortIndex = sortIndex;
    }

    public MainMenuItem(string name, short sortIndex, string chatCommand)
    {
        Type        = MainMenuItemType.ChatCommand;
        Name        = name;
        SortIndex   = sortIndex;
        ChatCommand = chatCommand;
    }

    public MainMenuItem(string name, short sortIndex, Action callback)
    {
        Type      = MainMenuItemType.Callback;
        Name      = name;
        SortIndex = sortIndex;
        Callback  = callback;
    }

    public unsafe MainMenuItem(string name, short sortIndex, uint commandId)
    {
        Type      = MainMenuItemType.MainCommand;
        Name      = name;
        SortIndex = sortIndex;
        CommandId = commandId;

        AgentHUD* agentHud = AgentHUD.Instance();
        if (agentHud == null) return;

        ThreadSafety.AssertMainThread();
        Name = Marshal.PtrToStringUTF8((nint)agentHud->GetMainCommandString(commandId)) ?? name;

        if (Name.Contains('[')) {
            var tmp = Name.Split('[');
            Name     = tmp[0].Trim();
            Shortkey = tmp[1].Replace("]", "").Trim();
        }
    }

    public unsafe void Invoke()
    {
        switch (Type) {
            case MainMenuItemType.MainCommand:
                UIModule* uiModule = UIModule.Instance();
                if (uiModule == null) return;

                uiModule->ExecuteMainCommand(CommandId!.Value);
                break;
            case MainMenuItemType.ChatCommand:
                if (ChatCommand == null
                 || ChatCommand == "")
                    return;

                Framework.Service<IChatSender>().Send(ChatCommand);
                break;
            case MainMenuItemType.Callback:
                Callback?.Invoke();
                break;
        }
    }

    public unsafe void Update()
    {
        if (Type != MainMenuItemType.MainCommand) {
            OnUpdate?.Invoke();
            return;
        }

        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null) return;

        AgentHUD* agentHud = AgentHUD.Instance();
        if (agentHud == null) return;

        IsDisabled = false
         == (
                agentHud->IsMainCommandEnabled(CommandId!.Value) && uiModule->IsMainCommandUnlocked(CommandId!.Value)
            );

        OnUpdate?.Invoke();
    }
}
