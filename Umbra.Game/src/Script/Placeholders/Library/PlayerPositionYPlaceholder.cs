using System.Globalization;
using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerPositionYPlaceholder(IZoneManager zoneManager) : ScriptPlaceholder(
    "player.position.y",
    "The Y (vertical) position of the player in the current zone."
)
{
    [OnTick]
    public void Update()
    {
        Value = zoneManager.HasCurrentZone
            ? zoneManager.CurrentZone.PlayerCoordinates.Y.ToString("N1", CultureInfo.InvariantCulture)
            : "0";
    }
}
