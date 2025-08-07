using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Camera = FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera;

namespace Umbra.Game;

[Service]
internal sealed class GameCamera : IGameCamera
{
    public unsafe bool WorldToScreen(
        Vector3 worldPos, out Vector2 screenPos, int horizontalOffset = 0, int verticalOffset = 0
    )
    {
        FFXIVClientStructs.FFXIV.Common.Math.Vector2 v2 = new();
        FFXIVClientStructs.FFXIV.Common.Math.Vector3 v3 = new(worldPos.X, worldPos.Y, worldPos.Z);
        Camera.WorldToScreenPoint(&v2, &v3);
        screenPos = new(v2.X, v2.Y);

        bool isInViewport = screenPos.X >= horizontalOffset
            && screenPos.X <= ImGui.GetIO().DisplaySize.X - horizontalOffset
            && screenPos.Y >= verticalOffset
            && screenPos.Y <= ImGui.GetIO().DisplaySize.Y - verticalOffset;

        return isInViewport && IsInFrontOfCamera(worldPos);
    }

    public bool IsInFrontOfCamera(Vector3 worldPos)
    {
        (Vector3 lookAtVector, Vector3 cameraPosition) = GetCameraVectors();
        
        Vector3 cameraDirection = Vector3.Normalize(lookAtVector - cameraPosition);
        Vector3 toPoint         = worldPos - cameraPosition;

        return Vector3.Dot(cameraDirection, toPoint) > 0;
    }
    
    public float GetCameraAngle()
    {
        (Vector3 lookAtVector, Vector3 cameraPosition) = GetCameraVectors();
        
        Vector3 cameraDirection = Vector3.Normalize(lookAtVector - cameraPosition);
        var     result          = MathF.Atan2(cameraDirection.Z, cameraDirection.X);
        
        if (float.IsNaN(result)) {
            return 0;
        }

        return result;
    }

    private unsafe (Vector3 position, Vector3 lookAt) GetCameraVectors()
    {
        CameraManager* cmPtr = CameraManager.Instance();
        return cmPtr == null 
            ? (default, default) 
            : (cmPtr->CurrentCamera->LookAtVector, cmPtr->CurrentCamera->Position);
    }
}
