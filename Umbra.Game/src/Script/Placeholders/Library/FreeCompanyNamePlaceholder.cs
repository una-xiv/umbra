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
        Value = fc != null ? fc->NameString : "";
    }
}
