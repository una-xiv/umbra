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

namespace Umbra.Markers.Factories;

[Service]
public class GatheringNodeWorldMarkerFactory(IDataManager dataManager, IObjectTable objectTable, IPlayer player)
    : IWorldMarkerFactory
{
    [ConfigVariable("Markers.GatheringNodes.Enabled", "EnabledMarkers")]
    private static bool Enabled { get; set; } = true;

    private readonly List<GatheringNode> _gatheringNodes = [];

    private int _displayIndex = 0;

    public List<WorldMarker> GetMarkers()
    {
        if (!Enabled) return [];

        lock (_gatheringNodes) {
            return _gatheringNodes
                .Select(
                    obj => new WorldMarker {
                        Position        = obj.Position,
                        IconId          = obj.IconId,
                        Label           = obj.Label,
                        SubLabel        = obj.SubLabel,
                        ShowDirection   = obj.ShowDirection,
                        MinFadeDistance = 12,
                        MaxFadeDistance = 15,
                    }
                )
                .ToList();
        }
    }

    [OnTick(interval: 1000)]
    internal void FindGatheringNodes()
    {
        if (!Enabled) return;

        lock (_gatheringNodes) {
            _gatheringNodes.Clear();

            foreach (var obj in objectTable) {
                if (!obj.IsTargetable
                 || obj.ObjectKind != ObjectKind.GatheringPoint)
                    continue;

                var node = CreateNodeFromObject(obj);
                if (node == null) continue;

                _gatheringNodes.Add(node.Value);
            }
        }
    }

    [OnTick(interval: 2000)]
    internal void IncreaseDisplayIndex()
    {
        _displayIndex++;

        if (_displayIndex > 1000)
            _displayIndex = 0;
    }

    private GatheringNode? CreateNodeFromObject(GameObject obj)
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
            Position      = obj.Position,
            IconId        = (uint)(point.GatheringPointBase.Value?.GatheringType.Value?.IconMain ?? 0),
            Label         = $"Lv.{point.GatheringPointBase.Value!.GatheringLevel} {obj.Name}",
            SubLabel      = items.Count > 0 ? $"{point.Count}x {items[_displayIndex % items.Count]}" : null,
            ShowDirection = !(!player.IsDiving && point.GatheringPointBase.Value?.GatheringType.Row == 5),
        };
    }

    private struct GatheringNode
    {
        public Vector3 Position;
        public uint    IconId;
        public string  Label;
        public string? SubLabel;
        public bool    ShowDirection;
    }
}
