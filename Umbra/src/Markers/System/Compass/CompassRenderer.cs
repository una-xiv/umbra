using Dalamud.Interface.Textures.TextureWraps;

using Umbra.Interface;
using Una.Drawing.Clipping;

namespace Umbra.Markers.System.Compass;

[Service]
internal sealed class CompassRenderer(
    IGameCamera         gameCamera,
    IPlayer             player,
    ITextureProvider    textureProvider,
    IZoneManager        zoneManager,
    WorldMarkerRegistry registry,
    UmbraVisibility     visibility
)
{
    [ConfigVariable("Markers.Compass.Enabled", "Markers", "MarkersCompass")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Markers.Compass.Radius", "Markers", "MarkersCompass", 8, 800)]
    private static int CompassRadius { get; set; } = 750;

    [ConfigVariable("Markers.Compass.IconScaleFactor", "Markers", "MarkersCompass", 50, 200)]
    private static int IconScaleFactor { get; set; } = 100;

    [ConfigVariable("Markers.Compass.IconOpacity", "Markers", "MarkersCompass", 0, 100)]
    private static int IconOpacity { get; set; } = 100;

    [ConfigVariable("Markers.Compass.SafeZoneOffsetWidth", "Markers", "MarkersCompass", 0, 1000)]
    private static int SafeZoneOffsetWidth { get; set; } = 0;

    [ConfigVariable("Markers.Compass.SafeZoneOffsetHeight", "Markers", "MarkersCompass", 0, 1000)]
    private static int SafeZoneOffsetHeight { get; set; } = 0;

    [ConfigVariable("Markers.Compass.YOffset", "Markers", "MarkersCompass", -4096, 4096)]
    private static int CenterPointYOffset { get; set; } = 0;

    [ConfigVariable("Markers.Compass.XOffset", "Markers", "MarkersCompass", -4096, 4096)]
    private static int CenterPointXOffset { get; set; } = 0;

    [OnDraw(executionOrder: int.MaxValue)]
    private void OnUpdate()
    {
        if (!Enabled || !visibility.AreMarkersVisible()) return;
        if (!zoneManager.HasCurrentZone) return;

        float   iconSize  = 24 * (IconScaleFactor / 100f) * Node.ScaleFactor;
        float   clampSize = iconSize * 2.5f;
        uint    iconColor = (0xFFFFFFFFu).ApplyAlphaComponent(IconOpacity / 100f);
        Vector2 vpSize    = ImGui.GetMainViewport().Size;
        Vector2 workPos   = ImGui.GetMainViewport().WorkPos;

        if (!gameCamera.WorldToScreen(player.Position, out Vector2 playerScreenPosition)) return;

        playerScreenPosition.X += CenterPointXOffset;
        playerScreenPosition.Y += CenterPointYOffset;

        foreach (var marker in registry.GetMarkers()) {
            if (!marker.ShowOnCompass) continue;

            Vector3 pos  = marker.WorldPosition;
            float   dist = Vector2.Distance(pos.ToVector2(), player.Position.ToVector2());

            if (marker.MaxVisibleDistance > 0 && dist > marker.MaxVisibleDistance) continue;

            // Skip rendering if the marker itself is in view.
            if (gameCamera.WorldToScreen(pos, out Vector2 markerScreenPosition, SafeZoneOffsetWidth, SafeZoneOffsetHeight))
                continue;

            Vector2 direction = Vector2.Normalize(gameCamera.IsInFrontOfCamera(pos) ? markerScreenPosition - playerScreenPosition : playerScreenPosition - markerScreenPosition);
            Vector2 iconPos   = playerScreenPosition + direction * CompassRadius;

            // Clamp the icon position to the screen bounds.
            iconPos.X = Math.Clamp(iconPos.X, clampSize, vpSize.X - clampSize);
            iconPos.Y = Math.Clamp(iconPos.Y, clampSize, vpSize.Y - clampSize);

            Vector2 p1 = iconPos - new Vector2(iconSize / 2) + workPos;
            Vector2 p2 = iconPos + new Vector2(iconSize / 2) + workPos;

            // Skip rendering if the marker is behind a UI window.
            if (!ShouldRenderAt(new(p1, p2))) continue;

            // Skip rendering if the icon is not loaded.
            if (GetIcon(marker.IconId) is not { } icon) continue;

            ImGui
               .GetBackgroundDrawList()
               .AddImage(icon.Handle, p1, p2, Vector2.Zero, Vector2.One, iconColor);

            DrawDirectionArrowAt(iconPos + workPos, direction, iconSize, iconColor);
        }
    }

    private void DrawDirectionArrowAt(Vector2 screenPos, Vector2 direction, float iconSize, uint iconColor)
    {
        if (GetIcon(60541) is not { } arrow) return;

        float angle    = MathF.Atan2(direction.Y, direction.X);
        float halfSize = 16 * (IconScaleFactor / 100f) * Node.ScaleFactor;
        var   arrowPos = new Vector2(screenPos.X - halfSize, screenPos.Y - halfSize);

        arrowPos.X += ((iconSize + (16 * (IconScaleFactor / 100f) * Node.ScaleFactor)) - halfSize) * MathF.Cos(angle);
        arrowPos.Y += ((iconSize + (16 * (IconScaleFactor / 100f) * Node.ScaleFactor)) - halfSize) * MathF.Sin(angle);

        ImGui
           .GetBackgroundDrawList()
           .AddImageRotated(arrow.Handle, angle, arrowPos, new(halfSize * 2, halfSize * 2), new(iconColor));
    }

    /// <summary>
    /// Returns true if a direction indicator should be rendered in the given
    /// rectangle. This determines whether the marker is obscured by a native
    /// UI window.
    /// </summary>
    private bool ShouldRenderAt(ClipRect rect)
    {
        return ClipRectProvider.FindClipRectsIntersectingWith(rect).Count == 0;
    }

    private IDalamudTextureWrap? GetIcon(uint iconId)
    {
        try {
            return textureProvider.GetFromGameIcon(new(iconId)).GetWrapOrDefault();
        } catch {
            return null;
        }
    }
}
