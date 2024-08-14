using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Umbra.Widgets.Library.ShortcutPanel.Windows;
using Umbra.Windows;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup
{
    private void CreateContextMenu()
    {
        ContextMenu = new(
            [
                ..Providers.GetAllProviders().Select(provider => new ContextMenuEntry(provider.ShortcutType) {
                    Label = provider.ContextMenuEntryName,
                    IconId = provider.ContextMenuEntryIcon,
                    OnClick = () => OpenPickerWindow(provider),
                }),
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

    private void OpenPickerWindow(AbstractShortcutProvider provider)
    {
        if (_selectedSlotNode == null) return;

        Framework.Service<WindowManager>().Present("Picker", new ShortcutPickerWindow(provider),
            window => {
                if (window.PickedId == null) return;
                Logger.Info($"Add item: {window.PickedId} at {_selectedCategory}, {_selectedSlotIndex}");
                AssignShortcut(_selectedCategory, _selectedSlotIndex, window.PickedId);
            });
    }
}
