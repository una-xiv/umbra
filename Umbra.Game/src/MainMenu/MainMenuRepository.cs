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
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using Umbra.Common;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace Umbra.Game;

[Service]
internal sealed class MainMenuRepository : IMainMenuRepository
{
    public readonly Dictionary<MenuCategory, MainMenuCategory> Categories = [];

    private readonly IAetheryteList               _aetheryteList;
    private readonly ITravelDestinationRepository _travelDestinationRepository;
    private readonly IPlayer                      _player;

    [ConfigVariable("Toolbar.Widget.MainMenu.Travel.ShowResidential", "ToolbarSettings")]
    private static bool ShowResidentialAreas { get; set; } = false;

    public MainMenuRepository(
        IDataManager                 dataManager,
        IAetheryteList               aetheryteList,
        ITravelDestinationRepository travelDestinationRepository,
        IPlayer                      player
    )
    {
        _aetheryteList               = aetheryteList;
        _travelDestinationRepository = travelDestinationRepository;
        _player                      = player;

        dataManager.GetExcelSheet<MainCommandCategory>()!
            .ToList()
            .ForEach(
                cmd => {
                    if (cmd.Name == "" || null == Enum.GetName(typeof(MenuCategory), cmd.RowId)) return;
                    Categories[(MenuCategory)cmd.RowId] = new((MenuCategory)cmd.RowId, cmd.Name);
                }
            );

        Categories
            .Values.ToList()
            .ForEach(
                category => {
                    dataManager.GetExcelSheet<MainCommand>()!
                        .Where(cmd => cmd.MainCommandCategory?.Row == (uint)category.Category)
                        .ToList()
                        .ForEach(
                            cmd => {
                                category.AddItem(
                                    new(cmd.Name, cmd.SortID, cmd.RowId) { Icon = cmd.Icon > 0 ? (uint)cmd.Icon : null }
                                );
                            }
                        );
                }
            );

        // Add Dalamud items to the system menu.
        Categories[MenuCategory.System].AddItem(new(-998));

        Categories[MenuCategory.System]
            .AddItem(
                new(I18N.Translate("MainMenu.UmbraSettings"), -999, "/umbra")
                    { Icon = SeIconChar.BoxedLetterU, IconColor = 0xFF40A0AC }
            );

        Categories[MenuCategory.System].AddItem(new(-1000));

        Categories[MenuCategory.System]
            .AddItem(
                new(I18N.Translate("MainMenu.DalamudSettings"), -1001, "/xlsettings")
                    { Icon = SeIconChar.BoxedLetterD, IconColor = 0xFF5151FF }
            );

        Categories[MenuCategory.System]
            .AddItem(
                new(I18N.Translate("MainMenu.DalamudPlugins"), -1002, "/xlplugins")
                    { Icon = SeIconChar.BoxedLetterD, IconColor = 0xFF5151FF }
            );
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

        var sortIndex = 900;

        foreach (var dest in _travelDestinationRepository.Destinations) {
            if (dest.IsHousing && !ShowResidentialAreas) continue;

            var key = $"Aetheryte:{dest.Id}:{dest.SubId}";
            usedNames.Add(key);

            var existingItem = category.Items.FirstOrDefault(item => item.MetadataKey == key);

            if (category.Items.All(item => item.Type != MainMenuItemType.Separator)) {
                category.AddItem(new((short)sortIndex));
                sortIndex++;
            }

            bool isDisabled = !_player.CanUseTeleportAction
             || (dest.IsHousing && _player.CurrentWorldName != _player.HomeWorldName);

            var gilCost = $"{dest.GilCost} gil";

            if (existingItem != null) {
                if (existingItem.Name != dest.Name) {
                    existingItem.Name = dest.Name;
                }

                if (existingItem.ShortKey != gilCost) {
                    existingItem.ShortKey = gilCost;
                }

                existingItem.IsDisabled = isDisabled;
                continue;
            }

            MainMenuItem item = new(
                dest.Name,
                (short)(sortIndex + (dest.IsHousing ? 10 : 0)),
                () => {
                    unsafe {
                        Telepo.Instance()->Teleport(dest.Id, (byte)dest.SubId);
                    }
                }
            ) {
                MetadataKey = key,
                IsDisabled  = isDisabled,
                ShortKey    = gilCost,
            };

            category.AddItem(item);
            sortIndex++;
        }

        category
            .Items.ToList()
            .ForEach(
                item => {
                    if (item.MetadataKey != null && !usedNames.Contains(item.MetadataKey)) {
                        category.RemoveItem(item);
                    }
                }
            );

        if (usedNames.Count == 0 && category.Items.Any(item => item.Type == MainMenuItemType.Separator)) {
            category.RemoveItem(category.Items.First(item => item.Type == MainMenuItemType.Separator));
        }
    }

    private List<AetheryteEntry> GetFavoredDestinations()
    {
        List<AetheryteEntry> result = [];

        foreach (AetheryteEntry aetheryte in _aetheryteList) {
            if (aetheryte.IsFavourite) {
                result.Add(aetheryte);
            }
        }

        return result;
    }

    private List<AetheryteEntry> GetHousingDestinations()
    {
        List<AetheryteEntry> result = [];

        foreach (AetheryteEntry aetheryte in _aetheryteList) {
            if (aetheryte.IsSharedHouse || aetheryte.IsAppartment) {
                result.Add(aetheryte);
            }
        }

        return result;
    }
}
