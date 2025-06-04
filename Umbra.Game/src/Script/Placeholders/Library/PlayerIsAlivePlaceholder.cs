using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerIsAlivePlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_alive",
    "True if the player is alive, false otherwise."
)
{
    [OnTick]
    public void Update()
    {
        Value = !player.IsDead ? "true" : "false";
    }
}
