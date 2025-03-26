/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class ChatSender : IChatSender
{
    public unsafe void Send(string message)
    {
        // Conversion from 0x27F:
        const AllowedEntities combinedEntities =
            AllowedEntities.UppercaseLetters |
            AllowedEntities.LowercaseLetters |
            AllowedEntities.Numbers |
            AllowedEntities.SpecialCharacters |
            AllowedEntities.CharacterList |
            AllowedEntities.OtherCharacters |
            AllowedEntities.Payloads |
            AllowedEntities.Unknown8 |
            AllowedEntities.Unknown9;

        using var msg = new Utf8String(message);
        msg.SanitizeString(combinedEntities, null);
        UIModule.Instance()->ProcessChatBoxEntry(&msg);
    }
}
