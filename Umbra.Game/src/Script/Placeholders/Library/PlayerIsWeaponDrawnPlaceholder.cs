using Umbra.Common;

namespace Umbra.Game.Script;

[Service]
internal class PlayerIsWeaponDrawnPlaceholder(IPlayer player) : ScriptPlaceholder(
    "player.is_weapon_drawn",
    "True if the player has its weapon drawn, false otherwise."
)
{
    [OnTick]
    public void Update()
    {
        Value = player.IsWeaponDrawn ? "true" : "false";
    }
}
