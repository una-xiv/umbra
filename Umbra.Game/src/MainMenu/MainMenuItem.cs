using System;
using Dalamud.Memory;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Misc;
using Umbra.Common;

namespace Umbra.Game;

public sealed class MainMenuItem : IDisposable
{
    public string            Name           { get; set; }
    public string            ShortKey       { get; set; } = string.Empty;
    public MainMenuItemType  Type           { get; set; }
    public string?           ChatCommand    { get; set; }
    public Action?           Callback       { get; set; }
    public uint?             CommandId      { get; set; }
    public short             SortIndex      { get; set; }
    public bool              IsDisabled     { get; set; }
    public object?           Icon           { get; set; }
    public uint?             IconColor      { get; set; }
    public string?           MetadataKey    { get; set; }
    public string?           ItemGroupId    { get; set; }
    public string?           ItemGroupLabel { get; set; }
    public MainMenuCategory? Category       { get; set; }

    public Action? OnUpdate;
    public Action? OnDisposed;

    public string Id => $"Item-{Crc32.Get(Category?.Category.ToString() ?? "X").ToString()}-{Type}-{Crc32.Get(Name)}";

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
        Name = agentHud->GetMainCommandString(commandId).ExtractText();

        if (Name.Contains('[')) {
            var tmp = Name.Split('[');
            Name     = tmp[0].Trim();
            ShortKey = $"[{tmp[1].Trim()}";
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
                if (string.IsNullOrEmpty(ChatCommand)) return;

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

        if (CommandId == null) return;

        IsDisabled = false
            == (
                agentHud->IsMainCommandEnabled(CommandId.Value) && uiModule->IsMainCommandUnlocked(CommandId.Value)
            );

        if (CommandId == 36) {
            // Add cooldown time for "Return".
            ShortKey = Framework.Service<IPlayer>().GetActionCooldownString(ActionType.GeneralAction, 8);
        }

        OnUpdate?.Invoke();
    }
}
