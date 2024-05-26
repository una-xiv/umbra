/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Umbra.Common;

namespace Umbra.Widgets;

public partial class OnlineStatusWidget
{
    /// <summary>
    /// Returns an OnlineStatus row by its ID.
    /// </summary>
    private static Lumina.Excel.GeneratedSheets.OnlineStatus GetStatusById(uint id)
    {
        return Framework.Service<IDataManager>().GetExcelSheet<Lumina.Excel.GeneratedSheets.OnlineStatus>()!.GetRow(id)!;
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
            InfoProxyParty.Instance()->InfoProxyCommonList.GetEntryByContentId(localContentId);

        if (characterData == null) return;

        var updateDataPacket = Unsafe.AsPointer(ref InfoProxySearchComment.Instance()->UpdateData);
        if (null == updateDataPacket) return;

        AgentDetail.Instance()->OpenForCharacterData(characterData, (InfoProxySearchComment.UpdateDataPacket*)updateDataPacket);
    }
}
