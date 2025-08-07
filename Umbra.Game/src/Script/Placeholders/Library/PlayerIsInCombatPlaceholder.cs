namespace Umbra.Game.Script;

[Service]
internal class PlayerIsInCombatPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_in_combat",
    "True if the player is currently in combat, false otherwise."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.IsInCombat ? "true" : "false";
    }
}
