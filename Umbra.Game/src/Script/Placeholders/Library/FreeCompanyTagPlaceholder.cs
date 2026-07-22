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
        // The fc tag is not available in some cases e.g. world visiting or between areas.
        // This also keeps the placeholder from being updated when the player leaves their fc.
        if (objectTable.LocalPlayer is not ICharacter c || string.IsNullOrEmpty(c.CompanyTag.TextValue))
        {
            return;
        }

        Value = c.CompanyTag.TextValue;
    }
}
