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
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
public sealed class WorldMarkerDirectionRenderer(
    IGameGui         gameGui,
    ITextureProvider textureProvider,
    ClipRectProvider clipRectProvider,
    IPlayer          player
)
{
    [ConfigVariable("Markers.Direction.Enabled", "MarkerSettings")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Markers.Direction.Radius", "MarkerSettings", min: 64, max: 600)]
    private static int Radius { get; set; } = 64;

    [ConfigVariable("Markers.Direction.UseCircularPositioning", "MarkerSettings")]
    private static bool UseCircularPositioning { get; set; } = false;

    public void Render(List<WorldMarkerObject> markers)
    {
        if (!Enabled) return;

        markers.ForEach(RenderDirection);
    }

    private void RenderDirection(WorldMarkerObject marker)
    {
        if (!marker.ShowDirection
         || gameGui.WorldToScreen(marker.Position, out _)) {
            return;
        }

        var playerPos = player.Position;
        var direction = new Vector2(marker.Position.X, marker.Position.Z) - new Vector2(playerPos.X, playerPos.Z);

        float angle = MathF.Atan2(direction.Y, direction.X) + CameraAngle;
        float wSize = 64 * Element.ScaleFactor;

        var displaySize = ImGui.GetIO().DisplaySize;
        var displayPos  = ImGui.GetMainViewport().Pos;

        // Move the marker to the edge of the screen.
        float dX = UseCircularPositioning ? displaySize.Y : displaySize.X;
        float dY = displaySize.Y;

        dX = dX / 2 - Radius;
        dY = dY / 2 - Radius;

        var xMin = displaySize.X / 2 - dX;
        var xMax = displaySize.X / 2 + dX;
        var yMin = displaySize.Y / 2 - dY;
        var yMax = displaySize.Y / 2 + dY;

        gameGui.WorldToScreen(playerPos, out var center);

        var cos    = MathF.Cos(angle);
        var xRange = cos > 0 ? (xMax - center.X) : (center.X - xMin);
        var sin    = MathF.Sin(angle);
        var yRange = sin > 0 ? (yMax - center.Y) : (center.Y - yMin);

        var angleForPos = MathF.Atan2(sin * xRange, cos * yRange);

        float x = center.X + MathF.Cos(angleForPos) * xRange;
        float y = center.Y + MathF.Sin(angleForPos) * yRange;

        x = MathF.Min(MathF.Max(x, xMin), xMax);
        y = MathF.Min(MathF.Max(y, yMin), yMax);

        x += displayPos.X;
        y += displayPos.Y;

        var bounds = new Rect(new(x - (wSize / 2), y - (wSize / 2)), new(x + (wSize / 2), y + (wSize / 2)));

        if (clipRectProvider.FindClipRectsIntersectingWith(bounds).Count > 0) {
            return;
        }

        if (textureProvider.GetIcon(marker.IconIds[0]) is { } icon) {
            float halfSize = 12 * Element.ScaleFactor;

            ImGui
                .GetBackgroundDrawList()
                .AddImage(
                    icon.ImGuiHandle,
                    new(x - halfSize, y - halfSize),
                    new(x + halfSize, y + halfSize),
                    Vector2.Zero,
                    Vector2.One,
                    0xFFFFFFFF.ApplyAlphaComponent(1.0f)
                );
        }

        if (textureProvider.GetIcon(60541) is { } arrow) {
            float halfSize = 16 * Element.ScaleFactor;

            var arrowPos = new Vector2(x - halfSize, y - halfSize);
            arrowPos.X += ((wSize / 2) - halfSize) * MathF.Cos(angle);
            arrowPos.Y += ((wSize / 2) - halfSize) * MathF.Sin(angle);

            ImGui
                .GetBackgroundDrawList()
                .AddImageRotated(arrow.ImGuiHandle, angle, arrowPos, new(halfSize * 2, halfSize * 2));
        }
    }

    private static unsafe float CameraAngle =>
        DegreesToRadians(AtkStage.GetSingleton()->GetNumberArrayData()[24]->IntArray[3]);

    private static float DegreesToRadians(float degrees) => MathF.PI / 180.0f * degrees;
}
