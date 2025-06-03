using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerNamePlaceholder() : ScriptPlaceholder(
    "player.name",
    "The name of the player."
)
{
    [OnTick]
    public void UpdatePlayerName()
    {
        Value = Framework.Service<IPlayer>().Name;
    }
}
