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

using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class TeleportWidgetPopup
{
    [ConfigVariable("Teleport.Favorites")]
    private static string FavoritesData { get; set; } = "";

    private List<string> Favorites { get; } = [];

    /// <summary>
    /// Adds the given destination to the favorites list.
    /// </summary>
    private void AddFavorite(TeleportDestination destination)
    {
        var id = $"{destination.AetheryteId}:{destination.SubIndex}";
        if (Favorites.Contains(id)) return;

        Favorites.Add(id);
        PersistFavorites();
    }

    /// <summary>
    /// Removes the given destination from the favorites list.
    /// </summary>
    private void RemoveFavorite(TeleportDestination destination)
    {
        var id = $"{destination.AetheryteId}:{destination.SubIndex}";
        if (!Favorites.Contains(id)) return;

        Favorites.Remove(id);
        PersistFavorites();
    }

    /// <summary>
    /// Returns true if the given destination is a favorite.
    /// </summary>
    private bool IsFavorite(TeleportDestination destination)
    {
        return Favorites.Contains($"{destination.AetheryteId}:{destination.SubIndex}");
    }

    /// <summary>
    /// Invoked when a cvar is changed. This is invoked when the user makes
    /// changes to their config profile.
    /// </summary>
    private void OnCvarChanged(string cvarName)
    {
        if (cvarName != "Teleport.Favorites") return;
        LoadFavorites();
    }

    /// <summary>
    /// Loads the favorites from the config.
    /// </summary>
    private void LoadFavorites()
    {
        Favorites.Clear();

        if (FavoritesData.Length > 0) {
            foreach (var favorite in FavoritesData.Split(',')) {
                if (favorite.Contains(':') == false) continue;
                if (!_destinations.ContainsKey(favorite)) continue;
                if (Favorites.Contains(favorite)) continue;

                Favorites.Add(favorite);
            }
        }

        Node? menuItemNode = Node.QuerySelector("#ExpansionList > #Favorites");
        Node? menuListNode = Node.QuerySelector("#DestinationList > #Favorites > .region > .favorite-destinations");
        if (menuItemNode != null) {
            menuItemNode.Style.IsVisible = Favorites.Count > 0;
        }

        if (menuListNode != null) {
            menuListNode.ChildNodes = [];
            var index = 0;
            foreach (var favId in Favorites) {
                if (_destinations.TryGetValue(favId, out var dst)) {
                    BuildDestinationNode(menuListNode, dst, index++, true);
                }
            }
        }

        if (Favorites.Count == 0 && _selectedExpansion == "Favorites") {
            ActivateExpansion(_expansions.Keys.First(), true);
        }
    }

    /// <summary>
    /// Persist the favorites list to the user config.
    /// </summary>
    private void PersistFavorites()
    {
        ConfigManager.Set("Teleport.Favorites", string.Join(",", Favorites));
    }
}
