namespace Umbra.Game;

public interface IZoneManager
{
    public event Action<IZone> ZoneChanged;

    public bool HasCurrentZone { get; }

    public IZone CurrentZone { get; }

    public IZone GetZone(uint zoneId);
}
