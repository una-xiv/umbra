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

using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class GameCamera : IGameCamera
{
    private Vector3 _cameraPosition;
    private Vector3 _lookAtVector;

    public unsafe bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos)
    {
        CameraManager* cmPtr = CameraManager.Instance();

        if (cmPtr == null) {
            screenPos = new();
            return false;
        }

        FFXIVClientStructs.FFXIV.Common.Math.Vector2 v2 = new();
        FFXIVClientStructs.FFXIV.Common.Math.Vector3 v3 = new(worldPos.X, worldPos.Y, worldPos.Z);
        Camera.WorldToScreenPoint(&v2, &v3);
        screenPos = new(v2.X, v2.Y);

        _lookAtVector   = cmPtr->CurrentCamera->LookAtVector;
        _cameraPosition = cmPtr->CurrentCamera->Position;

        bool isInViewport = screenPos.X >= 0
            && screenPos.X <= ImGui.GetIO().DisplaySize.X
            && screenPos.Y >= 0
            && screenPos.Y <= ImGui.GetIO().DisplaySize.Y;

        return isInViewport && IsInFrontOfCamera(worldPos);
    }

    public bool IsInFrontOfCamera(Vector3 worldPos)
    {
        Vector3 cameraDirection = Vector3.Normalize(_lookAtVector - _cameraPosition);
        Vector3 toPoint         = worldPos - _cameraPosition;

        return Vector3.Dot(cameraDirection, toPoint) > 0;
    }
}
