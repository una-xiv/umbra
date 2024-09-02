using Newtonsoft.Json;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Umbra.Widgets.Library.ShortcutPanel.Windows;
using Umbra.Windows;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuPopup
{
    private int? _selectedItemIndex;

    private void CreateContextMenu()
    {
        ContextMenu = new(
            [
                new("AddCustomItem") {
                    Label = I18N.Translate("Widget.DynamicMenu.ContextMenu.AddCustomItem"),
                    OnClick = () => {
                        DynamicMenuEntry entry = new() {
                            Ci = 14,
                            Cl = "Custom Item",
                            Cc = "/dance",
                            Ct = "Chat",
                        };

                        AddEntry(entry, _selectedItemIndex);
                        _selectedItemIndex = Entries.IndexOf(entry);
                        OpenCustomItemEditor();
                    },
                },
                ..Providers
                    .GetAllProviders()
                    .Select(
                        provider => new ContextMenuEntry(provider.ShortcutType) {
                            Label   = provider.ContextMenuEntryName,
                            IconId  = provider.ContextMenuEntryIcon,
                            OnClick = () => OpenPickerWindow(provider),
                        }
                    ),
                new("AddSeparator") {
                    Label = I18N.Translate("Widget.DynamicMenu.ContextMenu.AddSeparator"),
                    OnClick = () => {
                        DynamicMenuEntry entry = new() { Cl = "-" };
                        AddEntry(entry, _selectedItemIndex);
                    },
                },
                new("-"),
                new("EnableEditMode") {
                    Label = I18N.Translate("Widget.DynamicMenu.ContextMenu.EnableEditMode"),
                    OnClick = () => {
                        EmptyButtonPlaceholder.Style.IsVisible = true;
                        EditModeEnabled                        = true;
                        OnEditModeChanged?.Invoke(true);
                    },
                },
                new("DisableEditMode") {
                    Label = I18N.Translate("Widget.DynamicMenu.ContextMenu.DisableEditMode"),
                    OnClick = () => {
                        EmptyButtonPlaceholder.Style.IsVisible = false;
                        EditModeEnabled                        = false;
                        OnEditModeChanged?.Invoke(false);
                        if (Entries.Count == 0) Close();
                    },
                },
                new("Configure") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.Configure"),
                    OnClick = OpenCustomItemEditor,
                },
                new("MoveUp") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveUp"),
                    OnClick = () => MoveItemUp(Entries[_selectedItemIndex!.Value]),
                },
                new("MoveDown") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveDown"),
                    OnClick = () => MoveItemDown(Entries[_selectedItemIndex!.Value]),
                },
                new("Remove") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.Remove"),
                    OnClick = () => RemoveItem(Entries[_selectedItemIndex!.Value]),
                    IconId  = 61502u,
                },
            ]
        );
    }

    private void OpenContextMenu(int? itemIndex = null)
    {
        _selectedItemIndex = itemIndex;

        foreach (var provider in Providers.GetAllProviders()) {
            ContextMenu!.SetEntryVisible(provider.ShortcutType, EditModeEnabled);
        }

        ContextMenu!.SetEntryVisible("AddCustomItem",   EditModeEnabled);
        ContextMenu!.SetEntryVisible("AddSeparator",    EditModeEnabled);
        ContextMenu!.SetEntryVisible("-",               EditModeEnabled);
        ContextMenu!.SetEntryVisible("DisableEditMode", EditModeEnabled);
        ContextMenu!.SetEntryVisible("EnableEditMode",  !EditModeEnabled);
        ContextMenu!.SetEntryVisible("Configure",       itemIndex != null);
        ContextMenu!.SetEntryVisible("MoveUp",          itemIndex != null);
        ContextMenu!.SetEntryVisible("MoveDown",        itemIndex != null);
        ContextMenu!.SetEntryVisible("Remove",          itemIndex != null);

        if (itemIndex != null) {
            var entry = Entries[itemIndex.Value];
            ContextMenu!.SetEntryDisabled("Configure", entry.Pt != null || entry.Cl == "-");
            ContextMenu!.SetEntryDisabled("MoveUp",    !CanMoveItemUp(Entries[itemIndex.Value]));
            ContextMenu!.SetEntryDisabled("MoveDown",  !CanMoveItemDown(Entries[itemIndex.Value]));
        }

        ContextMenu!.Present();
    }

    private void OpenPickerWindow(AbstractShortcutProvider provider)
    {
        Framework
            .Service<WindowManager>()
            .Present(
                "Picker",
                new ShortcutPickerWindow(provider),
                window => {
                    if (window.PickedId == null) return;
                    string providerType = window.PickedId.Split("/")[0];
                    uint   providerId   = uint.Parse(window.PickedId.Split("/")[1]);

                    DynamicMenuEntry entry = new() {
                        Pt = providerType,
                        Pi = providerId,
                    };

                    AddEntry(entry, _selectedItemIndex);
                }
            );
    }
}
