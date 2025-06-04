using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class ZoneDistrictPlaceholder(IZoneManager zoneManager) : ScriptPlaceholder(
    "zone.district",
    "The district name of the current zone/map."
)
{
    [OnTick]
    public void Update()
    {
        Value = zoneManager.HasCurrentZone ? zoneManager.CurrentZone.CurrentDistrictName : "";
    }
}
