namespace Umbra.Game.Script;

[Service]
internal class ZoneNamePlaceholder(IZoneManager zoneManager) : ScriptPlaceholder(
    "zone.name",
    "The name of the current zone/map."
)
{
    [OnTick]
    public void Update()
    {
        Value = zoneManager.HasCurrentZone ? zoneManager.CurrentZone.Name : "";
    }
}
