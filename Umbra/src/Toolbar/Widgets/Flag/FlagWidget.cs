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
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Companion;

[Service]
internal partial class FlagWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.Flag.Enabled", "ToolbarWidgets")]
    private static bool Enabled { get; set; } = true;

    private readonly IAetheryteList _aetheryteList;
    private readonly IZoneManager   _zoneManager;
    private readonly IPlayer        _player;
    private readonly Element        _name;
    private readonly Element        _info;
    private readonly Element        _icon;

    private AetheryteEntry? _aetheryteEntry;
    private string?         _aetheryteKey;
    private string?         _lastMarkerKey;

    public FlagWidget(IAetheryteList aetheryteList, IZoneManager zoneManager, IPlayer player)
    {
        _aetheryteList = aetheryteList;
        _zoneManager   = zoneManager;
        _player        = player;

        _name = Element.Get("Container.Text.Name");
        _info = Element.Get("Container.Text.Info");
        _icon = Element.Get("Container.Icon");

        Element.OnClick      += OnClick;
        Element.OnRightClick += OnRightClick;
        Element.OnMouseEnter += OnMouseEnter;
        Element.OnMouseLeave += OnMouseLeave;

        zoneManager.ZoneChanged += _ => {
            _aetheryteKey   = null;
            _aetheryteEntry = null;
        };

        Element.Tooltip = I18N.Translate("LocationWidget.Tooltip");
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        UpdateWidgetButtonState();

        if (IsFlagMarkerSet()) UpdateWidgetInfoState();
    }

    public void OnUpdate() { }

    [OnTick(interval: 2000)]
    public void OnTick()
    {
        if (!Enabled || !IsFlagMarkerSet()) return;
        if (!_zoneManager.HasCurrentZone) return;

        // Periodically update the widget info.
        _aetheryteKey = null;
    }

    private unsafe void OnClick()
    {
        if (!IsFlagMarkerSet() || !_player.CanUseTeleportAction) return;
        if (_aetheryteEntry == null) return;

        Telepo* tp = Telepo.Instance();
        if (tp == null) return;

        tp->Teleport(_aetheryteEntry.AetheryteId, _aetheryteEntry.SubIndex);
    }

    private static void OnRightClick()
    {
        if (!IsFlagMarkerSet()) return;

        RemoveFlagMarker();
    }

    private void OnMouseEnter()
    {
        Element.Get<BorderElement>().Color     = 0xFF6A6A6A;
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
    }

    private void OnMouseLeave()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
    }

    private static unsafe bool IsFlagMarkerSet()
    {
        AgentMap* map = AgentMap.Instance();
        if (map == null) return false;

        return map->IsFlagMarkerSet == 1;
    }

    private static unsafe void RemoveFlagMarker()
    {
        AgentMap* map = AgentMap.Instance();
        if (map == null) return;

        map->IsFlagMarkerSet = 0;
    }

    private void UpdateWidgetButtonState()
    {
        if (!IsFlagMarkerSet()) {
            Element.IsVisible = false;
            _aetheryteEntry   = null;
            _aetheryteKey     = null;
            return;
        }

        Element.IsVisible = true;

        _icon.Style.ImageGrayscale = _player.CanUseTeleportAction && _aetheryteEntry != null ? 0 : 1;
        Element.Style.Opacity      = _player.CanUseTeleportAction ? 1 : 0.5f;
    }

    private unsafe void UpdateWidgetInfoState()
    {
        AgentMap* map = AgentMap.Instance();
        if (map == null) return;

        var cacheKey =
            $"{_zoneManager.CurrentZone.Id}_{map->FlagMapMarker.MapId}_{map->FlagMapMarker.XFloat}_{map->FlagMapMarker.YFloat}";

        if (_aetheryteKey == cacheKey) return;

        if (_lastMarkerKey != cacheKey) {
            _lastMarkerKey = cacheKey;
            _icon.Padding  = new(-16);
            _icon.Animate(new Animation<InOutCirc>(500) { Padding = new(0) });
        }

        IZone   zone = _zoneManager.GetZone(map->FlagMapMarker.MapId);
        Vector2 pos  = MapUtil.WorldToMap(new(map->FlagMapMarker.XFloat, map->FlagMapMarker.YFloat), zone.MapSheet);

        _aetheryteKey = cacheKey;
        _name.Text    = $"{zone.Name} at <{pos.X:F1}, {pos.Y:F1}>";

        var flagPos2D = new Vector2(map->FlagMapMarker.XFloat, map->FlagMapMarker.YFloat);

        // Find all Aetheryte markers in the current zone.
        List<ZoneMarker> aetherytes = zone
            .StaticMarkers
            .Where(m => m.Type == ZoneMarkerType.Aetheryte)
            .ToList();

        // Find the nearest Aetheryte marker to the flag marker, or null if none are nearby.
        ZoneMarker? marker = aetherytes.Count != 0
            ? aetherytes.MinBy(m => Vector2.Distance(new(m.WorldPosition.X, m.WorldPosition.Z), flagPos2D))
            : null;

        // Find the AetheryteEntry that the player has actually unlocked and is able to use.
        _aetheryteEntry = marker != null
            ? _aetheryteList.FirstOrDefault(a => a.AetheryteId == marker.Value.DataId)
            : null;

        // Abort if there is none.
        if (_aetheryteEntry == null) {
            _info.Text = I18N.Translate("LocationWidget.NoAetheryteNearby");
            return;
        }

        var placeName = _aetheryteEntry.AetheryteData.GameData!.PlaceName.Value!.Name.ToString();

        _info.Text = placeName == zone.Name
            ? I18N.Translate("LocationWidget.TeleportNearbyForGil",  _aetheryteEntry.GilCost.ToString("D"))
            : I18N.Translate("LocationWidget.TeleportToPlaceForGil", placeName, _aetheryteEntry.GilCost.ToString("D"));
    }
}
