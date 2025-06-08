using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerLevelPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.level",
    "The level of the current job of the player."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.Level.ToString();
    }
}
