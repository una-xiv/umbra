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
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Gearset;

[Service]
internal partial class CurrencyWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.Currencies.Enabled", "ToolbarWidgets")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.Currencies.TrackedCurrencyId")]
    private static uint TrackedCurrencyId { get; set; }

    private readonly List<uint>          _currencyIds;
    private readonly IDataManager        _dataManager;
    private readonly Player              _player;
    private readonly ToolbarPopupContext _ctx;

    private int _lastCurrencyCount = 0;

    public CurrencyWidget(IDataManager dataManager, Player player, ToolbarPopupContext ctx)
    {
        _dataManager = dataManager;
        _player      = player;
        _ctx         = ctx;

        _currencyIds = [
            1,                          // Gil
            29,                         // MGP
            20,                         // Maelstrom
            21,                         // Twin Adder
            22,                         // Immortal Flames
            27,                         // Allied Seals
            10307,                      // Centurio Seals
            26533,                      // Sacks of Nuts
            GetLimitedTomestoneId(),    // Limited Tomestones
            GetNonLimitedTomestoneId(), // Non-limited Tomestones
            28,                         // Poetics
            25,                         // Wolf Marks
            36656,                      // Trophy Crystals
            33913,                      // Purple Crafter Scrips
            33914,                      // Purple Gatherer Scrips
            25199,                      // White Crafter Scrips
            25200,                      // White Gatherer Scrips
            28063,                      // Skybuilders Scrips
            26807                       // Bicolor Gemstones
        ];

        ctx.RegisterDropdownActivator(Element, _dropdownElement);

        BuildCurrencyItemList();
        AlignCurrencyRows();

        Element.OnMouseEnter += () => Element.Style.TextColor = 0xFFFFFFFF;
        Element.OnMouseLeave += () => Element.Style.TextColor = 0xFFC0C0C0;
        Element.OnRightClick += OnRightClick;
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;

        if (0 == TrackedCurrencyId) {
            Element.Get("Text").Text        = I18N.Translate("CurrencyWidget.Name");
            Element.Get("Icon").Text        = FontAwesomeIcon.Coins.ToIconString();
            Element.Get("Icon").Style.Image = null;
        } else {
            var trackedItem = _dataManager.GetExcelSheet<Item>()!.GetRow(TrackedCurrencyId);

            if (null != trackedItem) {
                int currencyCount = _player.GetItemCount(TrackedCurrencyId);

                if (_lastCurrencyCount != currencyCount) {
                    _lastCurrencyCount          = currencyCount;
                    Element.Get("Text").Padding = new(left: -16);
                    Element.Get("Text").Animate(new Animation<OutElastic>(500) { Padding = new(left: 0) });
                }

                Element.Get("Icon").Style.Image = (uint)trackedItem.Icon;
                Element.Get("Icon").Text        = "";
                Element.Get("Text").Text        = $"{currencyCount:N0} {trackedItem.Name}";
            }
        }

        if (!_dropdownElement.IsVisible) return;

        foreach (uint currencyId in _currencyIds) {
            if (!_dropdownElement.Has($"Content.Currency_{currencyId}.Value")) continue;
            var item = _dataManager.GetExcelSheet<Item>()!.GetRow(currencyId);
            if (item == null) continue;

            Element el     = _dropdownElement.Get($"Content.Currency_{currencyId}.Value");
            int     amount = _player.GetItemCount(currencyId);

            // FIXME: Max stack sizes sometimes depend on other factors, such as GC level, limited tomestone types, etc.
            //        We're currently just using the default stack size for all items.

            el.Text = item.StackSize > 0 && item.RowId != 1 ? $"{amount:N0} / {item.StackSize:N0}" : $"{amount:N0}";
        }
    }

    public void OnUpdate() { }

    private void BuildCurrencyItemList()
    {
        var target = _dropdownElement.Get("Content");

        target.AddChild(CreateCurrencyRow(1));  // Gil
        target.AddChild(CreateCurrencyRow(29)); // MGP

        if (_player.GrandCompanyId == 1) target.AddChild(CreateCurrencyRow(20)); // Maelstrom
        if (_player.GrandCompanyId == 2) target.AddChild(CreateCurrencyRow(21)); // Twin Adder
        if (_player.GrandCompanyId == 3) target.AddChild(CreateCurrencyRow(22)); // Immortal Flames

        target.AddChild(new DropdownSeparatorElement { SortIndex = _sortIndex++ });
        target.AddChild(CreateCurrencyRow(27));    // Allied Seals
        target.AddChild(CreateCurrencyRow(10307)); // Centurio Seals
        target.AddChild(CreateCurrencyRow(26533)); // Sacks of Nuts
        target.AddChild(new DropdownSeparatorElement { SortIndex = _sortIndex++ });
        target.AddChild(CreateCurrencyRow(GetLimitedTomestoneId()));    // Limited Tomestones
        target.AddChild(CreateCurrencyRow(GetNonLimitedTomestoneId())); // Non-limited Tomestones
        target.AddChild(CreateCurrencyRow(28));                         // Poetics
        target.AddChild(new DropdownSeparatorElement { SortIndex = _sortIndex++ });
        target.AddChild(CreateCurrencyRow(25));    // Wolf Marks
        target.AddChild(CreateCurrencyRow(36656)); // Trophy Crystals
        target.AddChild(new DropdownSeparatorElement { SortIndex = _sortIndex++ });
        target.AddChild(CreateCurrencyRow(33913)); // Purple Crafter Scrips
        target.AddChild(CreateCurrencyRow(33914)); // Purple Gatherer Scrips
        target.AddChild(CreateCurrencyRow(25199)); // White Crafter Scrips
        target.AddChild(CreateCurrencyRow(25200)); // White Gatherer Scrips
        target.AddChild(CreateCurrencyRow(28063)); // Skybuilders Scrips
        target.AddChild(new DropdownSeparatorElement { SortIndex = _sortIndex++ });
        target.AddChild(CreateCurrencyRow(26807)); // Bicolor Gemstones
    }

    private void AlignCurrencyRows()
    {
        Element target   = _dropdownElement.Get("Content");
        var     maxWidth = 0;

        foreach (var child in target.Children) {
            if (child is not DropdownSeparatorElement) {
                Element name = child.Get("Name");
                name.ComputeLayout(new(0, 0));
                maxWidth = Math.Max(maxWidth, name.ComputedSize.Width);
            }
        }

        foreach (var child in target.Children) {
            if (child is not DropdownSeparatorElement) {
                child.Get("Name").Size = new(maxWidth + 100, 20);
            }
        }
    }

    private uint GetLimitedTomestoneId()
    {
        return _dataManager.GetExcelSheet<TomestonesItem>()!
            .First(tomestone => tomestone.Tomestones.Row is 3)
            .Item.Row;
    }

    private uint GetNonLimitedTomestoneId()
    {
        return _dataManager.GetExcelSheet<TomestonesItem>()!
            .First(tomestone => tomestone.Tomestones.Row is 2)
            .Item.Row;
    }

    private static unsafe void OnRightClick()
    {
        UIModule* uiModule = UIModule.Instance();
        if (uiModule == null) return;

        uiModule->ExecuteMainCommand(66);
    }
}
