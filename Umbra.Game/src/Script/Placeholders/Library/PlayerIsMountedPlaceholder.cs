using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerIsMountedPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_mounted",
    "True if the player is currently mounted, false otherwise."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.IsMounted ? "true" : "false";
    }
}
