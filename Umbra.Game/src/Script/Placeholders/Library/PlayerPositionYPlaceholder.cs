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
        Value = I18N.FormatNumber(zoneManager.CurrentZone.PlayerCoordinates.Y);
    }
}
