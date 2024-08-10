using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup
{
    private readonly Dictionary<byte, Dictionary<int, (uint, Action<uint>)?>> _buttonActions = new();

    private void AssignAction(byte categoryId, int slotId, uint? itemId, Action<uint>? action)
    {
        if (!_buttonActions.TryGetValue(categoryId, out Dictionary<int, (uint, Action<uint>)?>? slots)) {
            slots                      = new();
            _buttonActions[categoryId] = slots;
        }

        slots[slotId] = itemId == null || action == null ? null : (itemId.Value, action);
    }

    private void InvokeAction(byte categoryId, int slotId)
    {
        if (!_buttonActions.TryGetValue(categoryId, out Dictionary<int, (uint, Action<uint>)?>? slots)) return;
        if (!slots.TryGetValue(slotId, out (uint, Action<uint>)? action)) return;
        if (null == action?.Item1) return;

        action.Value.Item2(action.Value.Item1);
        if (AutoCloseOnUse) Close();
    }

    private void InvokeInventoryItem(uint itemId)
    {
        if (Player.GetItemCount(itemId) == 0) {
            Logger.Info($"Item {itemId} is not in inventory.");
            return;
        }

        Framework.Service<IPlayer>().UseInventoryItem(itemId);
    }

    private unsafe void InvokeCollectionItem(uint itemId)
    {
        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(1);
        values[1].SetUInt(itemId);

        AgentModule.Instance()->GetAgentByInternalId(AgentId.McGuffin)->ReceiveEvent(result, values, 2, 0);
    }

    private unsafe void InvokeInventoryKeyItem(uint itemId)
    {
        EventItem? item = DataManager.GetExcelSheet<EventItem>()!.GetRow(itemId);
        if (item == null) return;

        ActionManager* am = ActionManager.Instance();

        if (am->GetActionStatus(ActionType.KeyItem, itemId) == 0) am->UseAction(ActionType.KeyItem, itemId);
    }

    private void InvokeEmote(uint emoteId)
    {
        Emote? emote = DataManager.GetExcelSheet<Emote>()!.GetRow(emoteId);
        if (emote?.TextCommand.Value == null) return;

        Framework.Service<IChatSender>().Send(emote.TextCommand.Value.Command.ToDalamudString().TextValue);
    }

    private unsafe void InvokeMount(uint mountId)
    {
        ActionManager* am = ActionManager.Instance();
        if (am == null || am->GetActionStatus(ActionType.Mount, mountId) != 0) return;

        am->UseAction(ActionType.Mount, mountId);
    }

    private unsafe void InvokeMinion(uint minionId)
    {
        ActionManager* am = ActionManager.Instance();
        if (am == null || am->GetActionStatus(ActionType.Companion, minionId) != 0) return;

        am->UseAction(ActionType.Companion, minionId);
    }

    private unsafe void InvokeMainCommand(uint commandId)
    {
        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null || !uiModule->IsMainCommandUnlocked(commandId)) return;

        uiModule->ExecuteMainCommand(commandId);
    }

    private unsafe void InvokeGeneralCommand(uint actionId)
    {
        ActionManager* am = ActionManager.Instance();

        if (!Framework.Service<IPlayer>().IsGeneralActionUnlocked(actionId)) return;
        if (am == null || am->GetActionStatus(ActionType.GeneralAction, actionId) != 0) return;

        Framework.Service<IPlayer>().UseGeneralAction(actionId);
    }

    private unsafe void InvokeIndividualMacro(uint macroId)
    {
        var macro = GetMacro(0, macroId);
        if (macro == null) return;

        RaptureShellModule* rsm = RaptureShellModule.Instance();
        if (rsm == null) return;

        rsm->ExecuteMacro(macro);
    }

    private unsafe void InvokeSharedMacro(uint macroId)
    {
        var macro = GetMacro(1, macroId);
        if (macro == null) return;

        RaptureShellModule* rsm = RaptureShellModule.Instance();
        if (rsm == null) return;

        rsm->ExecuteMacro(macro);
    }

    private unsafe void InvokeCraftingRecipe(uint recipeId)
    {
        // Opening a crafting recipe is only allowed if the player is not already crafting.
        if (Framework.Service<ICondition>()[ConditionFlag.Crafting]) return;

        AgentRecipeNote* arn = AgentRecipeNote.Instance();
        if (arn == null) return;

        arn->OpenRecipeByRecipeId(recipeId);
    }

    private unsafe RaptureMacroModule.Macro* GetMacro(byte isShared, uint macroId)
    {
        RaptureMacroModule* rmm = RaptureMacroModule.Instance();
        if (rmm == null) return null;

        RaptureMacroModule.Macro* macro = rmm->GetMacro(isShared, macroId);

        if (macro == null || !macro->IsNotEmpty()) {
            return null;
        }

        return macro;
    }
}
