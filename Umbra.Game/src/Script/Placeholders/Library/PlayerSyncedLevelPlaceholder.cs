namespace Umbra.Game.Script;

[Service]
internal class PlayerSyncedLevelPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.level.synced",
    "The synced level of the current job of the player. 0 if not in synced content."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.SyncedLevel.ToString();
    }
}
