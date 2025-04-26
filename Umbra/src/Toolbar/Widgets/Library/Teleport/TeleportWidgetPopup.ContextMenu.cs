using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private TeleportDestination? _selectedDestination;
    
    private void BuildContextMenu()
    {
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
    }

    private void OpenContextMenu(TeleportDestination destination, bool showSortables = false)
    {
        _selectedDestination = destination;
        
        ContextMenu!.SetEntryVisible("AddFav", !IsFavorite(destination));
        ContextMenu!.SetEntryVisible("DelFav", IsFavorite(destination));
        ContextMenu!.SetEntryVisible("MoveUp", showSortables);
        ContextMenu!.SetEntryVisible("MoveDown", showSortables);

        if (showSortables && IsFavorite(destination)) {
            var indexAt = Favorites.IndexOf($"{destination.AetheryteId}:{destination.SubIndex}");
            ContextMenu!.SetEntryDisabled("MoveUp", indexAt == 0);
            ContextMenu!.SetEntryDisabled("MoveDown", indexAt == Favorites.Count - 1);
        }
        
        ContextMenu!.Present();
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

        var id    = $"{_selectedDestination.Value.AetheryteId}:{_selectedDestination.Value.SubIndex}";
        var index = Favorites.IndexOf(id);
        if (index == -1) return;

        var newIndex = index + direction;
        if (newIndex < 0 || newIndex >= Favorites.Count) return;

        Favorites.RemoveAt(index);
        Favorites.Insert(newIndex, id);
        PersistFavorites();
        UpdateFavoriteSortIndices();
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
