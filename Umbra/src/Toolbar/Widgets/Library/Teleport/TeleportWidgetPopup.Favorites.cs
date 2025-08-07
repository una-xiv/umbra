using Umbra.Widgets.Popup;

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

        BuildFavoritesButton(destination);
    }

    /// <summary>
    /// Removes the given destination from the favorites list.
    /// </summary>
    private void RemoveFavorite(TeleportDestination destination)
    {
        var id = $"{destination.AetheryteId}:{destination.SubIndex}";
        if (!Favorites.Remove(id)) return;
        
        RemoveFavoritesButton(destination);
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
    }

    /// <summary>
    /// Persist the favorites list to the user config.
    /// </summary>
    private void PersistFavorites()
    {
        ConfigManager.Set("Teleport.Favorites", string.Join(",", Favorites));
    }
}
