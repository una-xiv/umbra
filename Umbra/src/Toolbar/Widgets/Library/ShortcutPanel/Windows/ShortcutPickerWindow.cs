using System.Collections.Generic;
using System.Linq;
using Umbra.Widgets.Library.ShortcutPanel.Providers;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class ShortcutPickerWindow : PickerWindowBase
{
    protected override string Title  { get; }
    protected override string TypeId { get; }

    private string? _lastSearchValue = string.Empty;

    private readonly AbstractShortcutProvider _provider;

    private const int MaxItemCount = 50;

    public ShortcutPickerWindow(AbstractShortcutProvider provider)
    {
        Title  = provider.ContextMenuEntryName;
        TypeId = provider.ShortcutType;

        _provider = provider;
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        
        OnSearchValueChanged(null);
    }

    protected override void OnSearchValueChanged(string? value)
    {
        if (value != null && _lastSearchValue == value) return;
        _lastSearchValue = value;

        value = value?.Trim() ?? "";
        if (value.Length < _provider.MinSearchLength) return;

        foreach (var node in ItemListNode.ChildNodes.ToArray()) node.Dispose();
        IList<Shortcut> items = _provider.GetShortcuts(value);

        foreach (var item in items.Take(MaxItemCount)) {
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

        if (items.Count > MaxItemCount) {
            AddTooManyResultsMessage();
        }
    }
}
