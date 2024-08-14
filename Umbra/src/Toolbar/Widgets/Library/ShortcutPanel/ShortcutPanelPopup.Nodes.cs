using ImGuiNET;
using System;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Una.Drawing;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup
{
    private Node?  _selectedSlotNode;
    private byte   _selectedCategory;
    private int    _selectedSlotIndex;
    private byte   _numRows;
    private byte   _numCols;
    private string _catHash = string.Empty;

    private void UpdateGridDimensions()
    {
        string catHash = string.Join('$', CategoryNames);

        if (_catHash != catHash || _numRows != NumRows || _numCols != NumCols) {
            _numRows = NumRows;
            _numCols = NumCols;
            _catHash = catHash;

            ReconfigurePanel(CategoryNames, _numRows, _numCols);
        }
    }

    protected override void OnButtonSlotCreated(Node node, byte categoryId, int slotId)
    {
        node.OnMouseUp += _ => InvokeAction(categoryId, slotId);

        node.OnRightClick += n => {
            string[] parts = n.Id!.Split('_');
            _selectedSlotNode  = n;
            _selectedCategory  = byte.Parse(parts[1]);
            _selectedSlotIndex = int.Parse(parts[2]);

            string? data = GetShortcutData(_selectedCategory, _selectedSlotIndex);
            ContextMenu?.SetEntryDisabled("Copy", data == null);
            ContextMenu?.SetEntryDisabled("Clear", data == null);

            string? clipboardText = ImGui.GetClipboardText();
            ContextMenu?.SetEntryDisabled("Paste", string.IsNullOrEmpty(clipboardText) || !clipboardText.StartsWith("SC:"));

            ContextMenu?.Present();
        };
    }

    private void SetButton(byte categoryId, int slotId, string? data)
    {
        Node? node = GetSlotContainer(categoryId).QuerySelector($"#Slot_{categoryId}_{slotId}");
        if (node == null) return;

        if (data == null) {
            ClearSlot(node, categoryId, slotId);
            return;
        }

        string typeId;
        uint   itemId;

        try {
            string[] payload = data.Split('/');
            typeId = payload[0];
            itemId = uint.Parse(payload[1]);
        } catch (Exception e) {
            Logger.Error($"Failed to parse shortcut data for slot {categoryId}:{slotId} -> {e.Message}");
            return;
        }

        if (typeId == string.Empty) {
            ClearSlot(node, categoryId, slotId);
            return;
        }

        AbstractShortcutProvider? provider = Providers.GetProvider(typeId);
        if (provider == null) {
            Logger.Error($"No provider found for shortcut type: {typeId}");
            return;
        }

        Shortcut? shortcut = provider.GetShortcut(itemId, WidgetInstanceId);

        if (shortcut == null) {
            Logger.Warning($"Shortcut could not be constructed: {typeId}:{itemId}");
            ClearSlot(node, categoryId, slotId);
            return;
        }

        node.TagsList.Remove("empty-hidden");
        node.TagsList.Remove("empty-visible");

        SetSlotState(node, shortcut.Value);
        AssignAction(categoryId, slotId, typeId, itemId);
    }

    private void ClearSlot(Node slotNode, byte categoryId, int slotId)
    {
        slotNode.Tooltip                                               = null;
        slotNode.QuerySelector(".slot-button--icon")!.Style.IconId     = null;
        slotNode.QuerySelector(".slot-button--sub-icon")!.Style.IconId = null;
        slotNode.QuerySelector(".slot-button--count")!.NodeValue       = null;

        slotNode.TagsList.Add($"empty-{(ShowEmptySlots ? "visible" : "hidden")}");
        slotNode.TagsList.Remove($"empty-{(ShowEmptySlots ? "hidden" : "visible")}");
        slotNode.TagsList.Remove("blocked");

        AssignAction(categoryId, slotId, null, null);
        AssignShortcut(categoryId, slotId, null);
    }

    private static void SetSlotState(Node slotNode, Shortcut shortcut)
    {
        slotNode.QuerySelector(".slot-button--icon")!.Style.IconId     = shortcut.IconId;
        slotNode.QuerySelector(".slot-button--sub-icon")!.Style.IconId = shortcut.SubIconId;
        slotNode.QuerySelector(".slot-button--count")!.NodeValue       = shortcut.Count?.ToString();
        slotNode.Tooltip                                               = shortcut.Name;
    }
}
