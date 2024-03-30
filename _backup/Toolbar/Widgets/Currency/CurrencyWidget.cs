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

using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Drawing.Prefabs;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class CurrencyWidget : IToolbarWidget
{
    [ConfigVariable(
        "Toolbar.Widget.Currency.Enabled",
        "Toolbar Widgets",
        "Show currency widget",
        "Displays a widget that allows you to open the currency window."
    )]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    private readonly Player       _player;
    private readonly IDataManager _dataManager;

    public unsafe CurrencyWidget(Player player, IDataManager dataManager, ToolbarDropdownContext context)
    {
        _player      = player;
        _dataManager = dataManager;

        BuildCurrencyItemList();
        context.RegisterDropdownActivator(Element, _dropdownElement);

        Element.OnRightClick += () => {
            UIModule* uiModule = UIModule.Instance();
            if (uiModule == null) return;

            uiModule->ExecuteMainCommand(66);
        };
    }

    [OnDraw]
    public void OnDraw()
    {
        Element.IsVisible = Enabled;
        if (!Enabled) return;

        Element.Get("Info.Sub").GetNode<TextNode>().Text = _player.GetItemCount(1).ToString("N0");

        if (!_dropdownElement.IsVisible) return;

        UpdateCurrencyValue(1);
        UpdateCurrencyValue(29);

        if (_player.GrandCompanyId == 1) UpdateCurrencyValue(20);
        if (_player.GrandCompanyId == 2) UpdateCurrencyValue(21);
        if (_player.GrandCompanyId == 3) UpdateCurrencyValue(22);

        UpdateCurrencyValue(27);
        UpdateCurrencyValue(10307);
        UpdateCurrencyValue(26533);
        UpdateCurrencyValue(GetLimitedTomestoneId());
        UpdateCurrencyValue(GetNonLimitedTomestoneId());
        UpdateCurrencyValue(28);
        UpdateCurrencyValue(33913);
        UpdateCurrencyValue(33914);
        UpdateCurrencyValue(25199);
        UpdateCurrencyValue(25200);
        UpdateCurrencyValue(28063);
        UpdateCurrencyValue(26807);
    }

    private void BuildCurrencyItemList()
    {
        var target = _dropdownElement.Content.Get("CurrencyList");

        target.AddChild(CreateCurrencyRow(1));  // Gil
        target.AddChild(CreateCurrencyRow(29)); // MGP

        if (_player.GrandCompanyId == 1) target.AddChild(CreateCurrencyRow(20)); // Maelstrom
        if (_player.GrandCompanyId == 2) target.AddChild(CreateCurrencyRow(21)); // Twin Adder
        if (_player.GrandCompanyId == 3) target.AddChild(CreateCurrencyRow(22)); // Immortal Flames

        target.AddChild(new DropdownSeparatorElement());
        target.AddChild(CreateCurrencyRow(27));    // Allied Seals
        target.AddChild(CreateCurrencyRow(10307)); // Centurio Seals
        target.AddChild(CreateCurrencyRow(26533)); // Sacks of Nuts
        target.AddChild(new DropdownSeparatorElement());
        target.AddChild(CreateCurrencyRow(GetLimitedTomestoneId()));    // Limited Tomestones
        target.AddChild(CreateCurrencyRow(GetNonLimitedTomestoneId())); // Non-limited Tomestones
        target.AddChild(CreateCurrencyRow(28));                         // Poetics
        target.AddChild(new DropdownSeparatorElement());
        target.AddChild(CreateCurrencyRow(33913)); // Purple Crafter Scrips
        target.AddChild(CreateCurrencyRow(33914)); // Purple Gatherer Scrips
        target.AddChild(CreateCurrencyRow(25199)); // White Crafter Scrips
        target.AddChild(CreateCurrencyRow(25200)); // White Gatherer Scrips
        target.AddChild(CreateCurrencyRow(28063)); // Skybuilders Scrips
        target.AddChild(new DropdownSeparatorElement());
        target.AddChild(CreateCurrencyRow(26807)); // Bicolor Gemstones
    }

    private void UpdateCurrencyValue(uint itemId)
    {
        _dropdownElement
            .Content
            .Get($"CurrencyList.CurrencyRow_{itemId}.Amount")
            .GetNode<TextNode>()
            .Text = _player.GetItemCount(itemId).ToString("N0");
    }

    private uint GetLimitedTomestoneId()
    {
        return _dataManager.GetExcelSheet<TomestonesItem>()!
            .Where(tomestone => tomestone.Tomestones.Row is 3)
            .First()
            .Item.Row;
    }

    private uint GetNonLimitedTomestoneId()
    {
        return _dataManager.GetExcelSheet<TomestonesItem>()!
            .Where(tomestone => tomestone.Tomestones.Row is 2)
            .First()
            .Item.Row;
    }
}
