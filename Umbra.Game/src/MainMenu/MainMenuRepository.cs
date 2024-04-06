/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
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
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace Umbra.Game;

[Service]
public sealed class MainMenuRepository : IMainMenuRepository
{
    public readonly Dictionary<MenuCategory, MainMenuCategory> Categories = [];

    private readonly IAetheryteList _aetheryteList;
    private readonly Player _player;

    public MainMenuRepository(IDataManager dataManager, IAetheryteList aetheryteList, Player player)
    {
        _aetheryteList = aetheryteList;
        _player        = player;

        dataManager.GetExcelSheet<MainCommandCategory>()!.ToList().ForEach(cmd => {
            if (cmd.Name == "" || null == Enum.GetName(typeof(MenuCategory), cmd.RowId)) return;
            Categories[(MenuCategory)cmd.RowId] = new ((MenuCategory)cmd.RowId, cmd.Name);
        });

        Categories.Values.ToList().ForEach(category => {
            dataManager.GetExcelSheet<MainCommand>()!.Where(cmd => cmd.MainCommandCategory?.Row == (uint)category.Category).ToList().ForEach(cmd => {
                category.AddItem(new (cmd.Name, cmd.SortID, cmd.RowId) { Icon = cmd.Icon > 0 ? (uint)cmd.Icon : null});
            });
        });

        // Add Dalamud items to the system menu.
        Categories[MenuCategory.System].AddItem(new (-998));
        Categories[MenuCategory.System].AddItem(new (I18N.Translate("MainMenu.UmbraSettings"), -999, "/umbra") { Icon = SeIconChar.BoxedLetterU, IconColor = 0xFF40A0AC });
        Categories[MenuCategory.System].AddItem(new (-1000));
        Categories[MenuCategory.System].AddItem(new (I18N.Translate("MainMenu.DalamudSettings"), -1001, "/xlsettings") { Icon = SeIconChar.BoxedLetterD, IconColor = 0xFF5151FF });
        Categories[MenuCategory.System].AddItem(new (I18N.Translate("MainMenu.DalamudPlugins"), -1002, "/xlplugins") { Icon = SeIconChar.BoxedLetterD, IconColor = 0xFF5151FF });
    }

    public List<MainMenuCategory> GetCategories()
    {
        return [.. Categories.Values];
    }

    public MainMenuCategory GetCategory(MenuCategory category)
    {
        return Categories.GetValueOrDefault(category)
            ?? throw new Exception($"Category {category} not found.");
    }

    [OnTick(interval: 500)]
    public void OnTick()
    {
        SyncTravelDestinations();

        foreach (var category in Categories) {
            category.Value.Update();
        }
    }

    private void SyncTravelDestinations()
    {
        if (!Categories.TryGetValue(MenuCategory.Travel, out var category)) return;

        List<string> usedNames = [];

        short sortIndex = 900;

        for (int i = 0; i < _aetheryteList.Length; i++) {
            var aetheryte = _aetheryteList[i];
            if (null == aetheryte) continue;

            if (aetheryte.IsFavourite) {
                var info = aetheryte.AetheryteData.GameData;
                if (null == info) continue;

                var name = $"{info.PlaceName.Value?.Name ?? "???"} ({aetheryte.GilCost} gil)";
                var key  = $"Aetheryte:{aetheryte.AetheryteId}";
                usedNames.Add(key);

                var existingItem = category.Items.FirstOrDefault(item => item.MetadataKey == key);
                if (existingItem != null) {
                    if (existingItem.Name != name) {
                        existingItem.Name = name;
                    }

                    existingItem.IsDisabled = !_player.CanUseTeleportAction;
                    continue;
                }

                if (category.Items.All(item => item.Type != MainMenuItemType.Separator)) {
                    category.AddItem(new(sortIndex));
                    sortIndex++;
                }

                category.AddItem(new(name, sortIndex, () => {
                    unsafe {
                        Telepo.Instance()->Teleport(aetheryte.AetheryteId, aetheryte.SubIndex);
                    }
                }) {
                    MetadataKey = $"Aetheryte:{aetheryte.AetheryteId}",
                    IsDisabled = !_player.CanUseTeleportAction
                } );
                sortIndex++;
            }
        }

        category.Items.ToList().ForEach(item => {
            if (item.MetadataKey != null && !usedNames.Contains(item.MetadataKey)) {
                category.RemoveItem(item);
            }
        });

        if (usedNames.Count == 0 && category.Items.Any(item => item.Type == MainMenuItemType.Separator)) {
            category.RemoveItem(category.Items.First(item => item.Type == MainMenuItemType.Separator));
        }
    }
}
