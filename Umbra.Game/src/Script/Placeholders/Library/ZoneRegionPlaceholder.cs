using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class ZoneRegionPlaceholder(IZoneManager zoneManager) : ScriptPlaceholder(
    "zone.region",
    "The region name of the current zone/map."
)
{
    [OnTick]
    public void Update()
    {
        Value = zoneManager.HasCurrentZone ? zoneManager.CurrentZone.RegionName : "";
    }
}
