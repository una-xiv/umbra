

namespace Umbra.Markers.System;

internal static class ImDrawListPtrExtensions
{
    /// <summary>
    /// Adds a rotated image to the draw list.
    /// </summary>
    /// <param name="drawList">The <see cref="ImDrawListPtr"/> to draw to.</param>
    /// <param name="image">A pointer to the image to draw.</param>
    /// <param name="rotation">The rotation of the image in radians.</param>
    /// <param name="position">The position where to draw the image.</param>
    /// <param name="size">The size of the image. Defaults to 32x32 if omitted.</param>
    /// <param name="color">The color tint to apply to the image.</param>
    /// <param name="opacity">The opacity of the image.</param>
    public static void AddImageRotated(
        this ImDrawListPtr drawList,
        ImTextureID        image,
        float              rotation,
        Vector2            position,
        Vector2?           size    = null,
        Color?             color   = null,
        float              opacity = 1.0f
    )
    {
        size  ??= new (32, 32);
        color ??= new(0xFFFFFFFF);

        color = new(color.Value.R, color.Value.G, color.Value.B, (byte)Math.Clamp(opacity * 255, 0, 255));

        var center = size.Value / 2;

        Matrix3x2 mat = Matrix3x2.CreateRotation(rotation, center);

        Vector2[] corners = [
            new (0,            0),
            new (size.Value.X, 0),
            new (size.Value.X, size.Value.Y),
            new (0,            size.Value.Y)
        ];

        center -= size.Value / 2;

        for (byte i = 0; i < 4; i++) {
            corners[i] -= center;
            corners[i] =  Vector2.Transform(corners[i], mat);
            corners[i] += center;
        }

        drawList.AddImageQuad(
            image,
            position + corners[0],
            position + corners[1],
            position + corners[2],
            position + corners[3],
            Vector2.UnitY,
            Vector2.Zero,
            Vector2.UnitX,
            Vector2.One,
            color.Value.ToUInt()
        );
    }
}
