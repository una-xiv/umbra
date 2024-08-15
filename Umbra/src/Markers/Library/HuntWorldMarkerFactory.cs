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
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class HuntWorldMarkerFactory(IDataManager dataManager, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "HuntMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Hunt.Name");
    public override string Description { get; } = I18N.Translate("Markers.Hunt.Description");

    private readonly Dictionary<uint, NotoriousMonster?> _notoriousMonstersCache = [];

    private readonly Dictionary<byte, string> _rankPrefixes = new() {
        { 1, "[B]" },
        { 2, "[A]" },
        { 3, "[S]" },
        { 4, "[SS]" }
    };

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable("ShowB",  I18N.Translate("Markers.Hunt.ShowB"),  null, false),
            new BooleanMarkerConfigVariable("ShowA",  I18N.Translate("Markers.Hunt.ShowA"),  null, true),
            new BooleanMarkerConfigVariable("ShowS",  I18N.Translate("Markers.Hunt.ShowS"),  null, true),
            new BooleanMarkerConfigVariable("ShowSS", I18N.Translate("Markers.Hunt.ShowSS"), null, true),
            ..DefaultFadeConfigVariables,
        ];
    }

    [OnTick(interval: 100)]
    private unsafe void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        var cm = CharacterManager.Instance();
        if (cm == null) return;

        var fadeDist = GetConfigValue<int>("FadeDistance");
        var fadeAttn = GetConfigValue<int>("FadeAttenuation");

        List<string> activeIds = [];

        foreach (var chara in cm->BattleCharas) {
            BattleChara* bc = chara.Value;

            if (bc == null || 0 == bc->Character.GameObject.BaseId) continue;

            var nm = GetNotoriousMonster(bc->Character.GameObject.BaseId);
            if (nm == null) continue;

            string name = bc->Character.GameObject.NameString;
            string rank = _rankPrefixes[nm.Rank];

            switch (nm.Rank) {
                case 0:
                case 1 when !GetConfigValue<bool>("ShowB"):
                case 2 when !GetConfigValue<bool>("ShowA"):
                case 3 when !GetConfigValue<bool>("ShowS"):
                case 4 when !GetConfigValue<bool>("ShowSS"):
                    continue;
            }

            var p  = bc->Character.GameObject.Position;
            var id = $"NM_{bc->Character.GameObject.BaseId}";

            activeIds.Add(id);

            RemoveAllMarkers();
            SetMarker(
                new() {
                    Key           = id,
                    MapId         = zoneManager.CurrentZone.Id,
                    Position      = new(p.X, p.Y + bc->Character.CalculateHeight(), p.Z),
                    IconId        = GetMarkerIcon(nm.Rank, bc->Character.IsHostile),
                    Label         = $"{rank} {name}",
                    SubLabel      = bc->Character.InCombat ? " (In Combat)" : null,
                    FadeDistance  = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass = GetConfigValue<bool>("ShowOnCompass")
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private NotoriousMonster? GetNotoriousMonster(uint dataId)
    {
        if (_notoriousMonstersCache.TryGetValue(dataId, out var nm)) return nm;

        NotoriousMonster? monster = dataManager.GetExcelSheet<NotoriousMonster>()!
            .FirstOrDefault(n => n.BNpcBase.Row == dataId);

        _notoriousMonstersCache[dataId] = monster;

        return monster;
    }

    private static uint GetMarkerIcon(byte rank, bool isHostile)
    {
        uint startOffset = (uint)(isHostile ? 61706 : 61700);
        uint iconOffset  = (uint)Math.Min(rank + 3, 4);

        return startOffset + iconOffset;
    }
}
