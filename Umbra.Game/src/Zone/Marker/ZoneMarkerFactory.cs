/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Sheet = Lumina.Excel.GeneratedSheets;

namespace Umbra.Game;

[Service]
internal sealed class ZoneMarkerFactory(IDataManager dataManager)
{
    // https://xivapi.com/docs/Icons?set=icons060000
    private readonly Dictionary<ZoneMarkerType, uint[]> _typeToIconMap = new() {
        { ZoneMarkerType.Aetheryte, [60453, 60959] },
        { ZoneMarkerType.Aethernet, [60430] },
        { ZoneMarkerType.Area, [60339, 60409, 60410, 60411, 60566] },
        { ZoneMarkerType.Pin, [60408, 60442] },
        { ZoneMarkerType.MapLink, [60441, 60446, 60447, 60457, 60467, 60467, 60971] },
        { ZoneMarkerType.InstanceEntry, [60414, 60428, 60452, 63971, 63972, 63973] },
        { ZoneMarkerType.Settlement, [60448, 60454, 60455] }, {
            ZoneMarkerType.Vendor,
            [60412, 60413, 60417, 60418, 60419, 60427, 60550, 60759, 60768, 60935, 60987, 63919, 60091]
        },
        { ZoneMarkerType.Repair, [60434] },
        { ZoneMarkerType.Melder, [60910] },
        { ZoneMarkerType.RetainerVocate, [60426] },
        { ZoneMarkerType.Marketboard, [60570] },
        { ZoneMarkerType.Inn, [60436, 60559, 60988] },
        { ZoneMarkerType.Mailbox, [60551] },
        { ZoneMarkerType.HuntVendor, [60571] },
        { ZoneMarkerType.Taxi, [60581, 60311] },
        { ZoneMarkerType.Ferry, [60352, 60456] },
        { ZoneMarkerType.ChocoboTender, [60842] },
        { ZoneMarkerType.GoldSaucerEmployee, [60582] },
        { ZoneMarkerType.FashionReportNpc, [60960] },
        { ZoneMarkerType.CombatArea, [60459] }, {
            ZoneMarkerType.TribeArea,
            [
                60600, 60601, 60602, 60603, 60604, 60605, 60606, 60607, 60608, 60609, 60610, 60611, 60612, 60613, 60614,
                60615, 60616, 60617, 60618, 60619
            ]
        },
        { ZoneMarkerType.SpecialistApproval, [60473] },
        { ZoneMarkerType.Stronghold, [60449, 60450, 60451] }, {
            ZoneMarkerType.GatheringNode,
            [60432, 60433, 60437, 60438, 60445, 60461, 60462, 60463, 60464, 60465, 60466]
        },
        { ZoneMarkerType.SummoningBell, [60425, 60560] },
        { ZoneMarkerType.City, [60881, 60882, 60883, 60884, 60885, 60886, 60887, 60888, 60889] }, {
            ZoneMarkerType.JobGuild,
            [
                60318, 60319, 60320, 60321, 60322, 60326, 60330, 60331, 60333, 60334, 60335, 60337, 60342, 60344, 60345,
                60345, 60346, 60347, 60348, 60351, 60362, 60363, 60364, 60844
            ]
        }, {
            ZoneMarkerType.GrandCompany,
            [60567, 60568, 60569, 60572, 60573, 60574, 60576, 60577, 60578, 60871, 60872, 60873]
        },
        { ZoneMarkerType.GrandCompanyChest, [60460] }, {
            ZoneMarkerType.HousingPlot,
            [
                60751, 60752, 60753, 60754, 60755, 60756, 60757, 60758, 60759, 60761, 60762, 60763, 60764, 60765, 60766,
                60767, 60769, 60770, 60771, 60772, 60773, 60774, 60775, 60776, 60777, 60778, 60779, 60780, 60781, 60782,
                60783, 60784, 60785, 60786, 60787, 60788
            ]
        },
        { ZoneMarkerType.AppartmentBuilding, [60789, 60790, 60791, 60792] }, {
            ZoneMarkerType.Fate,
            [
                60458, 60501, 60502, 60503, 60504, 60505, 60506, 60507, 60508, 60721, 60722, 60723, 60724, 60725, 60726,
                60727, 60728, 60958
            ]
        }, {
            ZoneMarkerType.FateObjective,
            [60511, 60512, 60513, 60514, 60515, 60731, 60732, 60733, 60734, 60735, 60736]
        }, {
            ZoneMarkerType.LeveQuest,
            [
                71041, 71042, 71043, 71044, 71045, 71046, 71047, 71048, 71049, 71051, 71052, 71053, 71054, 71055, 71056,
                71057, 71058, 71059, 71081, 71082, 71083, 71084, 71085, 71086, 71087, 71088, 71089, 71091, 71092, 71093,
                71094, 71095, 71096, 71097, 71098, 71099
            ]
        }, {
            ZoneMarkerType.LoreQuest,
            [
                71061, 71062, 71063, 71064, 71065, 71066, 71067, 71068, 71069, 71071, 71072, 71073, 71074, 71075, 71076,
                71077, 71078, 71079
            ]
        }, {
            ZoneMarkerType.QuestObjective,
            [
                71003, 71004, 71005, 71006, 71023, 71024, 71025, 71026, 71043, 71044, 71045, 71046, 71063, 71064, 71065,
                71066, 71083, 71084, 71085, 71086, 71123, 71124, 71125, 71126, 71143, 71144, 71145, 71203, 71223, 71225,
                71243, 71244, 71245, 71263, 71264, 71265, 71283, 71284, 71285, 71286, 71312, 71313, 71323, 71324, 71325,
                71343, 71344, 71345, 71346, 70961, 70962, 70963, 70964, 70965, 70966, 70967, 70968, 70969, 70970, 70971,
                70972, 70973, 70974, 70975, 70976, 70977, 70978, 70979, 70980, 70981, 70982, 70983, 70984, 70985, 70986,
                70987, 70988, 70989, 70990, 70991, 70992, 70993, 70994, 70995, 70996, 70997, 70998, 70999
            ]
        }, {
            ZoneMarkerType.Quest,
            [
                71001, 71002, 71003, 71004, 71005, 71006, 71007, 71008, 71009, 71011, 71012, 71013, 71014, 71015, 71016,
                71017, 71018, 71019, 71021, 71022, 71023, 71024, 71025, 71026, 71027, 71028, 71029, 71031, 71032, 71033,
                71034, 71035, 71036, 71037, 71038, 71039, 71111, 71112, 71113, 71114, 71115, 71116, 71117, 71118, 71119
            ]
        }, {
            ZoneMarkerType.FeatureQuest,
            [
                71141, 71142, 71143, 71144, 71145, 71146, 71147, 71148, 71149, 71151, 71152, 71153, 71154, 71155, 71156,
                71157, 71158, 71159
            ]
        }, {
            ZoneMarkerType.QuestLink,
            [
                71061, 71062, 71063, 71064, 71065, 71066, 71067, 71068, 71069, 71071, 71072, 71073, 71074, 71075, 71076,
                71077, 71078, 71079
            ]
        }, {
            ZoneMarkerType.ObjectiveArea,
            [60490, 60491, 60492, 60493, 60494, 60495, 60496, 60497, 60498, 60499]
        },
        { ZoneMarkerType.TripleTriad, [71101, 71102, 71103, 71104, 71105, 71106, 71107, 71108, 71109] }, {
            ZoneMarkerType.Smith,
            [
                71121, 71122, 71123, 71124, 71125, 71126, 71127, 71128, 71129, 71131, 71132, 71133, 71134, 71135, 71136,
                71137, 71138, 71139
            ]
        }, {
            ZoneMarkerType.Event,
            [
                63900, 63901, 63902, 63903, 63904, 63905, 63906, 63907, 63908, 63909, 63910, 63911, 63912, 63913, 63914,
                63915, 63916, 63917, 63918, 63920, 63921, 63922, 63923, 63924, 63925, 63926, 63927, 63928, 63929, 63930,
                63931, 63932
            ]
        },
        { ZoneMarkerType.WondrousTails, [60926] },
        { ZoneMarkerType.CustomDeliveries, [60927, 60928, 60986] },
    };

    private readonly Dictionary<uint, ZoneMarkerType> _iconToTypeMap = [];

    public ZoneMarker FromMinimapGatheringMarker(Sheet.Map map, MiniMapGatheringMarker marker)
    {
        var type = DetermineMarkerType(marker.MapMarker.IconId, "");

        int x = marker.MapMarker.X;
        int y = marker.MapMarker.Y;

        Vector3 worldPosition = new(x / 16, 0, y / 16);
        Vector2 mapPosition   = MapUtil.WorldToMap(new(worldPosition.X, worldPosition.Z), map);

        return new(
            type,
            marker.TooltipText.ToString(),
            mapPosition,
            worldPosition,
            marker.MapMarker.IconId,
            0
        );
    }

    public unsafe ZoneMarker FromMapMarkerData(Sheet.Map map, MapMarkerData data)
    {
        var position = MapUtil.WorldToMap(new(data.X, data.Z), map);
        var name     = SanitizeMarkerName(data.TooltipString->ToString());
        var type     = DetermineMarkerType(data.IconId, name);

        return new(
            type,
            name,
            position,
            new(data.X, data.Y + 2f, data.Z),
            type is ZoneMarkerType.Area or ZoneMarkerType.ObjectiveArea ? 0u : data.IconId,
            data.ObjectiveId,
            data.Radius
        );
    }

    public ZoneMarker FromMiniMapMarker(Sheet.Map map, MiniMapMarker marker)
    {
        var type = DetermineMarkerType(marker.MapMarker.IconId, "");

        return new ZoneMarker(
            type,
            "",
            new Vector2(marker.MapMarker.X, marker.MapMarker.Y),
            MarkerToWorldPosition(map, new Vector2(marker.MapMarker.X, marker.MapMarker.Y)),
            type is ZoneMarkerType.Area or ZoneMarkerType.ObjectiveArea ? 0u : marker.MapMarker.IconId,
            0
        );
    }

    public ZoneMarker FromMapMarkerSheet(Sheet.Map map, Sheet.MapMarker marker)
    {
        var position = new Vector2(marker.X, marker.Y);
        var name     = GetStaticMarkerName(marker);
        var type     = DetermineMarkerType(marker.Icon, name);

        return new ZoneMarker(
            type,
            name,
            position,
            MarkerToWorldPosition(map, position),
            type == ZoneMarkerType.Area ? 0u : marker.Icon,
            marker.DataKey
        );
    }

    private ZoneMarkerType DetermineMarkerType(uint iconId, string name)
    {
        if (_iconToTypeMap.TryGetValue(iconId, out var cachedType)) return cachedType;

        foreach (var (type, iconIds) in _typeToIconMap) {
            if (iconIds.Contains(iconId)) {
                _iconToTypeMap[iconId] = type;
                return type;
            }
        }

        if (iconId == 0 && !string.IsNullOrEmpty(name)) return ZoneMarkerType.Area;

        return ZoneMarkerType.Unknown;
    }

    private static Vector3 MarkerToWorldPosition(Sheet.Map map, Vector2 pos)
    {
        var v = pos.ToVector3();

        v.X = ((v.X - 1024f) / (map.SizeFactor / 100.0f)) - (map.OffsetX * (map.SizeFactor / 100.0f));
        v.Z = ((v.Z - 1024f) / (map.SizeFactor / 100.0f)) - (map.OffsetY * (map.SizeFactor / 100.0f));

        return v;
    }

    private string GetStaticMarkerName(Sheet.MapMarker marker)
    {
        var label = marker.PlaceNameSubtext.Value?.Name.ToDalamudString().TextValue ?? "";
        if (!string.IsNullOrEmpty(label)) return SanitizeMarkerName(label);
        if (marker.Icon == 0) return "";

        if (marker.DataType == 4) {
            var placeName = dataManager.GetExcelSheet<Sheet.PlaceName>()!.GetRow(marker.DataKey);
            if (placeName != null) return SanitizeMarkerName(placeName.Name.ToDalamudString().TextValue);
        }

        var symbol = dataManager.GetExcelSheet<Sheet.MapSymbol>()!.GetRow(marker.Icon);
        if (symbol == null) return "";

        return SanitizeMarkerName(symbol.PlaceName.Value?.Name.ToDalamudString().TextValue ?? "");
    }

    private static string SanitizeMarkerName(string name)
    {
        // If name solely consists of digits, prefix it with "Plot ".
        if (name.All(char.IsDigit)) return $"Plot {name}";

        return name;
    }
}