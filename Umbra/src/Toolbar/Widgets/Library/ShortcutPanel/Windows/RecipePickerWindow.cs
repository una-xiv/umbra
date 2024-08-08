using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class RecipePickerWindow : PickerWindowBase
{
    protected override string Title  { get; } = I18N.Translate("Widget.ShortcutPanel.PickerWindow.Recipe.Title");
    protected override string TypeId { get; } = "CR";

    private readonly List<Recipe> _recipes;

    public RecipePickerWindow()
    {
        _recipes = Framework.Service<IDataManager>().GetExcelSheet<Recipe>()!
            .Where(r => r.CraftType.Value == null || r.ItemResult.Value != null)
            .ToList();

        _recipes.Sort(
            (a, b) => string.Compare(
                a.ItemResult.Value!.Name.ToString(),
                b.ItemResult.Value!.Name.ToString(),
                StringComparison.OrdinalIgnoreCase
            )
        );

        ItemListNode.AppendChild(
            new() {
                NodeValue = I18N.Translate("Widget.ShortcutPanel.PickerWindow.Recipe.SearchHint"),
                Style = new() {
                    Anchor   = Anchor.MiddleCenter,
                    Padding  = new(8),
                    FontSize = 13,
                    Color    = new("Window.TextMuted")
                }
            }
        );
    }

    protected override void OnDisposed()
    {
        _recipes.Clear();
        base.OnDisposed();
    }

    private string _lastSearchValue = string.Empty;

    protected override void OnSearchValueChanged(string value)
    {
        if (_lastSearchValue == value) return;
        _lastSearchValue = value;

        value = value.Trim();
        if (value.Length < 1) return;

        foreach (var node in ItemListNode.ChildNodes.ToArray()) {
            node.Remove();
        }

        IEnumerable<Recipe> recipes = _recipes.Where(
            r => r.ItemResult.Value!.Name.ToString().Contains(value, StringComparison.OrdinalIgnoreCase)
        );

        foreach (var recipe in recipes.Take(50)) {
            AddItem(
                recipe.ItemResult.Value!.Name.ToString(),
                recipe.CraftType.Value?.Name.ToString() ?? "",
                recipe.ItemResult.Value!.Icon,
                () => {
                    Logger.Info($"NOW? {recipe.RowId}");
                    SetPickedItemId(recipe.RowId);
                    Close();
                }
            );
        }
    }
}
