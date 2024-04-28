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

namespace Umbra;

[Service]
public class UmbraVisibility
{
    [ConfigVariable("General.ShowInCutscenes", "General")]
    public static bool ShowInCutscenes { get; set; } = false;

    [ConfigVariable("General.ShowInGPose", "General")]
    public static bool ShowInGPose { get; set; } = false;

    [ConfigVariable("General.ShowOnUserHide", "General")]
    public static bool ShowOnUserHide { get; set; } = false;

    [OnTick(interval: 1000)]
    internal void OnTick()
    {
        Framework.DalamudPlugin.UiBuilder.DisableCutsceneUiHide = ShowInCutscenes;
        Framework.DalamudPlugin.UiBuilder.DisableGposeUiHide    = ShowInGPose;
        Framework.DalamudPlugin.UiBuilder.DisableUserUiHide     = ShowOnUserHide;
    }
}
