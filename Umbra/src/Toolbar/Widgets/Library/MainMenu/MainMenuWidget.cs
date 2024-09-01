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

using Dalamud.Game.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using System.IO;
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

    private IMainMenuRepository? _repository;
    private MainMenuCategory?    _category;
    private string?              _selectedCategory;
    private string?              _displayMode;
    private string?              _iconLocation;
    private uint?                _customIconId;

    public override string GetInstanceName()
    {
        return _category is null
            ? base.GetInstanceName()
            : $"{base.GetInstanceName()} - {_category.Name}";
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _repository       =  Framework.Service<IMainMenuRepository>();
        Popup.OnPopupOpen += OnPopupOpened;
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

        var displayMode   = GetConfigValue<string>("DisplayMode");
        var iconLocation  = GetConfigValue<string>("IconLocation");
        var customIconId  = GetConfigValue<uint>("CustomIconId");
        var showItemIcons = GetConfigValue<bool>("ShowItemIcons");

        if (displayMode != _displayMode || iconLocation != _iconLocation || customIconId != _customIconId) {
            _displayMode  = displayMode;
            _iconLocation = iconLocation;
            _customIconId = customIconId;

            uint existingCustomIconId = customIconId;

            if (existingCustomIconId > 0) {
                // Make sure the icon exists.
                try {
                    Framework.Service<ITextureProvider>().GetIconPath(customIconId);
                } catch (FileNotFoundException) {
                    existingCustomIconId = 0;
                }
            }

            bool hasIcon = displayMode is "IconOnly" or "TextAndIcon";

            if (hasIcon) {
                SetIcon((existingCustomIconId == 0 ? _category.GetIconId() : existingCustomIconId));
            } else {
                SetIcon(null);
            }

            SetLabel(_category.Name);
        }

        Popup.UseGrayscaleIcons = GetConfigValue<bool>("UseGrayscaleIcons");

        if (Popup.IsOpen) {
            foreach (var item in _category.Items) {
                if (Popup.HasButton(item.Id)) {
                    Popup.SetButtonDisabled(item.Id, item.IsDisabled);
                    Popup.SetButtonAltLabel(item.Id, string.IsNullOrEmpty(item.ShortKey) ? " " : item.ShortKey);
                    Popup.SetButtonIcon(item.Id, showItemIcons || item.Icon is SeIconChar ? item.Icon : null);
                }
            }
        }

        base.OnUpdate();
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
            new IconIdWidgetConfigVariable(
                "CustomIconId",
                I18N.Translate("Widget.MainMenu.Config.CustomIconId.Name"),
                I18N.Translate("Widget.MainMenu.Config.CustomIconId.Description"),
                0
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
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

    protected override void OnDisposed()
    {
        Popup.OnPopupOpen -= OnPopupOpened;

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

        Popup.RemoveButton(item.Id);

        if (item.ItemGroupId is not null
            && Popup.HasGroup(item.ItemGroupId)
            && Popup.GetGroupItemCount(item.ItemGroupId) == 0) {
            Popup.RemoveGroup(item.ItemGroupId);
        }
    }

    private void OnPopupOpened()
    {
        if (string.IsNullOrEmpty(GetConfigValue<string>("Category"))) return;
        SetCategory(_repository!.GetCategory(Enum.Parse<MenuCategory>(_selectedCategory!)));
    }
}
