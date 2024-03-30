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

using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class FlagWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    [ConfigVariable(
        "Toolbar.Widget.Flag.Enabled",
        "Toolbar Widgets",
        "Show flag teleport widget",
        "Displays a widget that allows you to teleport to an Aetheryte near the flag marker."
    )]
    private static bool Enabled { get; set; } = true;

    private AetheryteEntry? _currentAetheryteEntry;

    private readonly IAetheryteList _aetheryteList;
    private readonly IZoneManager   _zoneManager;
    private readonly Player         _player;

    public unsafe FlagWidget(IAetheryteList aetheryteList, IZoneManager zoneManager, Player player)
    {
        _aetheryteList = aetheryteList;
        _zoneManager   = zoneManager;
        _player        = player;

        Element.OnClick += () => {
            if (null == _currentAetheryteEntry) return;
            if (!player.CanUseTeleportAction) return;

            var agentMap = AgentMap.Instance();
            if (null == agentMap || 0 == agentMap->IsFlagMarkerSet) return;

            var tp = Telepo.Instance();
            if (null == tp) return;

            tp->Teleport(_currentAetheryteEntry.AetheryteId, _currentAetheryteEntry.SubIndex);
        };

        Element.OnRightClick += () => {
            AgentMap* agentMap = AgentMap.Instance();
            if (null == agentMap) return;

            agentMap->IsFlagMarkerSet = 0;
        };
    }

    [OnDraw]
    public unsafe void OnDraw()
    {
        if (false == Enabled) {
            Element.IsVisible = false;
            return;
        }

        AgentMap* agentMap = AgentMap.Instance();

        if (null == agentMap) {
            Element.IsVisible = false;
            return;
        }

        bool isFlagMarkerSet = 1 == agentMap->IsFlagMarkerSet;

        Element.IsVisible = true;
        Element.Size      = new(0, Height - 6);

        Element.Tooltip = isFlagMarkerSet
            ? "Left click: Teleport to Aetheryte near flag marker. Right click to remove the flag."
            : "No flag marker set.";

        // Update widget size.
        Element.Size                                      = new(0, Height - 6);
        Element.Get("Info").IsVisible                     = isFlagMarkerSet;
        Element.Get("Icon").Size                          = new(Height - 6, Height - 6);
        Element.Get("Icon").GetNode<IconNode>().IconId    = 60561;
        Element.Get("Icon").GetNode<IconNode>().Grayscale = isFlagMarkerSet && _player.CanUseTeleportAction ? 0 : 1;
        Element.Get("Icon").GetNode<IconNode>().Opacity   = isFlagMarkerSet ? 1 : 0.66f;

        Element.Get("Info.Name").Size                      = new Size(0, Height >= 31 ? Height / 2 - 3 : Height - 9);
        Element.Get("Info.Name").GetNode<TextNode>().Align = Height >= 31 ? Align.BottomRight : Align.MiddleRight;
        Element.Get("Info.Name").GetNode<TextNode>().Font  = Height >= 40 ? Font.Axis : Font.AxisSmall;

        Element.Get("Info.Sub").IsVisible                = Height >= 31;
        Element.Get("Info.Sub").Size                     = new Size(0, Height / 2 - 3);
        Element.Get("Info.Sub").GetNode<TextNode>().Font = Height >= 40 ? Font.AxisSmall : Font.AxisExtraSmall;

        if (isFlagMarkerSet) {
            var marker = agentMap->FlagMapMarker;
            var zone   = _zoneManager.GetZone(agentMap->FlagMapMarker.MapId);
            var pos    = new Vector3(marker.XFloat, 0, marker.YFloat);

            Element.Get("Info.Name").GetNode<TextNode>().Text = $"{zone.Name}";

            // Find nearest Aetheryte.
            ZoneMarker? aetheryte = zone
                .StaticMarkers.Where(m => m.Type == ZoneMarkerType.Aetheryte)
                .OrderBy(m => Vector3.Distance(pos, m.WorldPosition))
                .FirstOrDefault();

            if (null != aetheryte) {
                var aetheryteName = aetheryte.Value.Name;

                if (aetheryteName == "") {
                    // Find nearest district close to the Aetheryte.
                    ZoneMarker? district = zone
                        .StaticMarkers.Concat(zone.DynamicMarkers)
                        .Where(
                            m => m.Name != ""
                             && (m.Type    == ZoneMarkerType.Pin
                                 || m.Type == ZoneMarkerType.Settlement
                                 || m.Type == ZoneMarkerType.Area
                                 || m.Type == ZoneMarkerType.Aetheryte
                                 || m.Type == ZoneMarkerType.Aethernet)
                        )
                        .OrderBy(m => Vector2.Distance(aetheryte.Value.Position, m.WorldPosition.ToVector2()))
                        .FirstOrDefault();

                    if (null != district) {
                        aetheryteName = district.Value.Name;
                    }
                }

                _currentAetheryteEntry = _aetheryteList.FirstOrDefault(a => a.AetheryteId == aetheryte.Value.DataId);

                if (null != _currentAetheryteEntry) {
                    Element.Get("Info.Sub").GetNode<TextNode>().Text =
                        $"{aetheryteName} ({_currentAetheryteEntry.GilCost} gil)";
                } else {
                    Element.Get("Info.Sub").GetNode<TextNode>().Text = "No Aetheryte in zone.";
                }
            } else {
                Element.Get("Info.Sub").GetNode<TextNode>().Text = "No Aetheryte in zone.";
            }
        }
    }
}
