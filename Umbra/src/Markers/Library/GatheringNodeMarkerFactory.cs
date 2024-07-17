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

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class GatheringNodeMarkerFactory(
    IDataManager dataManager,
    IObjectTable objectTable,
    IPlayer      player,
    IZoneManager zoneManager
) : WorldMarkerFactory
{
    public override string Id          { get; } = "GatheringNodeMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.GatheringNodes.Name");
    public override string Description { get; } = I18N.Translate("Markers.GatheringNodes.Description");

    private          int                 _displayIndex;
    private readonly List<GatheringNode> _gatheringNodes = [];
    private readonly List<GatheringNode> _farAwayNodes   = [];

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables
        ];
    }

    protected override void OnZoneChanged(IZone zone)
    {
        RemoveAllMarkers();
    }

    [OnTick(interval: 1000)]
    private void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        UpdateGatheringNodes();

        List<string> activeIds = [];

        var fadeDist = GetConfigValue<int>("FadeDistance");
        var fadeAttn = GetConfigValue<int>("FadeAttenuation");

        foreach (GatheringNode node in _gatheringNodes) {
            activeIds.Add(node.Key);

            SetMarker(
                new() {
                    MapId         = zoneManager.CurrentZone.Id,
                    Key           = node.Key,
                    Position      = node.Position,
                    IconId        = node.IconId,
                    Label         = node.Label,
                    SubLabel      = node.SubLabel,
                    ShowOnCompass = node.ShowDirection && GetConfigValue<bool>("ShowOnCompass"),
                    FadeDistance  = new(fadeDist, fadeDist + fadeAttn)
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private void UpdateGatheringNodes()
    {
        _gatheringNodes.Clear();

        List<string> activeKeys = [];

        foreach (var obj in objectTable) {
            if (!obj.IsTargetable
                || obj.ObjectKind != ObjectKind.GatheringPoint)
                continue;

            var node = CreateNodeFromObject(obj);
            if (node == null) continue;

            _gatheringNodes.Add(node.Value);
            activeKeys.Add(CreateLocationKey(node.Value.Position));
        }

        if (!zoneManager.HasCurrentZone) return;

        zoneManager.CurrentZone.DynamicMarkers.Where(t => t.Type == ZoneMarkerType.GatheringNode).ToList().ForEach(
            obj => {
                // Don't add these types of markers if we already got one from the object table.
                if (activeKeys.Contains(CreateLocationKey(obj.WorldPosition))) return;

                _gatheringNodes.Add(new() {
                    Key           = $"GN_{obj.WorldPosition.X:N0}_{obj.WorldPosition.Y:N0}_{obj.WorldPosition.Z:N0}",
                    Position      = obj.WorldPosition,
                    IconId        = obj.IconId,
                    Label         = $"{obj.Name}",
                    SubLabel      = null,
                    ShowDirection = true,
                });
            }
        );
    }

    private static string CreateLocationKey(Vector3 position)
    {
        var x = (int)(position.X / 5) * 5;
        var z = (int)(position.Z / 5) * 5;

        return $"{x}:{z}";
    }

    private GatheringNode? CreateNodeFromObject(IGameObject obj)
    {
        var point = dataManager.GetExcelSheet<GatheringPoint>()!.GetRow(obj.DataId);
        if (point == null) return null;

        List<string> items = point.GatheringPointBase.Value!
            .Item.Select(
                i => {
                    if (i == 0) return null;

                    var gItem = dataManager.GetExcelSheet<GatheringItem>()!.GetRow((uint)i);

                    return gItem == null
                        ? null
                        : dataManager.GetExcelSheet<Item>()!.GetRow((uint)gItem.Item)?.Name.ToString();
                }
            )
            .Where(i => i != null)
            .ToList()!;

        return new GatheringNode {
            Key           = $"GN_{obj.Position.X:N0}_{obj.Position.Y:N0}_{obj.Position.Z:N0}",
            Position      = obj.Position,
            IconId        = (uint)(point.GatheringPointBase.Value?.GatheringType.Value?.IconMain ?? 0),
            Label         = $"Lv.{point.GatheringPointBase.Value!.GatheringLevel} {obj.Name}",
            SubLabel      = items.Count > 0 ? $"{point.Count}x {items[_displayIndex % items.Count]}" : null,
            ShowDirection = !(!player.IsDiving && point.GatheringPointBase.Value?.GatheringType.Row == 5),
        };
    }

    [OnTick(interval: 2000)]
    internal void IncreaseDisplayIndex()
    {
        _displayIndex++;

        if (_displayIndex > 1000) _displayIndex = 0;
    }

    private struct GatheringNode
    {
        public string  Key;
        public Vector3 Position;
        public uint    IconId;
        public string  Label;
        public string? SubLabel;
        public bool    ShowDirection;
    }
}
