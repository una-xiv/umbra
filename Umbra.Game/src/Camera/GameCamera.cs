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
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class GameCamera : IGameCamera, IDisposable
{
    public GameCamera(ISigScanner sigScanner, IGameGui gameGui) { }

    public void Dispose() { }

    //
    [OnDraw]
    private unsafe void OnDraw()
    {
        // CameraManager* cmPtr  = CameraManager.Instance();
        // Camera*        camera = cmPtr->CurrentCamera;
        //
        // ImGui.Begin("DebugShit");
        //
        // ImGui.TextUnformatted($"Input         : {_lastWorldPos}");
        // ImGui.TextUnformatted($"WorldToScreen : {_worldToScreenResult}");
        // ImGui.TextUnformatted($"ScreenToWorld : {_screenToWorldResult}");
        // ImGui.TextUnformatted($"LookAtVector  : {_lookAtVector}");
        // ImGui.TextUnformatted($"CameraPosition: {_cameraPosition}");
        //
        // ImGui.TextUnformatted("");
        // ImGui.TextUnformatted($"FoV      : {camera->RenderCamera->FoV}");
        // ImGui.TextUnformatted($"NearClip : {camera->RenderCamera->NearPlane}");
        // ImGui.TextUnformatted($"FarClip  : {camera->RenderCamera->FarPlane}");
        //
        // ImGui.End();
    }

    public unsafe bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos)
    {
        CameraManager* cmPtr = CameraManager.Instance();

        if (cmPtr == null) {
            screenPos = new();
            return false;
        }

        _lastWorldPos = worldPos;

        var res = Camera.WorldToScreenPoint(worldPos);
        var r2  = Camera.ScreenToWorldPoint(res);

        screenPos = new(res.X, res.Y);

        _worldToScreenResult = screenPos;
        _screenToWorldResult = r2;

        _lookAtVector   = cmPtr->CurrentCamera->LookAtVector;
        _cameraPosition = cmPtr->CurrentCamera->Position;

        return IsVisible(worldPos) && IsInFrontOfCamera(worldPos);
    }

    private Vector3 _cameraPosition      = new();
    private Vector3 _lookAtVector        = new();
    private Vector3 _lastWorldPos        = new();
    private Vector2 _worldToScreenResult = new();
    private Vector3 _screenToWorldResult = new();

    public bool IsInFrontOfCamera(Vector3 worldPos)
    {
        Vector3 cameraDirection = Vector3.Normalize(_lookAtVector - _cameraPosition);
        Vector3 toPoint         = worldPos - _cameraPosition;

        // Dot product to check if the point is in front of the camera
        float dot = Vector3.Dot(cameraDirection, toPoint);
        return dot > 0;
    }

    public bool IsVisible(Vector3 worldPos, float tolerance = 0.0001f)
    {
        Vector3 cameraDirection = Vector3.Normalize(_lookAtVector - _cameraPosition);
        Vector3 toPoint         = Vector3.Normalize(worldPos - _cameraPosition);

        // Dot product to check if the point is in front of the camera
        float dot = Vector3.Dot(cameraDirection, toPoint);

        // Check if the dot product is approximately 1 within a small tolerance
        return dot > tolerance;
    }
}
