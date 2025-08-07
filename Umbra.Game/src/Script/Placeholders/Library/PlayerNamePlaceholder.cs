namespace Umbra.Game.Script;

[Service]
internal class PlayerNamePlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.name",
    "The name of the player."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.Name;
    }
}
