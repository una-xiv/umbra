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

using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace Umbra.Widgets;

public unsafe partial class MailIndicatorWidget
{
    /// <summary>
    /// Returns true if the player has unread mail.
    /// </summary>
    /// <returns></returns>
    private uint GetUnreadMailCount()
    {
        var ipl = (InfoProxyLetterCount*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.Letter);

        return ipl->NumLetters;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InfoProxyLetterCount
    {
        [FieldOffset(0x26)] public byte NumLetters;
    }
}
