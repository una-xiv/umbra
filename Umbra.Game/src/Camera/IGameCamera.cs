using System.Numerics;

namespace Umbra.Game;

public interface IGameCamera
{
    bool WorldToScreen(Vector3 worldPosition, out Vector2 screenPosition, int horizontalOffset = 0, int verticalOffset = 0);

    bool IsInFrontOfCamera(Vector3 worldPosition);

    float GetCameraAngle();
}
