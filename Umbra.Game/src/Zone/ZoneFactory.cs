using Map = Lumina.Excel.Sheets.Map;

namespace Umbra.Game;

[Service]
internal sealed class ZoneFactory(
    IDataManager            dataManager,
    WeatherForecastProvider weatherForecastProvider,
    ZoneMarkerFactory       markerFactory,
    IPlayer                 player
) : IDisposable
{
    private readonly Dictionary<uint, Zone> _zoneCache = [];

    public void Dispose()
    {
        _zoneCache.Clear();
    }

    public Zone GetZone(uint zoneId)
    {
        if (_zoneCache.TryGetValue(zoneId, out var cachedZone)) return cachedZone;

        if (null == dataManager.GetExcelSheet<Map>().FindRow(zoneId)) {
            throw new InvalidOperationException($"Zone {zoneId} does not exist");
        }

        var zone = new Zone(dataManager, weatherForecastProvider, markerFactory, player, zoneId);

        _zoneCache[zoneId] = zone;

        return zone;
    }
}
