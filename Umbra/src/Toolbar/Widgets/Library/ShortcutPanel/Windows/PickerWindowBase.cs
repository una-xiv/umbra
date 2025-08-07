using Umbra.Windows;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal abstract partial class PickerWindowBase : Window
{
    public string? PickedId => _pickedItemId == null ? null : $"{TypeId}/{_pickedItemId}";

    protected override Vector2 MinSize     { get; } = new(300, 512);
    protected override Vector2 MaxSize     { get; } = new(600, 1300);
    protected override Vector2 DefaultSize { get; } = new(450, 720);

    protected override string UdtResourceName => "umbra.windows.shortcut_picker.window.xml";

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
        RootNode.QuerySelector<StringInputNode>("#Search")!.OnValueChanged += OnSearchValueChanged;
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        RootNode.QuerySelector<StringInputNode>("#Search")!.OnValueChanged -= OnSearchValueChanged;
    }

    protected virtual void OnSearchValueChanged(string value)
    {
        foreach (Node node in RootNode.QuerySelectorAll(".item")) {
            node.Style.IsVisible = string.IsNullOrEmpty(value) ||
                node.QuerySelector(".item-name")!.NodeValue!.ToString()!.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
