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

using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.System;

[Service]
internal class StreamSphereOverride(IPlayer player)
{
    /// <summary>
    /// Overrides the collision stream sphere to be larger. This ensures that
    /// raycast operations across larger distances are more accurate.
    /// </summary>
    [OnTick]
    public unsafe void OverrideCollisionStreamSphere()
    {
        if (player.IsBetweenAreas || player.IsInQuestEvent) return;

        var fw = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        if (fw == null) return;

        var bc = fw->BGCollisionModule;
        if (bc == null) return;

        bc->ForcedStreamingSphere.W = 4000;
    }
}
