using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Umbra.Common;

namespace Umbra.Game;

[Service]
public sealed class GameCamera : IDisposable
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint GetEngineCoreSingletonDelegate();

    public Matrix4x4 ViewProjection    { get; private set; }
    public Matrix4x4 Projection        { get; private set; }
    public Matrix4x4 ViewMatrix        { get; private set; }
    public Matrix4x4 CameraWorldMatrix { get; private set; }
    public Vector2   ViewportSize      { get; private set; }
    public Vector3   CameraPosition    { get; private set; }
    public float     CameraAzimuth     { get; private set; }
    public float     CameraAltitude    { get; private set; }

    private nint _engineCoreSingleton;

    public GameCamera(ISigScanner sigScanner)
    {
        _engineCoreSingleton = Marshal.GetDelegateForFunctionPointer<GetEngineCoreSingletonDelegate>(
            sigScanner.ScanText("E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4C 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??")
        )();

        if (_engineCoreSingleton == IntPtr.Zero) Logger.Error("Failed to initialize GameCamera.");
    }

    public void Dispose()
    {
        _engineCoreSingleton = IntPtr.Zero;
    }

    [OnDraw]
    private void OnDraw()
    {
        if (_engineCoreSingleton == IntPtr.Zero) return;

        ViewProjection    = ReadMatrix(_engineCoreSingleton + 0x1B4);
        Projection        = ReadMatrix(_engineCoreSingleton + 0x174);
        ViewMatrix        = ViewProjection * Inverse(Projection);
        CameraWorldMatrix = Inverse(ViewMatrix);
        CameraAzimuth     = MathF.Atan2(ViewMatrix.M31, ViewMatrix.M33);
        CameraAltitude    = MathF.Asin(ViewMatrix.M32);
        ViewportSize      = ReadVec2(_engineCoreSingleton + 0x1F4);
        CameraPosition    = new(CameraWorldMatrix.M41, CameraWorldMatrix.M42, CameraWorldMatrix.M43);
    }

    private static unsafe Matrix4x4 ReadMatrix(nint address)
    {
        var       p = (float*)address;
        Matrix4x4 mtx;

        // Unrolled for performance.
        mtx.M11 = p[0];
        mtx.M12 = p[1];
        mtx.M13 = p[2];
        mtx.M14 = p[3];
        mtx.M21 = p[4];
        mtx.M22 = p[5];
        mtx.M23 = p[6];
        mtx.M24 = p[7];
        mtx.M31 = p[8];
        mtx.M32 = p[9];
        mtx.M33 = p[10];
        mtx.M34 = p[11];
        mtx.M41 = p[12];
        mtx.M42 = p[13];
        mtx.M43 = p[14];
        mtx.M44 = p[15];

        return mtx;
    }

    private static unsafe Vector2 ReadVec2(nint address)
    {
        var p = (float*)address;
        return new(p[0], p[1]);
    }

    private static Matrix4x4 Inverse(Matrix4x4 mat)
    {
        // Thank you ChatGPT for not making me write this by hand...
        var m = new float[16];

        m[0]  = mat.M11;
        m[1]  = mat.M12;
        m[2]  = mat.M13;
        m[3]  = mat.M14;
        m[4]  = mat.M21;
        m[5]  = mat.M22;
        m[6]  = mat.M23;
        m[7]  = mat.M24;
        m[8]  = mat.M31;
        m[9]  = mat.M32;
        m[10] = mat.M33;
        m[11] = mat.M34;
        m[12] = mat.M41;
        m[13] = mat.M42;
        m[14] = mat.M43;
        m[15] = mat.M44;

        var inv = new float[16];

        inv[0] = m[5] * m[10] * m[15]
          - m[5]      * m[11] * m[14]
          - m[9]      * m[6]  * m[15]
          + m[9]  * m[7] * m[14]
          + m[13] * m[6] * m[11]
          - m[13] * m[7] * m[10];

        inv[4] = -m[4] * m[10] * m[15]
          + m[4]       * m[11] * m[14]
          + m[8]       * m[6]  * m[15]
          - m[8]  * m[7] * m[14]
          - m[12] * m[6] * m[11]
          + m[12] * m[7] * m[10];

        inv[8] = m[4] * m[9]  * m[15]
          - m[4]      * m[11] * m[13]
          - m[8]      * m[5]  * m[15]
          + m[8]  * m[7] * m[13]
          + m[12] * m[5] * m[11]
          - m[12] * m[7] * m[9];

        inv[12] = -m[4] * m[9]  * m[14]
          + m[4]        * m[10] * m[13]
          + m[8]        * m[5]  * m[14]
          - m[8]  * m[6] * m[13]
          - m[12] * m[5] * m[10]
          + m[12] * m[6] * m[9];

        inv[1] = -m[1] * m[10] * m[15]
          + m[1]       * m[11] * m[14]
          + m[9]       * m[2]  * m[15]
          - m[9]  * m[3] * m[14]
          - m[13] * m[2] * m[11]
          + m[13] * m[3] * m[10];

        inv[5] = m[0] * m[10] * m[15]
          - m[0]      * m[11] * m[14]
          - m[8]      * m[2]  * m[15]
          + m[8]  * m[3] * m[14]
          + m[12] * m[2] * m[11]
          - m[12] * m[3] * m[10];

        inv[9] = -m[0] * m[9]  * m[15]
          + m[0]       * m[11] * m[13]
          + m[8]       * m[1]  * m[15]
          - m[8]  * m[3] * m[13]
          - m[12] * m[1] * m[11]
          + m[12] * m[3] * m[9];

        inv[13] = m[0] * m[9]  * m[14]
          - m[0]       * m[10] * m[13]
          - m[8]       * m[1]  * m[14]
          + m[8]  * m[2] * m[13]
          + m[12] * m[1] * m[10]
          - m[12] * m[2] * m[9];

        inv[2] = m[1] * m[6] * m[15]
          - m[1]      * m[7] * m[14]
          - m[5]      * m[2] * m[15]
          + m[5]  * m[3] * m[14]
          + m[13] * m[2] * m[7]
          - m[13] * m[3] * m[6];

        inv[6] = -m[0] * m[6] * m[15]
          + m[0]       * m[7] * m[14]
          + m[4]       * m[2] * m[15]
          - m[4]  * m[3] * m[14]
          - m[12] * m[2] * m[7]
          + m[12] * m[3] * m[6];

        inv[10] = m[0] * m[5] * m[15]
          - m[0]       * m[7] * m[13]
          - m[4]       * m[1] * m[15]
          + m[4]  * m[3] * m[13]
          + m[12] * m[1] * m[7]
          - m[12] * m[3] * m[5];

        inv[14] = -m[0] * m[5] * m[14]
          + m[0]        * m[6] * m[13]
          + m[4]        * m[1] * m[14]
          - m[4]  * m[2] * m[13]
          - m[12] * m[1] * m[6]
          + m[12] * m[2] * m[5];

        inv[3] = -m[1] * m[6] * m[11]
          + m[1]       * m[7] * m[10]
          + m[5]       * m[2] * m[11]
          - m[5] * m[3] * m[10]
          - m[9] * m[2] * m[7]
          + m[9] * m[3] * m[6];

        inv[7] = m[0] * m[6] * m[11]
          - m[0]      * m[7] * m[10]
          - m[4]      * m[2] * m[11]
          + m[4] * m[3] * m[10]
          + m[8] * m[2] * m[7]
          - m[8] * m[3] * m[6];

        inv[11] = -m[0] * m[5] * m[11]
          + m[0]        * m[7] * m[9]
          + m[4]        * m[1] * m[11]
          - m[4] * m[3] * m[9]
          - m[8] * m[1] * m[7]
          + m[8] * m[3] * m[5];

        inv[15] = m[0] * m[5] * m[10]
          - m[0]       * m[6] * m[9]
          - m[4]       * m[1] * m[10]
          + m[4] * m[2] * m[9]
          + m[8] * m[1] * m[6]
          - m[8] * m[2] * m[5];

        float det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

        if (det == 0) throw new("Matrix is not invertible");

        det = 1.0f / det;

        Matrix4x4 result = new Matrix4x4(
            inv[0]  * det,
            inv[1]  * det,
            inv[2]  * det,
            inv[3]  * det,
            inv[4]  * det,
            inv[5]  * det,
            inv[6]  * det,
            inv[7]  * det,
            inv[8]  * det,
            inv[9]  * det,
            inv[10] * det,
            inv[11] * det,
            inv[12] * det,
            inv[13] * det,
            inv[14] * det,
            inv[15] * det
        );

        return result;
    }
}
