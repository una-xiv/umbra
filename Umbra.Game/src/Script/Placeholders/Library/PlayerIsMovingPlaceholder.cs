using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerIsMovingPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_moving",
    "True if the player is currently moving or being moved, false otherwise."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.IsMoving ? "true" : "false";
    }
}
