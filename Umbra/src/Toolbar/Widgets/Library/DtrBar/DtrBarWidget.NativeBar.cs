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

using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra.Common;

namespace Umbra.Widgets;

internal unsafe partial class DtrBarWidget
{
    private void UpdateNativeServerInfoBar()
    {
        if (! GetConfigValue<bool>("HideNative")) {
            SetNativeServerInfoBarVisibility(true);
            return;
        }

        SetNativeServerInfoBarVisibility(!(Toolbar.Enabled && Framework.Service<UmbraVisibility>().IsVisible()));
    }

    private void SetNativeServerInfoBarVisibility(bool isVisible)
    {
        var dtrBar = (AtkUnitBase*) _gameGui!.GetAddonByName("_DTR");
        if (dtrBar != null && dtrBar->IsVisible != isVisible) {
            dtrBar->IsVisible = isVisible;
        }
    }
}
