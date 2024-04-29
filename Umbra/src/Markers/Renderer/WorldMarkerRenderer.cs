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
using Umbra.Interface;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
public sealed class WorldMarkerRenderer(
    IGameGui         gameGui,
    IGameCamera      gameCamera,
    IPlayer          player,
    ClipRectProvider clipRectProvider
)
{
    [ConfigVariable("Markers.OcclusionTest.Enabled", "MarkerSettings")]
    private static bool OcclusionTestEnabled { get; set; } = true;

    [ConfigVariable("Markers.DistanceOpacity.Enabled", "MarkerSettings")]
    private static bool EnableDistanceBasedOpacity { get; set; } = true;

    [ConfigVariable("Markers.MarkerIconScaleFactor", "MarkerSettings", min: 50, max: 200)]
    private static int IconScaleFactor { get; set; } = 100;

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

        int iconSize = 32 * IconScaleFactor / 100;

        var r = new Rect(
            new(screenPosition.X - iconSize, screenPosition.Y - iconSize),
            new(screenPosition.X + iconSize, screenPosition.Y + iconSize)
        );

        if (clipRectProvider.FindClipRectsIntersectingWith(r).Count > 0) {
            return;
        }

        var distance = Vector3.Distance(player.Position, marker.Position);
        var element  = BuildElement(marker, distance);

        if (EnableDistanceBasedOpacity) {
            element.Style.Opacity = Math.Clamp(
                1 - (distance - marker.MaxFadeDistance) / (marker.MinFadeDistance - marker.MaxFadeDistance),
                marker.MinOpacity,
                marker.MaxOpacity
            );
        }

        if (OcclusionTestEnabled
            && distance > 30
            && IsMarkerOccluded(marker.Position)) {
            element.Style.Opacity = 0.45f;
        }

        element.Render(ImGui.GetBackgroundDrawList(), screenPosition with { Y = screenPosition.Y - 64 });

        if (marker.OnClick != null
         && distance       < 55) {
            var rect = new Rect(
                new(screenPosition.X - iconSize, screenPosition.Y - (iconSize * 2)),
                screenPosition with { X = screenPosition.X + iconSize }
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

    private bool IsMarkerOccluded(Vector3 position)
    {
        var markerPos = position + new Vector3(0, 1.8f, 0);
        var cameraPos = gameCamera.CameraPosition;
        var dir       = Vector3.Normalize(markerPos - cameraPos);

        if (false == BGCollisionModule.Raycast(cameraPos, dir, out var hit)) return false;

        return Vector3.Distance(cameraPos, hit.Point) < Vector3.Distance(cameraPos, markerPos);
    }

    private static Element BuildElement(WorldMarkerObject marker, float distance)
    {
        int iconSize = 32 * IconScaleFactor / 100;

        return new(
            id: "",
            flow: Flow.Vertical,
            anchor: Anchor.TopCenter,
            children: [
                new(
                    id: "IconContainer",
                    flow: Flow.Horizontal,
                    anchor: Anchor.TopCenter,
                    fit: true,
                    gap: 6,
                    children: marker
                        .IconIds[..Math.Min(marker.IconIds.Count, 3)]
                        .Select(
                            iconId => new Element(
                                id: "",
                                anchor: Anchor.TopCenter,
                                size: new(iconSize, iconSize),
                                style: new() {
                                    Image = iconId
                                }
                            )
                        )
                        .ToList()
                ),
                new(
                    id: "LabelContainer",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopCenter,
                    gap: distance >= marker.MaxFadeDistance ? 2 : 1,
                    children: [
                        ..marker
                            .Text[..Math.Min(marker.Text.Count, 4)]
                            .Select(
                                text => new Element(
                                    id: "",
                                    flow: Flow.Vertical,
                                    anchor: Anchor.TopCenter,
                                    size: new(512, 0),
                                    gap: 1,
                                    isVisible: text.Label?.Length > 0 || text.SubLabel?.Length > 0,
                                    children: [
                                        new(
                                            id: "",
                                            anchor: Anchor.TopCenter,
                                            size: new(512, 0),
                                            text: text.Label,
                                            style: new() {
                                                Font         = Font.AxisSmall,
                                                TextColor    = 0xDDFFFFFF,
                                                TextAlign    = Anchor.TopCenter,
                                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                                OutlineWidth = 1,
                                            }
                                        ) { IsVisible = text.Label?.Length > 0 },
                                        new(
                                            id: "",
                                            anchor: Anchor.TopCenter,
                                            size: new(512, 0),
                                            text: text.SubLabel,
                                            style: new() {
                                                Font         = Font.AxisExtraSmall,
                                                TextColor    = 0xA0FFFFFF,
                                                TextAlign    = Anchor.TopCenter,
                                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                                OutlineWidth = 1,
                                            }
                                        ) {
                                            IsVisible = text.SubLabel?.Length > 0 && distance >= marker.MaxFadeDistance
                                        }
                                    ]
                                )
                            )
                            .ToList(),
                        new Element(
                            id: "Distance",
                            anchor: Anchor.TopCenter,
                            fit: true,
                            text: $"{distance:F1} yalms",
                            style: new() {
                                Font         = Font.AxisExtraSmall,
                                TextColor    = 0xA0FFFFFF,
                                TextAlign    = Anchor.TopCenter,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1,
                            }
                        )
                    ]
                )
            ]
        );
    }
}
