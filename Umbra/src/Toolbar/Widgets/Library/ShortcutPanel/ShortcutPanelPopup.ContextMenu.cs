using ImGuiNET;
using System;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Windows;
using Umbra.Windows;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup
{
    private void CreateContextMenu()
    {
        ContextMenu = new(
            [
                new("PickItem") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickItem"),
                    IconId  = 2u,
                    OnClick = ContextActionPickItem,
                },
                new("PickKeyItem") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickKeyItem"),
                    IconId  = 3u,
                    OnClick = ContextActionPickKeyItem,
                },
                new("PickEmote") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickEmote"),
                    IconId  = 9u,
                    OnClick = ContextActionPickEmote,
                },
                new("PickMinion") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickMinion"),
                    IconId  = 59u,
                    OnClick = ContextActionPickMinion,
                },
                new("PickMount") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickMount"),
                    IconId  = 58u,
                    OnClick = ContextActionPickMount,
                },
                new("PickGeneralAction") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickGeneralAction"),
                    IconId  = 4u,
                    OnClick = ContextActionPickGeneralAction,
                },
                new("PickMainCommand") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickMainCommand"),
                    IconId  = 29u,
                    OnClick = ContextActionPickMainCommand,
                },
                new("PickIndividualMacro") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickIndividualMacro"),
                    IconId  = 30u,
                    OnClick = ContextActionPickIndividualMacro,
                },
                new("PickSharedMacro") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickSharedMacro"),
                    IconId  = 30u,
                    OnClick = ContextActionPickSharedMacro,
                },
                new("PickCraftingLog") {
                    Label   = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PickCraftingRecipe"),
                    IconId  = 22u,
                    OnClick = ContextActionPickCraftingRecipe,
                },
                new("-"),
                new("Copy") {
                    Label      = I18N.Translate("Widget.ShortcutPanel.ContextMenu.CopySlot"),
                    OnClick    = ContextActionCopySlot,
                    IsDisabled = true,
                },
                new("Paste") {
                    Label      = I18N.Translate("Widget.ShortcutPanel.ContextMenu.PasteSlot"),
                    OnClick    = ContextActionPasteSlot,
                    IsDisabled = true,
                },
                new("Clear") {
                    Label      = I18N.Translate("Widget.ShortcutPanel.ContextMenu.ClearSlot"),
                    OnClick    = ContextActionClearSlot,
                    IconId     = 61502u,
                    IsDisabled = true,
                },
            ]
        );
    }

    private void ContextActionPickItem()
    {
        if (_selectedSlotNode == null) return;

        OpenPickerWindow<ItemPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickKeyItem()
    {
        if (_selectedSlotNode == null) return;

        OpenPickerWindow<KeyItemPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickEmote()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<EmotePickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickMount()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<MountPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickMinion()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<MinionPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickGeneralAction()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<GeneralActionPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickMainCommand()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<MainCommandPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickIndividualMacro()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<IndividualMacroPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickSharedMacro()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<SharedMacroPickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionPickCraftingRecipe()
    {
        if (_selectedSlotNode != null) OpenPickerWindow<RecipePickerWindow>(AssignPickedIdToSelectedSlot);
    }

    private void ContextActionClearSlot()
    {
        if (_selectedSlotNode == null) return;
        ClearSlot(_selectedSlotNode, _selectedCategory, _selectedSlotIndex);
    }

    private void ContextActionCopySlot()
    {
        if (_selectedSlotNode == null) return;

        string? data = GetShortcutData(_selectedCategory, _selectedSlotIndex);
        if (data == null) return;

        ImGui.SetClipboardText($"SC:{data}");
    }

    private void ContextActionPasteSlot()
    {
        if (_selectedSlotNode == null) return;

        string? clipboard = ImGui.GetClipboardText();
        if (string.IsNullOrEmpty(clipboard) || !clipboard.StartsWith("SC:")) return;

        AssignShortcut(_selectedCategory, _selectedSlotIndex, clipboard[3..]);
        SetButton(_selectedCategory, _selectedSlotIndex, clipboard[3..]);
    }

    private void OpenPickerWindow<T>(Action<string?> result) where T : PickerWindowBase, new()
    {
        if (_selectedSlotNode == null) return;

        Framework.Service<WindowManager>().Present("Picker", new T(), window => result(window.PickedId));
    }

    private void AssignPickedIdToSelectedSlot(string? id)
    {
        Logger.Info($"Assign: {id}");
        if (id == null || _selectedSlotNode == null) return;

        Logger.Info(
            $"Assign picked ID {id} to slot {_selectedSlotNode.Id} ({_selectedCategory}, {_selectedSlotIndex})"
        );

        AssignShortcut(_selectedCategory, _selectedSlotIndex, id);
    }
}
