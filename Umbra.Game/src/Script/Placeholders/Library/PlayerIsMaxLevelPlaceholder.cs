using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerIsMaxLevelPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_max_level",
    "True if the player is at the maximum level, otherwise false."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.IsMaxLevel ? "true" : "false";
    }
}
