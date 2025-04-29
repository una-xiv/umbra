using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Umbra.Common;

namespace Umbra.Widgets;

internal partial class OnlineStatusWidget
{
    /// <summary>
    /// Returns an OnlineStatus row by its ID.
    /// </summary>
    private static Lumina.Excel.Sheets.OnlineStatus GetStatusById(uint id)
    {
        return Framework.Service<IDataManager>().GetExcelSheet<Lumina.Excel.Sheets.OnlineStatus>()!.GetRow(id)!;
    }

    /// <summary>
    /// Opens the search info window.
    /// </summary>
    /// <remarks>
    /// There is a known issue where the selected status is not updated in this
    /// window until the social window has been opened. This is solely a local
    /// state issue that is deliberately not fixed in this code, as it would
    /// otherwise require sending things to the server to request the updated
    /// status.
    /// </remarks>
    private unsafe void OpenSearchInfoWindow()
    {
        InfoModule* infoModule = InfoModule.Instance();
        if (infoModule->IsInCrossWorldDuty()) return;

        ulong localContentId = infoModule->GetLocalContentId();

        InfoProxyCommonList.CharacterData* characterData =
            InfoProxyPartyMember.Instance()->InfoProxyCommonList.GetEntryByContentId(localContentId);

        if (characterData == null) return;

        var updateDataPacket = Unsafe.AsPointer(ref InfoProxyDetail.Instance()->UpdateData);
        if (null == updateDataPacket) return;

        AgentDetail.Instance()->OpenForCharacterData(characterData, (InfoProxyDetail.UpdateDataPacket*)updateDataPacket);
    }
}
