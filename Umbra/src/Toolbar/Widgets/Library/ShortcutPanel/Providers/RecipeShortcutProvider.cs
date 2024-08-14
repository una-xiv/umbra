using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

[Service]
internal sealed class RecipeShortcutProvider(IDataManager dataManager, ICondition condition, IPlayer player) : AbstractShortcutProvider
{
    public override string ShortcutType => "CR"; // Crafting Recipe

    public override string PickerWindowTitle =>
        I18N.Translate("Widget.ShortcutPanel.PickerWindow.CraftingRecipe.Title");

    public override string ContextMenuEntryName =>
        I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickCraftingRecipe");

    public override uint? ContextMenuEntryIcon  => 22;
    public override int   ContextMenuEntryOrder => -500;

    /// <inheritdoc/>
    public override unsafe IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        PlayerState* ps = PlayerState.Instance();
        if (ps == null) return [];

        List<Recipe> recipes = dataManager.GetExcelSheet<Recipe>()!
            .Where(
                r => r.RowId > 0
                    && r.ItemResult.Value != null
                    && !string.IsNullOrEmpty(r.ItemResult.Value.Name.ToDalamudString().TextValue)
                    && (r.SecretRecipeBook.Row == 0 || ps->IsSecretRecipeBookUnlocked(r.SecretRecipeBook.Row))
                    && (r.Quest.Row == 0 || QuestManager.IsQuestComplete(r.Quest.Row))
                    && (r
                            .ItemResult.Value?.Name.ToString()
                            .Contains(searchFilter ?? "", StringComparison.OrdinalIgnoreCase)
                        ?? false)
            )
            .ToList();

        recipes.Sort(
            (a, b) => string.Compare(
                a.ItemResult.Value!.Name.ToDalamudString().TextValue.ToString(),
                b.ItemResult.Value!.Name.ToDalamudString().TextValue.ToString(),
                StringComparison.OrdinalIgnoreCase
            )
        );

        return recipes
            .Take(51)
            .Select(
                recipe => new Shortcut() {
                    Id   = recipe.RowId,
                    Name = recipe.ItemResult.Value!.Name.ToString(),
                    Description =
                        $"{recipe.CraftType.Value!.Name.ToDalamudString().TextValue} [{recipe.RecipeLevelTable.Value?.ClassJobLevel}]",
                    IconId = recipe.ItemResult.Value!.Icon
                }
            )
            .ToList();
    }

    /// <inheritdoc/>
    public override Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var recipe = dataManager.GetExcelSheet<Recipe>()!.GetRow(id);
        var item   = recipe?.ItemResult.Value;
        if (item == null) return null;

        string itemName  = item.Name.ToDalamudString().TextValue;
        string craftType = recipe!.CraftType.Value!.Name.ToDalamudString().TextValue;
        uint   count     = (uint)player.GetItemCount(item.RowId);

        return new() {
            Id         = id,
            Name       = $"{craftType}: {itemName}",
            IconId     = item.Icon,
            SubIconId  = 66414u,
            Count      = count > 0 ? count : null,
            IsDisabled = condition[ConditionFlag.Crafting],
        };
    }

    /// <inheritdoc/>
    public override unsafe void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        // Opening a crafting recipe is only allowed if the player is not already crafting.
        if (condition[ConditionFlag.Crafting]) return;

        AgentRecipeNote* arn = AgentRecipeNote.Instance();
        if (arn == null) return;

        arn->OpenRecipeByRecipeId(id);
    }
}
