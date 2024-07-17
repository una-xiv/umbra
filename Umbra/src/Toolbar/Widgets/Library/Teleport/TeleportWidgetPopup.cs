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

using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup : WidgetPopup, IDisposable
{
    public int    MinimumColumns        { get; set; } = 1;
    public string ExpansionMenuPosition { get; set; } = "Auto";
    public bool   ShowNotification      { get; set; }

    private string               _selectedExpansion = string.Empty;
    private TeleportDestination? _selectedDestination;

    public TeleportWidgetPopup()
    {
        ConfigManager.CvarChanged += OnCvarChanged;
        LoadFavorites();
    }

    public void Dispose()
    {
        ConfigManager.CvarChanged -= OnCvarChanged;
    }

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        return Framework.Service<IPlayer>().CanUseTeleportAction;
    }

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        HydrateAetherytePoints();
        BuildNodes();
        LoadFavorites();
        ActivateExpansion(_selectedExpansion, true);

        ContextMenu = new(
            [
                new("Teleport") {
                    Label   = I18N.Translate("Widget.Teleport.Name"),
                    IconId  = 111,
                    OnClick = ContextMenuTeleport
                },
                new("ShowMap") {
                    Label   = I18N.Translate("Widget.Teleport.OpenMap"),
                    OnClick = ContextMenuShowOnMap
                },
                new("AddFav") {
                    Label   = I18N.Translate("Widget.Teleport.Favorites.Add"),
                    OnClick = ContextMenuAddToFavorites
                },
                new("DelFav") {
                    Label   = I18N.Translate("Widget.Teleport.Favorites.Remove"),
                    OnClick = ContextMenuRemoveFromFavorites
                },
                new("MoveUp") {
                    Label   = I18N.Translate("Widget.Teleport.Favorites.MoveUp"),
                    OnClick = () => ContextMenuMoveFavorite(-1),
                },
                new("MoveDown") {
                    Label   = I18N.Translate("Widget.Teleport.Favorites.MoveDown"),
                    OnClick = () => ContextMenuMoveFavorite(1),
                }
            ]
        );

        Node.QuerySelector("#DestinationList")!.TagsList.Add(ExpansionMenuPosition == "Left" ? "right" : "left");
        Node.QuerySelector("#ExpansionList")!.TagsList.Add(ExpansionMenuPosition == "Left" ? "left" : "right");

        foreach (var node in Node.FindById("ExpansionList")!.QuerySelectorAll(".expansion")) {
            node.Style.RoundedCorners = ExpansionMenuPosition == "Left"
                ? RoundedCorners.TopLeft | RoundedCorners.BottomLeft
                : RoundedCorners.TopRight | RoundedCorners.BottomRight;
        }
    }

    /// <inheritdoc/>
    protected override void OnClose()
    {
        ExpansionLists.Clear();

        _expansions.Clear();
        _selectedExpansion = string.Empty;
    }

    private void ActivateExpansion(string key, bool force = false)
    {
        if (!force && key == _selectedExpansion) return;

        if (_selectedExpansion != string.Empty) {
            Node.FindById("ExpansionList")!.QuerySelector(_selectedExpansion)!.TagsList.Remove("selected");
            Node.FindById("DestinationList")!.QuerySelector(_selectedExpansion)!.Style.IsVisible = false;
        }

        _selectedExpansion = key;
        Node.FindById("ExpansionList")!.QuerySelector(key)!.TagsList.Add("selected");
        Node.FindById("DestinationList")!.QuerySelector(key)!.Style.IsVisible = true;
    }

    private void ContextMenuTeleport()
    {
        if (null == _selectedDestination) return;
        Teleport(_selectedDestination.Value);
    }

    private unsafe void ContextMenuShowOnMap()
    {
        if (null == _selectedDestination) return;

        AgentMap* am = AgentMap.Instance();
        am->ShowMap(true, false);

        OpenMapInfo info = new() {
            Type        = MapType.Teleport,
            MapId       = _selectedDestination.Value.MapId,
            TerritoryId = _selectedDestination.Value.TerritoryId,
        };

        am->OpenMap(&info);
    }

    private void ContextMenuAddToFavorites()
    {
        if (null == _selectedDestination) return;
        AddFavorite(_selectedDestination.Value);
    }

    private void ContextMenuRemoveFromFavorites()
    {
        if (null == _selectedDestination) return;
        RemoveFavorite(_selectedDestination.Value);
    }

    private void ContextMenuMoveFavorite(int direction)
    {
        if (null == _selectedDestination) return;

        var id = $"{_selectedDestination.Value.AetheryteId}:{_selectedDestination.Value.SubIndex}";
        var index = Favorites.IndexOf(id);
        if (index == -1) return;

        var newIndex = index + direction;
        if (newIndex < 0 || newIndex >= Favorites.Count) return;

        Favorites.RemoveAt(index);
        Favorites.Insert(newIndex, id);
        PersistFavorites();
    }

    private void Teleport(TeleportDestination destination)
    {
        if (!Framework.Service<IPlayer>().CanUseTeleportAction) return;

        if (ShowNotification) {
            Framework
                .Service<IToastGui>()
                .ShowQuest(
                    $"{I18N.Translate("Widget.Teleport.Name")}: {destination.Name}",
                    new() { IconId = 111, PlaySound = true, DisplayCheckmark = false }
                );
        }

        unsafe {
            Telepo.Instance()->Teleport(destination.AetheryteId, destination.SubIndex);
        }

        Close();
    }
}
