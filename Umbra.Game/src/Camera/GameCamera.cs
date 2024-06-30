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

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using Umbra.Common;
using CameraManager = FFXIVClientStructs.FFXIV.Client.Graphics.Scene.CameraManager;

namespace Umbra.Game;

[Service]
internal sealed class GameCamera : IGameCamera, IDisposable
{
    public GameCamera(ISigScanner sigScanner, IGameGui gameGui) { }

    public void Dispose() { }
    //
    // [OnDraw]
    // private void OnDraw()
    // {
    //     ImGui.Begin("DebugShit");
    //
    //     if (ImGui.InputInt("Offset", ref _offset, 1, 2)) { }
    //
    //     ImGui.TextUnformatted($"Hex: 0x{_offset:X}");
    //     ImGui.TextUnformatted($"Last pos: {_lastScreenPos.X}, {_lastScreenPos.Y}");
    //
    //     ImGui.End();
    // }

    public unsafe bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos)
    {
        CameraManager* cmPtr = CameraManager.Instance();

        if (cmPtr == null) {
            screenPos = new();
            return false;
        }

        Camera* camera  = cmPtr->CurrentCamera;
        Vector2 size    = ImGui.GetIO().DisplaySize;
        Vector3 pCoords = Vector3.Transform(worldPos, camera->ViewMatrix * camera->RenderCamera->ProjectionMatrix);

        float width  = size.X;
        float height = size.Y;

        screenPos   = new(pCoords.X / MathF.Abs(pCoords.Z), pCoords.Y / MathF.Abs(pCoords.Z));
        screenPos.X = 0.5f * width * (screenPos.X + 1.0f);
        screenPos.Y = 0.5f * height * (1.0f - screenPos.Y);

        return pCoords.Z > 0.0f;
    }
}
