namespace Umbra.Game.Script;

[Service]
internal class PlayerIsDeadPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_dead",
    "True if the player is dead, false otherwise."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.IsDead ? "true" : "false";
    }
}
