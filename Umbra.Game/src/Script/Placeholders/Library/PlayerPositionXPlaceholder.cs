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
        Value = I18N.FormatNumber(zoneManager.CurrentZone.PlayerCoordinates.X);
    }
}
