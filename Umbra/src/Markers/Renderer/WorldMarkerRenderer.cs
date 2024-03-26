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
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using ImGuiNET;
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
public sealed class WorldMarkerRenderer(IGameGui gameGui, Player player, ClipRectProvider clipRectProvider)
{
    [ConfigVariable(
        "Markers.OcclusionTest.Enabled",
        "Marker Settings",
        "Enable occlusion testing for world markers",
        "Enforces a minimum opacity for markers that are not in line of sight of the player character."
    )]
    private static bool OcclusionTestEnabled { get; set; } = true;

    [ConfigVariable(
        "Markers.DistanceOpacity.Enabled",
        "Marker Settings",
        "Enable distance-based fading",
        "Fades out world markers when the player is closing in to the marker position. The actual distance may differ between marker types."
    )]
    private static bool EnableDistanceBasedOpacity { get; set; } = true;

    public void Render(List<WorldMarkerObject> markers)
    {
        foreach (var marker in markers) {
            RenderMarker(marker);
        }
    }

    private void RenderMarker(WorldMarkerObject marker)
    {
        if (false == gameGui.WorldToScreen(marker.Position, out var screenPosition)) {
            return;
        }

        var clipRect = new ClipRect(
            new(screenPosition.X - 32, screenPosition.Y - 32),
            new(screenPosition.X + 32, screenPosition.Y + 32)
        );

        if (clipRectProvider.FindClipRectsIntersectingWith(clipRect).Count > 0) {
            return;
        }

        var distance = Vector3.Distance(player.Position, marker.Position);
        var element  = BuildElement(marker, distance);

        if (EnableDistanceBasedOpacity) {
            element.Opacity = Math.Clamp(
                1 - (distance - marker.MaxFadeDistance) / (marker.MinFadeDistance - marker.MaxFadeDistance),
                marker.MinOpacity,
                marker.MaxOpacity
            );
        }

        if (OcclusionTestEnabled
         && distance <= marker.MaxFadeDistance
         && IsMarkerOccluded(marker.Position)) {
            element.Opacity = 0.65f;
        }

        element.Render(ImGui.GetBackgroundDrawList(), new(screenPosition.X, screenPosition.Y - 64));

        if (marker.OnClick != null
         && distance       < 55) {
            var rect = new ClipRect(
                new(screenPosition.X - 32, screenPosition.Y - 64),
                new(screenPosition.X + 32, screenPosition.Y)
            );

            if (rect.Contains(ImGui.GetMousePos())) {
                ImGui.SetNextWindowPos(rect.Min, ImGuiCond.Always);
                ImGui.SetNextWindowSize(rect.Max - rect.Min, ImGuiCond.Always);

                ImGui.Begin(
                    "##markerButton",
                    ImGuiWindowFlags.NoTitleBar
                  | ImGuiWindowFlags.NoResize
                  | ImGuiWindowFlags.NoMove
                  | ImGuiWindowFlags.NoBackground
                  | ImGuiWindowFlags.NoScrollbar
                  | ImGuiWindowFlags.NoSavedSettings
                );

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                    marker.OnClick();
                }

                ImGui.End();
            }
        }
    }

    private static unsafe bool IsMarkerOccluded(Vector3 position)
    {
        var cm  = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager.Instance();
        var obj = cm->GetActiveCamera()->CameraBase.SceneCamera.Object.Position;
        var pos = new Vector3(obj.X, obj.Y, obj.Z);
        var dir = Vector3.Normalize(position - pos);

        if (false == BGCollisionModule.Raycast(pos, dir, out var hit)) return false;

        return Vector3.Distance(pos, hit.Point) < Vector3.Distance(pos, position);
    }

    private Element BuildElement(WorldMarkerObject marker, float distance)
    {
        return new Element(
            direction: Direction.Vertical,
            anchor: Anchor.Top | Anchor.Center,
            children: [
                new(
                    direction: Direction.Horizontal,
                    anchor: Anchor.Top | Anchor.Left,
                    size: new(512, 0),
                    gap: 6,
                    children: marker
                        .IconIds[..Math.Min(marker.IconIds.Count, 3)].Select(
                            iconId => new Element(
                                anchor: Anchor.Top | Anchor.Center,
                                size: new(32, 32),
                                nodes: [
                                    new IconNode(iconId: iconId)
                                ]
                            )
                        )
                        .ToList()
                ),
                new(
                    id: "LabelContainer",
                    direction: Direction.Vertical,
                    anchor: Anchor.Top | Anchor.Left,
                    size: new(512, 0),
                    gap: distance >= marker.MaxFadeDistance ? 2 : 1,
                    children: [
                        ..marker
                            .Text[..Math.Min(marker.Text.Count, 4)]
                            .Select(
                                text => new Element(
                                    direction: Direction.Vertical,
                                    anchor: Anchor.Top | Anchor.Left,
                                    size: new(512, 0),
                                    gap: 1,
                                    isVisible: text.Label?.Length > 0 || text.SubLabel?.Length > 0,
                                    children: [
                                        new(
                                            anchor: Anchor.Top | Anchor.Left,
                                            fit: true,
                                            nodes: [
                                                new TextNode(
                                                    text: text.Label ?? "",
                                                    color: 0xDDFFFFFF,
                                                    outlineColor: 0x70000000,
                                                    outlineSize: 1,
                                                    align: Align.TopCenter,
                                                    font: Font.AxisSmall
                                                )
                                            ]
                                        ) { IsVisible = text.Label?.Length > 0 },
                                        new(
                                            anchor: Anchor.Top | Anchor.Left,
                                            fit: true,
                                            nodes: [
                                                new TextNode(
                                                    text: text.SubLabel ?? "",
                                                    color: 0xA0FFFFFF,
                                                    outlineColor: 0x70000000,
                                                    outlineSize: 1,
                                                    align: Align.TopCenter,
                                                    font: Font.AxisExtraSmall
                                                )
                                            ]
                                        ) {
                                            IsVisible = text.SubLabel?.Length > 0 && distance >= marker.MaxFadeDistance
                                        }
                                    ]
                                )
                            )
                            .ToList(),
                        new Element(
                            id: "Distance",
                            anchor: Anchor.Top | Anchor.Left,
                            fit: true,
                            nodes: [
                                new TextNode(
                                    text: $"{distance:F1} yalms",
                                    color: 0xA0FFFFFF,
                                    outlineColor: 0x70000000,
                                    outlineSize: 1,
                                    align: Align.TopCenter,
                                    font: Font.AxisExtraSmall,
                                    margin: new(4, 0, 0)
                                )
                            ]
                        )
                    ]
                )
            ]
        );
    }
}
