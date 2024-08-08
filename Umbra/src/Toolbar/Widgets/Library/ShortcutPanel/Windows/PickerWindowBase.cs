using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Windows;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal abstract partial class PickerWindowBase : Window
{
    public string? PickedId => _pickedItemId == null ? null : $"{TypeId}/{_pickedItemId}";

    protected override Vector2 MinSize     { get; } = new(300, 512);
    protected override Vector2 MaxSize     { get; } = new(600, 1300);
    protected override Vector2 DefaultSize { get; } = new(450, 720);

    /// <summary>
    /// The type ID to use in the generated ID for the picked item, so the
    /// slot knows what type of item it is. Should be unique for each picker.
    /// </summary>
    protected abstract string TypeId { get; }

    protected void SetPickedItemId(uint id)
    {
        _pickedItemId = id;
    }

    private uint? _pickedItemId;

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        Node.QuerySelector<StringInputNode>("#Search")!.OnValueChanged += OnSearchValueChanged;
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        Node.QuerySelector<StringInputNode>("#Search")!.OnValueChanged -= OnSearchValueChanged;
    }

    /// <inheritdoc/>
    protected override void OnUpdate(int instanceId)
    {
        Node.Style.Size                                       = ContentSize;
        Node.QuerySelector("#ItemList")!.Style.Size          = new(ContentSize.Width, ContentSize.Height - 45);
        Node.QuerySelector("#ItemListWrapper")!.Style.Size   = new(ContentSize.Width, 0);
        Node.QuerySelector("#SearchPanel")!.Style.Size        = new(ContentSize.Width, 0);
        Node.QuerySelector("#SearchInputWrapper")!.Style.Size = new(ContentSize.Width - 55, 0);

        foreach (Node node in Node.QuerySelectorAll(".item")) {
            node.Style.Size                              = new(ContentSize.Width - 30, 42);
            node.QuerySelector(".item-name")!.Style.Size = new(ContentSize.Width - 82, 0);
            node.QuerySelector(".item-command")!.Style.Size = new(ContentSize.Width - 82, 0);
        }
    }

    protected virtual void OnSearchValueChanged(string value)
    {
        foreach (Node node in Node.QuerySelectorAll(".item")) {
            node.Style.IsVisible = string.IsNullOrEmpty(value) ||
                node.QuerySelector(".item-name")!.NodeValue!.ToString()!.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
