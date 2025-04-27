using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("MainMenu", "Widget.MainMenu.Name", "Widget.MainMenu.Description", ["menu", "main", "navigation", "category"])]
internal sealed class MainMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    private IMainMenuRepository? _repository;
    private MainMenuCategory?    _category;
    private string?              _selectedCategory;

    private readonly Dictionary<string, MenuPopup.Button> _buttons = [];
    private readonly Dictionary<string, MenuPopup.Group>  _groups  = [];

    public override string GetInstanceName()
    {
        return _category is null
            ? base.GetInstanceName()
            : $"{base.GetInstanceName()} - {_category.Name}";
    }

    protected override void OnLoad()
    {
        _repository       =  Framework.Service<IMainMenuRepository>();
        Popup.OnPopupOpen += OnPopup2Opened;
    }

    protected override void OnUnload()
    {
        Popup.OnPopupOpen -= OnPopup2Opened;

        if (_category is null) return;

        _category.OnItemAdded   -= OnItemAdded;
        _category.OnItemRemoved -= OnItemRemoved;
    }

    protected override void OnDraw()
    {
        var selectedCategory = GetConfigValue<string>("Category");

        if (selectedCategory != _selectedCategory) {
            _selectedCategory = selectedCategory;
            SetCategory(_repository!.GetCategory(Enum.Parse<MenuCategory>(selectedCategory)));
        }

        if (_category is null) return;

        if (!GetConfigValue<bool>("UseCustomIcon")) {
            SetGameIconId(_category.GetIconId());
        }

        SetText(_category.Name);

        Popup.UseGrayscaleIcons = GetConfigValue<bool>("UseGrayscaleIcons");

        if (Popup.IsOpen && _category != null) {
            var showItemIcons = GetConfigValue<bool>("ShowItemIcons");

            foreach (var item in _category.Items) {
                if (!_buttons.TryGetValue(item.Id, out var button)) continue;

                button.IsDisabled = item.IsDisabled;
                button.AltText    = string.IsNullOrEmpty(item.ShortKey) ? " " : item.ShortKey;
                button.Icon       = showItemIcons || item.Icon is SeIconChar ? item.Icon : null;
                button.IconColor  = item.IconColor != null ? new(item.IconColor.Value) : null;
                button.SortIndex  = item.SortIndex;
            }
        }
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        var repository = Framework.Service<IMainMenuRepository>();

        return [
            ..base.GetConfigVariables(),
            new SelectWidgetConfigVariable(
                "Category",
                I18N.Translate("Widget.MainMenu.Config.Category.Name"),
                I18N.Translate("Widget.MainMenu.Config.Category.Description"),
                MenuCategory.Character.ToString(),
                repository.GetCategories().ToDictionary(c => c.Category.ToString(), c => c.Name)
            ),
            new BooleanWidgetConfigVariable(
                "UseCustomIcon",
                I18N.Translate("Widget.MainMenu.Config.UseCustomIcon.Name"),
                I18N.Translate("Widget.MainMenu.Config.UseCustomIcon.Description"),
                false
            ),
            new BooleanWidgetConfigVariable(
                "ShowItemIcons",
                I18N.Translate("Widget.MainMenu.Config.ShowItemIcons.Name"),
                I18N.Translate("Widget.MainMenu.Config.ShowItemIcons.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "UseGrayscaleIcons",
                I18N.Translate("Widget.MainMenu.Config.GrayscaleIcons.Name"),
                I18N.Translate("Widget.MainMenu.Config.GrayscaleIcons.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }

    private void SetCategory(MainMenuCategory category)
    {
        Popup.Clear(true);
        _buttons.Clear();
        _groups.Clear();

        if (_category is not null) {
            _category.OnItemAdded   -= OnItemAdded;
            _category.OnItemRemoved -= OnItemRemoved;
        }

        _category                =  category;
        _category!.OnItemAdded   += OnItemAdded;
        _category!.OnItemRemoved += OnItemRemoved;

        foreach (var item in _category.Items) OnItemAdded(item);
    }

    private void OnItemAdded(MainMenuItem item)
    {
        if (_buttons.ContainsKey(item.Id)) {
            Logger.Warning($"MainMenuWidget: Item with ID {item.Id} already exists in the popup.");
            return;
        }

        if (item.ItemGroupLabel != null && !_groups.TryGetValue(item.ItemGroupLabel, out var group)) {
            group = new MenuPopup.Group(item.ItemGroupLabel ?? "--") { SortIndex = item.SortIndex };
            Popup.Add(group);
            _groups.Add(item.ItemGroupLabel ?? "--", group);
        }

        switch (item.Type) {
            case MainMenuItemType.Separator:
                Popup.Add(new MenuPopup.Separator { SortIndex = item.SortIndex });
                break;
            default:
                var button = new MenuPopup.Button(item.Id) {
                    IsVisible         = true,
                    Label             = item.Name,
                    AltText           = item.ShortKey,
                    Icon              = item.Icon,
                    IconColor         = item.IconColor != null ? new(item.IconColor.Value) : null,
                    IsDisabled        = item.IsDisabled,
                    SortIndex         = item.SortIndex,
                    ClosePopupOnClick = true,
                    OnClick           = item.Invoke,
                };

                if (item.ItemGroupLabel != null) {
                    _groups[item.ItemGroupLabel].Add(button);
                } else {
                    Popup.Add(button);
                }

                _buttons.Add(item.Id, button);
                break;
        }
    }

    private void OnItemRemoved(MainMenuItem item)
    {
        if (item.Type == MainMenuItemType.Separator) return;

        Popup.RemoveById(item.Id);
        _buttons.Remove(item.Id);
    }

    private void OnPopup2Opened()
    {
        if (string.IsNullOrEmpty(GetConfigValue<string>("Category"))) return;
        SetCategory(_repository!.GetCategory(Enum.Parse<MenuCategory>(_selectedCategory!)));
    }
}
