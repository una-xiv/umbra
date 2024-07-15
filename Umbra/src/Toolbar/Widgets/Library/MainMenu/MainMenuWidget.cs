/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("MainMenu", "Widget.MainMenu.Name", "Widget.MainMenu.Description")]
internal sealed class MainMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override MenuPopup Popup { get; } = new();

    private readonly Dictionary<string, MainMenuItem> _items = [];

    private IMainMenuRepository? _repository;
    private MainMenuCategory?    _category;
    private string?              _selectedCategory;
    private bool?                _decorate;
    private string?              _displayMode;
    private string?              _iconLocation;
    private int?                 _customIconId;
    private bool?                _showItemIcons;
    private bool?                _useGrayscaleIcon;
    private int?                 _iconSize;

    public override string GetInstanceName()
    {
        return _category is null
            ? base.GetInstanceName()
            : $"{base.GetInstanceName()} - {_category.Name}";
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _repository = Framework.Service<IMainMenuRepository>();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        var selectedCategory = GetConfigValue<string>("Category");

        if (selectedCategory != _selectedCategory) {
            _selectedCategory = selectedCategory;
            _displayMode      = null;
            _iconLocation     = null;
            _customIconId     = null;
            SetCategory(_repository!.GetCategory(Enum.Parse<MenuCategory>(selectedCategory)));
        }

        if (_category is null) return;

        var decorate         = GetConfigValue<bool>("Decorate");
        var displayMode      = GetConfigValue<string>("DisplayMode");
        var iconLocation     = GetConfigValue<string>("IconLocation");
        var customIconId     = GetConfigValue<int>("CustomIconId");
        var showItemIcons    = GetConfigValue<bool>("ShowItemIcons");
        var useGrayscaleIcon = GetConfigValue<bool>("UseGrayscaleIcon");
        var iconSize         = GetConfigValue<int>("IconSize");

        if (decorate != _decorate) {
            _decorate = decorate;
            SetGhost(!decorate);
        }

        if (_showItemIcons != showItemIcons) {
            _showItemIcons = showItemIcons;

            foreach (var item in _items.Values) {
                Popup.SetButtonIcon(item.Id, showItemIcons || item.Icon is SeIconChar ? item.Icon : null);
            }
        }

        if (displayMode != _displayMode || iconLocation != _iconLocation || customIconId != _customIconId) {
            _displayMode  = displayMode;
            _iconLocation = iconLocation;
            _customIconId = customIconId;

            int existingCustomIconId = customIconId;

            if (existingCustomIconId > 0) {
                // Make sure the icon exists.
                if (Framework.Service<ITextureProvider>().GetIconPath((uint)customIconId) is null) {
                    existingCustomIconId = 0;
                }
            }

            bool hasIcon = displayMode is "IconOnly" or "TextAndIcon";

            if (hasIcon) {
                uint iconId = (existingCustomIconId == 0 ? _category.GetIconId() : (uint)existingCustomIconId);

                if (iconLocation is "Left") {
                    SetLeftIcon(iconId);
                    SetRightIcon(null);
                } else {
                    SetLeftIcon(null);
                    SetRightIcon(iconId);
                }
            } else {
                SetLeftIcon(null);
                SetRightIcon(null);
            }

            string? label    = displayMode is "TextOnly" or "TextAndIcon" ? _category.Name : null;
            bool    hasLabel = !string.IsNullOrEmpty(label);

            SetLabel(label);

            LeftIconNode.Style.Margin  = new(0, 0, 0, hasLabel ? -2 : 0);
            RightIconNode.Style.Margin = new(0, hasLabel ? -2 : 0, 0, 0);
            Node.Style.Padding         = new() { Left = hasIcon ? 3 : 0, Right = hasIcon ? 3 : 0 };

            Node.Tooltip = displayMode is "IconOnly" && !string.IsNullOrEmpty(_category.Name)
                ? _category.Name
                : null;
        }

        Popup.UseGrayscaleIcons = GetConfigValue<bool>("UseGrayscaleIcons");

        if (Popup.IsOpen) {
            foreach (var item in _category.Items) {
                if (Popup.HasButton(item.Id)) {
                    Popup.SetButtonDisabled(item.Id, item.IsDisabled);
                    Popup.SetButtonAltLabel(item.Id, string.IsNullOrEmpty(item.ShortKey) ? " " : item.ShortKey);
                }
            }
        }

        Node.QuerySelector("#Label")!.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffset"));

        if (_useGrayscaleIcon != useGrayscaleIcon) {
            _useGrayscaleIcon = useGrayscaleIcon;

            foreach (var node in Node.QuerySelectorAll(".icon")) {
                node.Style.ImageGrayscale = useGrayscaleIcon;
            }
        }

        if (_iconSize != iconSize) {
            _iconSize = iconSize;
            SetIconSize(iconSize);
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        var repository = Framework.Service<IMainMenuRepository>();

        return [
            new SelectWidgetConfigVariable(
                "Category",
                I18N.Translate("Widget.MainMenu.Config.Category.Name"),
                I18N.Translate("Widget.MainMenu.Config.Category.Description"),
                MenuCategory.Character.ToString(),
                repository.GetCategories().ToDictionary(c => c.Category.ToString(), c => c.Name)
            ),
            new SelectWidgetConfigVariable(
                "DisplayMode",
                I18N.Translate("Widget.MainMenu.Config.DisplayMode.Name"),
                I18N.Translate("Widget.MainMenu.Config.DisplayMode.Description"),
                "TextOnly",
                new() {
                    { "TextOnly", I18N.Translate("Widget.MainMenu.Config.DisplayMode.Option.TextOnly") },
                    { "IconOnly", I18N.Translate("Widget.MainMenu.Config.DisplayMode.Option.IconOnly") },
                    { "TextAndIcon", I18N.Translate("Widget.MainMenu.Config.DisplayMode.Option.TextAndIcon") }
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "IconLocation",
                I18N.Translate("Widget.MainMenu.Config.IconLocation.Name"),
                I18N.Translate("Widget.MainMenu.Config.IconLocation.Description"),
                "Left",
                new() {
                    { "Left", I18N.Translate("Widget.MainMenu.Config.IconLocation.Option.Left") },
                    { "Right", I18N.Translate("Widget.MainMenu.Config.IconLocation.Option.Right") },
                }
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "CustomIconId",
                I18N.Translate("Widget.MainMenu.Config.CustomIconId.Name"),
                I18N.Translate("Widget.MainMenu.Config.CustomIconId.Description"),
                0,
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "IconSize",
                I18N.Translate("Widget.MainMenu.Config.IconSize.Name"),
                I18N.Translate("Widget.MainMenu.Config.IconSize.Description"),
                18,
                16,
                48
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "UseGrayscaleIcon",
                I18N.Translate("Widget.MainMenu.Config.GrayscaleIcon.Name"),
                I18N.Translate("Widget.MainMenu.Config.GrayscaleIcon.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.MainMenu.Config.Decorate.Name"),
                I18N.Translate("Widget.MainMenu.Config.Decorate.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widget.MainMenu.Config.TextYOffset.Name"),
                I18N.Translate("Widget.MainMenu.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
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

    public override void Dispose()
    {
        if (_category is null) return;

        _category.OnItemAdded   -= OnItemAdded;
        _category.OnItemRemoved -= OnItemRemoved;
    }

    private void SetCategory(MainMenuCategory category)
    {
        if (_category is not null) {
            _category.OnItemAdded   -= OnItemAdded;
            _category.OnItemRemoved -= OnItemRemoved;
            Popup.Clear();
        }

        _category                =  category;
        _category!.OnItemAdded   += OnItemAdded;
        _category!.OnItemRemoved += OnItemRemoved;

        foreach (var item in _category.Items) OnItemAdded(item);
    }

    private void OnItemAdded(MainMenuItem item)
    {
        if (item.Type == MainMenuItemType.Separator) return;
        _items[item.Id] = item;

        if (item.ItemGroupId is not null
            && item.ItemGroupLabel is not null
            && Popup.HasGroup(item.ItemGroupId) == false) {
            Popup.AddGroup(item.ItemGroupId, item.ItemGroupLabel, item.SortIndex);
        }

        Popup.AddButton(
            item.Id,
            item.Name,
            item.SortIndex,
            item.Icon,
            item.ShortKey,
            item.Invoke,
            item.ItemGroupId,
            new(item.IconColor ?? 0)
        );
    }

    private void OnItemRemoved(MainMenuItem item)
    {
        if (item.Type == MainMenuItemType.Separator) return;

        _items.Remove(item.Id);
        Popup.RemoveButton(item.Id);

        if (item.ItemGroupId is not null
            && Popup.HasGroup(item.ItemGroupId)
            && Popup.GetGroupItemCount(item.ItemGroupId) == 0) {
            Popup.RemoveGroup(item.ItemGroupId);
        }
    }
}
