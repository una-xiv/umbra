using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using System;
using Umbra.Common;

namespace Umbra.Game;

[Service]
internal sealed class GameCamera : IGameCamera
{
    private Vector3 _cameraPosition;
    private Vector3 _lookAtVector;

    public unsafe bool WorldToScreen(
        Vector3 worldPos, out Vector2 screenPos, int horizontalOffset = 0, int verticalOffset = 0
    )
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

        bool isInViewport = screenPos.X >= horizontalOffset
            && screenPos.X <= ImGui.GetIO().DisplaySize.X - horizontalOffset
            && screenPos.Y >= verticalOffset
            && screenPos.Y <= ImGui.GetIO().DisplaySize.Y - verticalOffset;

        return isInViewport && IsInFrontOfCamera(worldPos);
    }

    public bool IsInFrontOfCamera(Vector3 worldPos)
    {
        Vector3 cameraDirection = Vector3.Normalize(_lookAtVector - _cameraPosition);
        Vector3 toPoint         = worldPos - _cameraPosition;

        return Vector3.Dot(cameraDirection, toPoint) > 0;
    }
    
    public float GetCameraAngle()
    {
        Vector3 cameraDirection = Vector3.Normalize(_lookAtVector - _cameraPosition);
        var result = MathF.Atan2(cameraDirection.Z, cameraDirection.X);
        
        if (float.IsNaN(result)) {
            return 0;
        }

        return result;
    }
}
