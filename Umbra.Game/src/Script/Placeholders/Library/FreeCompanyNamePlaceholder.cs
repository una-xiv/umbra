using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace Umbra.Game.Script;

[Service]
internal class FreeCompanyNamePlaceholder() : ScriptPlaceholder(
    "fc.name",
    "The name of the free company the player is a member of."
)
{
    [OnTick]
    public unsafe void Update()
    {
        var fc = InfoProxyFreeCompany.Instance();

        // While world visiting or during duty, fc info is not available.
        // Currently, the NameString property also does not update when returning from a duty.
        // This also keeps the placeholder from being updated when the player leaves their fc.
        if (fc == null || fc->Id == 0 || fc->NameString.IsNullOrEmpty())
        {
            return;
        }

        Value = fc->NameString;
    }
}
