/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;
using Umbra.Windows.Clipping;
using Una.Drawing;

namespace Umbra.Markers.System.Compass;

[Service]
internal partial class CompassRenderer(
    IGameCamera         gameCamera,
    IPlayer             player,
    ITextureProvider    textureProvider,
    ClipRectProvider    clipRectProvider,
    WorldMarkerRegistry registry,
    UmbraVisibility     visibility
)
{
    [ConfigVariable("Markers.Compass.Enabled", "Markers", "MarkersCompass")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Markers.Compass.Radius", "Markers", "MarkersCompass", 8, 800)]
    private static int CompassRadius { get; set; } = 250;

    [ConfigVariable("Markers.Compass.IconScaleFactor", "Markers", "MarkersCompass", 50, 200)]
    private static int IconScaleFactor { get; set; } = 100;

    [ConfigVariable("Markers.Compass.IconOpacity", "Markers", "MarkersCompass", 0, 100)]
    private static int IconOpacity { get; set; } = 100;

    [ConfigVariable("Markers.Compass.SmoothingFactor", "Markers", "MarkersCompass", 0.01f, 0.5f)]
    private static float SmoothingFactor { get; set; } = 0.075f;

    [OnDraw]
    private void OnDraw()
    {
        if (!Enabled || !visibility.IsVisible()) return;

        float iconSize  = 24 * (IconScaleFactor / 100f) * Node.ScaleFactor;
        uint  iconColor = (0xFFFFFFFFu).ApplyAlphaComponent(IconOpacity / 100f);

        List<string> usedKeys = [];

        foreach (var marker in registry.GetMarkers()) {
            if (!marker.ShowOnCompass) continue;

            Vector3 resolvedPosition = registry.GetResolvedPosition(marker);
            if (gameCamera.WorldToScreen(resolvedPosition, out _)) continue;

            usedKeys.Add(marker.Key);
            Vector2 p = GetMarkerScreenPosition(marker, resolvedPosition);

            if (!ShouldRenderAt(
                    new(
                        (int)(p.X - iconSize / 2),
                        (int)(p.Y - iconSize / 2),
                        (int)(p.X + iconSize),
                        (int)(p.Y + iconSize)
                    )
                ))
                continue;

            if (GetIcon(marker.IconId) is { } icon) {
                ImGui
                    .GetBackgroundDrawList()
                    .AddImage(
                        icon.ImGuiHandle,
                        p - (new Vector2(iconSize) / 2f),
                        p + (new Vector2(iconSize) / 2f),
                        Vector2.Zero,
                        Vector2.One,
                        iconColor
                    );
            }

            RenderDirectionArrowAt(p, resolvedPosition, iconSize, iconColor);
        }

        foreach (string key in CompassMarkers.Keys.Except(usedKeys).ToArray()) {
            CompassMarkers.Remove(key);
        }
    }

    /// <summary>
    /// Returns true if a direction indicator should be rendered in the given
    /// rectangle. This determines whether the marker is obscured by a native
    /// UI window.
    /// </summary>
    private bool ShouldRenderAt(Windows.Clipping.Rect rect)
    {
        return clipRectProvider.FindClipRectsIntersectingWith(rect).Count == 0;
    }

    /// <summary>
    /// Renders a directional arrow at the given screen position.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void RenderDirectionArrowAt(Vector2 pos, Vector3 markerPosition, float iconSize, uint iconColor)
    {
        Vector3 cameraToPlayer = Vector3.Normalize(player.Position - gameCamera.CameraPosition);
        Vector3 playerToMarker = Vector3.Normalize(markerPosition - player.Position);

        var cameraLookAngle = (float)Math.Atan2(cameraToPlayer.Z, cameraToPlayer.X);
        var directionAngle  = (float)Math.Atan2(playerToMarker.Z, playerToMarker.X);

        float angle = (directionAngle - cameraLookAngle - MathF.PI / 2f);

        if (GetIcon(60541) is { } arrow) {
            float halfSize = 16 * (IconScaleFactor / 100f) * Node.ScaleFactor;

            var arrowPos = new Vector2(pos.X - halfSize, pos.Y - halfSize);

            arrowPos.X += ((iconSize + (16 * (IconScaleFactor / 100f) * Node.ScaleFactor)) - halfSize)
                * MathF.Cos(angle);

            arrowPos.Y += ((iconSize + (16 * (IconScaleFactor / 100f) * Node.ScaleFactor)) - halfSize)
                * MathF.Sin(angle);

            ImGui
                .GetBackgroundDrawList()
                .AddImageRotated(
                    arrow.ImGuiHandle,
                    angle,
                    arrowPos,
                    new(halfSize * 2, halfSize * 2),
                    new(iconColor)
                );
        }
    }

    private Dictionary<string, Vector2> CompassMarkers { get; } = [];

    /// <summary>
    /// Returns the compass screen position on the XZ plane for a given marker
    /// position.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private Vector2 GetMarkerScreenPosition(WorldMarker marker, Vector3 markerPosition)
    {
        Vector3 cameraToPlayer = (player.Position - gameCamera.CameraPosition);
        Vector3 playerToMarker = (markerPosition - player.Position);

        gameCamera.WorldToScreen(player.Position, out Vector2 playerScreenPosition);

        float distance = MathF.Abs(Vector3.Distance(player.Position, markerPosition)) * 50f;

        var lookAngle    = MathF.Atan2(cameraToPlayer.Z, cameraToPlayer.X) + MathF.PI / 2f;
        var sinLookAngle = MathF.Sin(lookAngle);
        var cosLookAngle = MathF.Cos(lookAngle);

        Vector3 transformedMarkerDirection = Vector3.Normalize(
            new(
                playerToMarker.X * cosLookAngle + playerToMarker.Z * sinLookAngle,
                playerToMarker.Y,
                -playerToMarker.X * sinLookAngle + playerToMarker.Z * cosLookAngle
            )
        );

        Vector2 projectedDirection =
            new Vector2(transformedMarkerDirection.X, transformedMarkerDirection.Z);

        Vector2 screenPosition = playerScreenPosition + (projectedDirection * MathF.Min(distance, CompassRadius));

        Vector2 viewportPos  = ImGui.GetMainViewport().WorkPos;
        Vector2 viewportSize = ImGui.GetMainViewport().WorkSize;

        screenPosition.X = Math.Clamp(screenPosition.X, viewportPos.X + 64, viewportSize.X - 64);
        screenPosition.Y = Math.Clamp(screenPosition.Y, viewportPos.Y + 64, viewportSize.Y - 64);

        if (!CompassMarkers.ContainsKey(marker.Key)) {
            CompassMarkers[marker.Key] = screenPosition;
        }

        CompassMarkers[marker.Key] += SmoothingFactor * (screenPosition - CompassMarkers[marker.Key]);

        return CompassMarkers[marker.Key];
    }
}
