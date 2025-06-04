using System.Globalization;
using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerPositionXPlaceholder(IZoneManager zoneManager) : ScriptPlaceholder(
    "player.position.x",
    "The X (horizontal) position of the player in the current zone."
)
{
    [OnTick]
    public void Update()
    {
        Value = zoneManager.HasCurrentZone
            ? zoneManager.CurrentZone.PlayerCoordinates.X.ToString("N1", CultureInfo.InvariantCulture)
            : "0";
    }
}
