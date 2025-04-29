using System.Numerics;

namespace Umbra.Game;

public readonly struct ZoneMarker(
    ZoneMarkerType type,
    string         name,
    Vector2        position,
    Vector3        worldPosition,
    uint           iconId,
    uint           dataId,
    float          radius = 0.0f
)
{
    public readonly ZoneMarkerType Type          = type;
    public readonly string         Name          = name;
    public readonly Vector2        Position      = position;
    public readonly Vector3        WorldPosition = worldPosition;
    public readonly uint           IconId        = iconId;
    public readonly uint           DataId        = dataId;
    public readonly float          Radius        = radius;
}
