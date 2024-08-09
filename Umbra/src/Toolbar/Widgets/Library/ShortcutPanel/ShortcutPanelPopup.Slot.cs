using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using Una.Drawing;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup
{
    private void ClearSlot(Node slotNode, byte categoryId, int slotId)
    {
        slotNode.Tooltip                                               = null;
        slotNode.QuerySelector(".slot-button--icon")!.Style.IconId     = null;
        slotNode.QuerySelector(".slot-button--sub-icon")!.Style.IconId = null;
        slotNode.QuerySelector(".slot-button--count")!.NodeValue       = null;

        slotNode.TagsList.Add($"empty-{(ShowEmptySlots ? "visible" : "hidden")}");
        slotNode.TagsList.Remove($"empty-{(ShowEmptySlots ? "hidden" : "visible")}");
        slotNode.TagsList.Remove("blocked");

        AssignAction(categoryId, slotId, 0u, null);
        AssignShortcut(categoryId, slotId, null);
    }

    private void SetInventoryItemSlot(Node slotNode, byte categoryId, int slotId, uint itemId)
    {
        if (itemId == 0u) return;

        var item = DataManager.GetExcelSheet<Item>()!.GetRow(itemId);
        if (item == null) return;

        var count = Player.GetItemCount(itemId);

        if (count == 0) {
            slotNode.TagsList.Add("blocked");
        } else {
            slotNode.TagsList.Remove("blocked");
        }

        SetSlotState(slotNode, item.Icon, item.Name.ToDalamudString().TextValue, null, count);
        AssignAction(categoryId, slotId, itemId, InvokeInventoryItem);
    }

    private unsafe void SetInventoryKeyItemSlot(Node slotNode, byte categoryId, int slotId, uint itemId)
    {
        if (itemId == 0u) return;

        var item = DataManager.GetExcelSheet<EventItem>()!.GetRow(itemId);
        if (item == null) return;

        InventoryContainer* container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.KeyItems);
        if (container == null) return;
        bool found = false;

        for (var i = 0; i < container->Size; i++) {
            if (container->GetInventorySlot(i)->ItemId == itemId) {
                found = true;
                break;
            }
        }

        if (!found) {
            slotNode.TagsList.Add("blocked");
        } else {
            slotNode.TagsList.Remove("blocked");
        }

        SetSlotState(slotNode, item.Icon, TextDecoder.ProcessNoun("EventItem", item.RowId));
        AssignAction(categoryId, slotId, itemId, found ? InvokeInventoryKeyItem : null);
    }

    private unsafe void SetEmoteSlot(Node slotNode, byte categoryId, int slotId, uint emoteId)
    {
        if (emoteId == 0u) return;

        if (!UIState.Instance()->IsEmoteUnlocked((ushort)emoteId)) return;

        var emote = DataManager.GetExcelSheet<Emote>()!.GetRow(emoteId);
        if (emote == null) return;

        SetSlotState(slotNode, emote.Icon, emote.Name.ToDalamudString().TextValue);
        AssignAction(categoryId, slotId, emoteId, InvokeEmote);
    }

    private unsafe void SetMountSlot(Node slotNode, byte categoryId, int slotId, uint mountId)
    {
        if (mountId == 0u) return;

        if (!PlayerState.Instance()->IsMountUnlocked((ushort)mountId)) return;

        var mount = DataManager.GetExcelSheet<Mount>()!.GetRow(mountId);
        if (mount == null) return;

        SetSlotState(slotNode, mount.Icon, TextDecoder.ProcessNoun("Mount", mountId));
        AssignAction(categoryId, slotId, mountId, InvokeMount);
    }

    private unsafe void SetMinionSlot(Node slotNode, byte categoryId, int slotId, uint minionId)
    {
        if (minionId == 0u) return;

        if (!UIState.Instance()->IsCompanionUnlocked((ushort)minionId)) return;

        var minion = DataManager.GetExcelSheet<Companion>()!.GetRow(minionId);
        if (minion == null) return;

        SetSlotState(slotNode, minion.Icon, TextDecoder.ProcessNoun("Companion", minionId));
        AssignAction(categoryId, slotId, minionId, InvokeMinion);
    }

    private unsafe void SetIndividualMacroSlot(Node slotNode, byte categoryId, int slotId, uint macroId)
    {
        var macro = GetMacro(0, macroId);
        if (macro == null) return;

        SetSlotState(slotNode, MacroIconProvider.GetIconIdForMacro(0, macroId), macro->Name.ToString());
        AssignAction(categoryId, slotId, macroId, InvokeIndividualMacro);
    }

    private unsafe void SetSharedMacroSlot(Node slotNode, byte categoryId, int slotId, uint macroId)
    {
        var macro = GetMacro(1, macroId);
        if (macro == null) return;

        SetSlotState(slotNode, MacroIconProvider.GetIconIdForMacro(1, macroId), macro->Name.ToString());
        AssignAction(categoryId, slotId, macroId, InvokeSharedMacro);
    }

    private unsafe void SetMainCommandSlot(Node slotNode, byte categoryId, int slotId, uint commandId)
    {
        if (!UIModule.Instance()->IsMainCommandUnlocked(commandId)) return;

        var command = DataManager.GetExcelSheet<MainCommand>()!.GetRow(commandId);
        if (command == null) return;

        SetSlotState(slotNode, (uint)command.Icon, command.Name.ToDalamudString().TextValue);
        AssignAction(categoryId, slotId, commandId, InvokeMainCommand);
    }

    private void SetGeneralActionSlot(Node slotNode, byte categoryId, int slotId, uint actionId)
    {
        if (!Player.IsGeneralActionUnlocked(actionId)) return;

        var action = DataManager.GetExcelSheet<GeneralAction>()!.GetRow(actionId);
        if (action == null) return;

        SetSlotState(slotNode, (uint)action.Icon, action.Name.ToDalamudString().TextValue);
        AssignAction(categoryId, slotId, actionId, InvokeGeneralCommand);
    }

    private void SetCraftingRecipeSlot(Node slotNode, byte categoryId, int slotId, uint recipeId)
    {
        if (recipeId == 0u) return;

        var recipe = DataManager.GetExcelSheet<Recipe>()!.GetRow(recipeId);
        var item   = recipe?.ItemResult.Value;
        if (item == null) return;

        string itemName  = item.Name.ToDalamudString().TextValue;
        string craftType = recipe!.CraftType.Value!.Name.ToDalamudString().TextValue;

        SetSlotState(slotNode, item.Icon, $"{craftType}: {itemName}", 66414u);
        AssignAction(categoryId, slotId, recipeId, InvokeCraftingRecipe);
    }

    private static void SetSlotState(Node slotNode, uint iconId, string tooltip, uint? subIcon = null, int? num = null)
    {
        slotNode.QuerySelector(".slot-button--icon")!.Style.IconId     = iconId;
        slotNode.QuerySelector(".slot-button--sub-icon")!.Style.IconId = subIcon;
        slotNode.QuerySelector(".slot-button--count")!.NodeValue       = num?.ToString();
        slotNode.Tooltip                                               = tooltip;
    }
}
