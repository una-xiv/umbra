using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Una.Drawing;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class ShortcutPickerWindow : PickerWindowBase
{
    protected override string Title  { get; }
    protected override string TypeId { get; }

    private string? _lastSearchValue = string.Empty;

    private readonly AbstractShortcutProvider _provider;

    public ShortcutPickerWindow(AbstractShortcutProvider provider)
    {
        Title  = provider.ContextMenuEntryName;
        TypeId = provider.ShortcutType;

        _provider = provider;

        OnSearchValueChanged(null);
    }

    protected override void OnSearchValueChanged(string? value)
    {
        if (value != null && _lastSearchValue == value) return;
        _lastSearchValue = value;

        value = value?.Trim() ?? "";
        if (value.Length < _provider.MinSearchLength) return;

        foreach (var node in ItemListNode.ChildNodes.ToArray()) node.Remove();
        IList<Shortcut> items = _provider.GetShortcuts(value);

        foreach (var item in items.Take(50)) {
            AddItem(
                item.Name,
                item.Description,
                item.IconId,
                () => {
                    SetPickedItemId(item.Id);
                    Close();
                }
            );
        }

        if (items.Count > 50) {
            AddTooManyResultsMessage();
        }
    }
}
