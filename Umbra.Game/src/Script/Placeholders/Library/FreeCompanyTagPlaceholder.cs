using Dalamud.Game.ClientState.Objects.Types;

namespace Umbra.Game.Script;

[Service]
internal class FreeCompanyTagPlaceholder(IObjectTable objectTable) : ScriptPlaceholder(
    "fc.tag",
    "The free company tag of the player."
)
{
    [OnTick]
    public void Update()
    {
        Value = objectTable.LocalPlayer is ICharacter c ? c.CompanyTag.TextValue : "";
    }
}
