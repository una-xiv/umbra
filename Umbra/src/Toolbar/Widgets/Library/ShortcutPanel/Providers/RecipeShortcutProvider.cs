﻿using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

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

        List<Recipe> recipes = dataManager.GetExcelSheet<Recipe>()
            .Where(
                r => r is { RowId: > 0, ItemResult.ValueNullable: not null }
                    && !string.IsNullOrEmpty(r.ItemResult.Value.Name.ExtractText())
                    && (r.SecretRecipeBook.RowId == 0 || ps->IsSecretRecipeBookUnlocked(r.SecretRecipeBook.RowId))
                    && (r.Quest.RowId == 0 || QuestManager.IsQuestComplete(r.Quest.RowId))
                    && (r
                            .ItemResult.ValueNullable?.Name.ToString()
                            .Contains(searchFilter ?? "", StringComparison.OrdinalIgnoreCase)
                        ?? false)
            )
            .ToList();

        recipes.Sort(
            (a, b) => string.Compare(
                a.ItemResult.Value.Name.ExtractText().ToString(),
                b.ItemResult.Value.Name.ExtractText().ToString(),
                StringComparison.OrdinalIgnoreCase
            )
        );

        return recipes
            .Take(51)
            .Select(
                recipe => new Shortcut() {
                    Id   = recipe.RowId,
                    Name = recipe.ItemResult.Value.Name.ToString(),
                    Description =
                        $"{recipe.CraftType.Value.Name.ExtractText()} [{recipe.RecipeLevelTable.ValueNullable?.ClassJobLevel}]",
                    IconId = recipe.ItemResult.Value.Icon
                }
            )
            .ToList();
    }

    /// <inheritdoc/>
    public override Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        if (id == 0u) return null;

        var recipe = dataManager.GetExcelSheet<Recipe>().GetRow(id);
        var item   = recipe.ItemResult.ValueNullable;
        if (item == null) return null;

        string itemName  = item.Value.Name.ExtractText();
        string craftType = recipe.CraftType.Value.Name.ExtractText();
        uint   count     = (uint)player.GetItemCount(item.Value.RowId);

        return new() {
            Id         = id,
            Name       = $"{craftType}: {itemName}",
            IconId     = item.Value.Icon,
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
