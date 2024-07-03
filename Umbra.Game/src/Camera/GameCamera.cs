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
        var cameraManager = CameraManager.Instance();
        screenPos = Vector2.Zero;

        if (cameraManager == null)
            return false;

        var worldPosPointer = new FFXIVClientStructs.FFXIV.Common.Math.Vector3(worldPos.X, worldPos.Y, worldPos.Z);
        var screenPosPointer = new FFXIVClientStructs.FFXIV.Common.Math.Vector2();

        Camera.WorldToScreenPoint(&screenPosPointer, &worldPosPointer);
        screenPos = new Vector2(screenPosPointer.X, screenPosPointer.Y);

        var currentCamera = cameraManager->CurrentCamera;

        if (currentCamera == null)
            return false;

        _lookAtVector = currentCamera->LookAtVector;
        _cameraPosition = currentCamera->Position;

        var imguiDisplaySize = ImGui.GetIO().DisplaySize;

        if (screenPosPointer.X < 0 || screenPosPointer.X > imguiDisplaySize.X ||
            screenPosPointer.Y < 0 || screenPosPointer.Y > imguiDisplaySize.Y)
        {
            return false;
        }

        return IsInFrontOfCamera(worldPos);
    }

    public bool IsInFrontOfCamera(Vector3 worldPos)
    {
        Vector3 cameraDirection = Vector3.Normalize(_lookAtVector - _cameraPosition);
        Vector3 toPoint         = worldPos - _cameraPosition;

        return Vector3.Dot(cameraDirection, toPoint) > 0;
    }
}
