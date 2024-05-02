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

using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Game;

namespace Umbra;

[Service]
public class UmbraVisibility(IPlayer player)
{
    [ConfigVariable("General.ShowInCutscenes", "General", "VisibilitySettings")]
    public static bool ShowInCutscenes { get; set; } = false;

    [ConfigVariable("General.ShowInGPose", "General", "VisibilitySettings")]
    public static bool ShowInGPose { get; set; } = false;

    [ConfigVariable("General.ShowOnUserHide", "General", "VisibilitySettings")]
    public static bool ShowOnUserHide { get; set; } = false;

    [ConfigVariable("General.ShowBetweenAreas", "General", "VisibilitySettings")]
    public static bool ShowBetweenAreas { get; set; } = false;

    [ConfigVariable("General.ShowDuringIdleCam", "General", "VisibilitySettings")]
    public static bool ShowDuringIdleCam { get; set; } = false;

    [OnTick(interval: 500)]
    internal void OnTick()
    {
        Framework.DalamudPlugin.UiBuilder.DisableCutsceneUiHide = ShowInCutscenes;
        Framework.DalamudPlugin.UiBuilder.DisableGposeUiHide    = ShowInGPose;
        Framework.DalamudPlugin.UiBuilder.DisableUserUiHide     = ShowOnUserHide;
    }

    public bool IsVisible()
    {
        if (player.IsInCutscene && !ShowInCutscenes)
            return false;

        if (player.IsBetweenAreas && !ShowBetweenAreas)
            return false;

        if (player.IsInIdleCam && !ShowDuringIdleCam)
            return false;

        return true;
    }
}
