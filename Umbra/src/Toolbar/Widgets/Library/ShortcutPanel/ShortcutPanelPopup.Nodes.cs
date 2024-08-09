using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using Umbra.Common;
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
        Node? node = GetSlotContainer(categoryId)?.QuerySelector($"#Slot_{categoryId}_{slotId}");
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

        node.TagsList.Remove("empty-hidden");
        node.TagsList.Remove("empty-visible");

        switch (typeId) {
            case "I": // Inventory Item
                SetInventoryItemSlot(node, categoryId, slotId, itemId);
                break;
            case "EI": // Key items
                SetInventoryKeyItemSlot(node, categoryId, slotId, itemId);
                break;
            case "EM": // Emote
                SetEmoteSlot(node, categoryId, slotId, itemId);
                break;
            case "IM": // Individual Macro
                SetIndividualMacroSlot(node, categoryId, slotId, itemId);
                break;
            case "SM": // Shared Macro
                SetSharedMacroSlot(node, categoryId, slotId, itemId);
                break;
            case "MC": // Main Command
                SetMainCommandSlot(node, categoryId, slotId, itemId);
                break;
            case "GA": // General Action
                SetGeneralActionSlot(node, categoryId, slotId, itemId);
                break;
            case "MO": // Mount
                SetMountSlot(node, categoryId, slotId, itemId);
                break;
            case "MI": // Minion
                SetMinionSlot(node, categoryId, slotId, itemId);
                break;
            case "CR": // Crafting Recipe
                SetCraftingRecipeSlot(node, categoryId, slotId, itemId);
                break;
            default:
                Logger.Error($"Unknown shortcut type: {typeId}");
                break;
        }
    }
}
