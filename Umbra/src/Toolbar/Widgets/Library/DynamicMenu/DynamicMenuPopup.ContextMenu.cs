using Lumina.Data;
using Umbra.Widgets.Library.ShortcutPanel.Providers;
using Umbra.Widgets.Library.ShortcutPanel.Windows;
using Umbra.Windows;
using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

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
                            Cj = 0xFFFFFFFF,
                            Cl = "Custom Item",
                            Cm = "Emote",
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
                new("AddCategory") {
                    Label = I18N.Translate("Widget.DynamicMenu.ContextMenu.AddCategory"),
                    OnClick = () => {
                        DynamicMenuEntry entry = CreateCategoryEntry();
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
                    OnClick = () => {
                        if (_selectedItemIndex == null) return;
                        var item = Entries[_selectedItemIndex.Value];

                        if (IsCategory(item)) {
                            OpenCategoryEditor();
                        } else if (IsSeparator(item)) {
                            OpenSeparatorEditor();
                        } else if (item.Pt == null) {
                            OpenCustomItemEditor();
                        } else if (item.Pt is "SM" or "IM") {
                            OpenMacroItemEditor();
                        }
                    },
                },
                new("MoveToTop") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveToTop"),
                    OnClick = () => MoveItemToTop(Entries[_selectedItemIndex!.Value]),
                },
                new("MoveUp") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveUp"),
                    OnClick = () => MoveItemUp(Entries[_selectedItemIndex!.Value]),
                },
                new("MoveDown") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveDown"),
                    OnClick = () => MoveItemDown(Entries[_selectedItemIndex!.Value]),
                },
                new("MoveToBottom") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveToBottom"),
                    OnClick = () => MoveItemToBottom(Entries[_selectedItemIndex!.Value]),
                },
                new("MoveToCategory") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.MoveToCategory"),
                    OnClick = () => OpenMoveToCategoryWindow(),
                },
                new("RemoveFromCategory") {
                    Label   = I18N.Translate("Widget.DynamicMenu.ContextMenu.RemoveFromCategory"),
                    OnClick = () => RemoveItemFromCategory(Entries[_selectedItemIndex!.Value]),
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
        bool isEntrySelected = itemIndex != null;
        DynamicMenuEntry? entry = isEntrySelected ? Entries[itemIndex!.Value] : null;
        bool isCategory = entry != null && IsCategory(entry);
        bool isSeparator = entry != null && IsSeparator(entry);
        bool isItemInCategory = entry != null && IsInCategory(entry);

        foreach (var provider in Providers.GetAllProviders()) {
            ContextMenu!.SetEntryVisible(provider.ShortcutType, EditModeEnabled && !isCategory);
        }

        ContextMenu!.SetEntryVisible("AddCustomItem",       EditModeEnabled && !isCategory);
        ContextMenu!.SetEntryVisible("AddSeparator",        EditModeEnabled && !isCategory && !isItemInCategory);
        ContextMenu!.SetEntryVisible("AddCategory",         EditModeEnabled && !isCategory && !isItemInCategory);
        ContextMenu!.SetEntryVisible("-",                   EditModeEnabled && !isCategory);
        ContextMenu!.SetEntryVisible("DisableEditMode",     EditModeEnabled);
        ContextMenu!.SetEntryVisible("EnableEditMode",      !EditModeEnabled);
        ContextMenu!.SetEntryVisible("Configure",           isEntrySelected);
        ContextMenu!.SetEntryVisible("MoveToTop",           isEntrySelected);
        ContextMenu!.SetEntryVisible("MoveUp",              isEntrySelected);
        ContextMenu!.SetEntryVisible("MoveDown",            isEntrySelected);
        ContextMenu!.SetEntryVisible("MoveToBottom",        isEntrySelected);
        ContextMenu!.SetEntryVisible("MoveToCategory",      isEntrySelected && !isCategory && !isSeparator);
        ContextMenu!.SetEntryVisible("RemoveFromCategory",  isEntrySelected && !isCategory && !isSeparator && isItemInCategory);
        ContextMenu!.SetEntryVisible("Remove",              isEntrySelected);

        if (isCategory && EditModeEnabled) {
            foreach (var provider in Providers.GetAllProviders()) {
                ContextMenu!.SetEntryVisible(provider.ShortcutType, false);
            }
            ContextMenu!.SetEntryVisible("AddCustomItem",       false);
            ContextMenu!.SetEntryVisible("AddSeparator",        false);
            ContextMenu!.SetEntryVisible("AddCategory",         false);
            ContextMenu!.SetEntryVisible("MoveToCategory",      false);
            ContextMenu!.SetEntryVisible("RemoveFromCategory",  false);
            ContextMenu!.SetEntryVisible("-",                   false);
            ContextMenu!.SetEntryVisible("EnableEditMode",      false);
        }

        if (isEntrySelected) {
            ContextMenu!.SetEntryDisabled("Configure", entry!.Pt is not (null or "SM" or "IM") && !isCategory);
            ContextMenu!.SetEntryDisabled("MoveToTop",      !CanMoveItemUp(entry!));
            ContextMenu!.SetEntryDisabled("MoveUp",         !CanMoveItemUp(entry!));
            ContextMenu!.SetEntryDisabled("MoveDown",       !CanMoveItemDown(entry!));
            ContextMenu!.SetEntryDisabled("MoveToBottom",   !CanMoveItemDown(entry!));

            bool hasCategories = Entries.Any(candidate => IsCategory(candidate));
            ContextMenu!.SetEntryDisabled("MoveToCategory", !hasCategories);
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

    private void OpenMoveToCategoryWindow()
    {
        if (_selectedItemIndex == null) return;
        var entry = Entries[_selectedItemIndex.Value];
        if (IsCategory(entry) || IsSeparator(entry)) return;

        var categories = Entries.Where(IsCategory).ToList();
        StringSelectVariable categoryVar = new("Category") {
            Name = I18N.Translate("Widget.DynamicMenu.MoveToCategory.Label"),
            Description = I18N.Translate("Widget.DynamicMenu.MoveToCategory.Description"),
            Value = categories.FirstOrDefault()?.Ck ?? "CreateNew",
            Choices = categories
                .Where(category => !string.IsNullOrWhiteSpace(category.Ck))
                .ToDictionary(
                    category => category.Ck!,
                    category => string.IsNullOrWhiteSpace(category.Cl)
                        ? I18N.Translate("Widget.DynamicMenu.Category.DefaultLabel")
                        : category.Cl!
                ),
        };

        categoryVar.Choices.Add("CreateNew", I18N.Translate("Widget.DynamicMenu.MoveToCategory.CreateNew"));

        List<Variable> variables = [categoryVar];
        VariablesEditorWindow window = new(I18N.Translate("Widget.DynamicMenu.MoveToCategory.Title"), variables, []);

        Framework
            .Service<WindowManager>()
            .Present(
                "MoveToCategory",
                window,
                _ => {
                    if (categoryVar.Value == "CreateNew") {
                        var newCategory = CreateCategoryEntry();
                        InsertCategoryNearEntry(newCategory, entry);
                        MoveItemToCategory(entry, newCategory);
                        return;
                    }

                    var categoryEntry = categories.FirstOrDefault(category => category.Ck == categoryVar.Value);
                    if (categoryEntry == null) return;
                    MoveItemToCategory(entry, categoryEntry);
                }
            );
    }
}
