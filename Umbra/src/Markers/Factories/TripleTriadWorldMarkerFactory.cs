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
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers;

[Service]
internal sealed class TripleTriadWorldMarkerFactory : IWorldMarkerFactory
{
    [ConfigVariable("Markers.TripleTriad.Enabled", "Enabled Markers", "Show unobtained Triple Triad card markers")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable(
        "Markers.TripleTriad.ShowLockedCards",
        "Marker Settings",
        "Show Locked Triple Triad Cards",
        "Show markers for unobtained cards that require a quest to unlock."
    )]
    private static bool ShowLockedCards { get; set; } = false;

    private readonly Dictionary<uint, List<Card>> _cardLocations = [];

    private readonly IDataManager _dataManager;
    private readonly IZoneManager _zoneManager;

    public unsafe TripleTriadWorldMarkerFactory(IDataManager dataManager, IZoneManager zoneManager)
    {
        _dataManager = dataManager;
        _zoneManager = zoneManager;

        UIState* ui = UIState.Instance();
        if (ui == null) return;

        _dataManager.GetExcelSheet<TripleTriadCardResident>()!
            .ToList()
            .ForEach(
                (resident) => {
                    var card = _dataManager.GetExcelSheet<TripleTriadCard>()!.GetRow(resident.RowId);

                    if (card                 == null
                     || card.Name.ToString() == string.Empty)
                        return;

                    if (ui->IsTripleTriadCardUnlocked((ushort)card.RowId)) return;

                    // Only show cards that we can infer a valid location from.
                    if (resident.Location < 1
                     || _dataManager.GetExcelSheet<Level>()!.GetRow(resident.Location) is not { } level)
                        return;

                    var position = new Vector3(level.X, level.Y + 1.25f, level.Z);
                    var mapId    = level.Map.Row;
                    var name     = card.Name.ToString();

                    var starCount           = resident.TripleTriadCardRarity.Value?.Stars ?? 0;
                    if (starCount > 0) name += $" ({new string('★', starCount)})";

                    if (!_cardLocations.ContainsKey(mapId)) _cardLocations[mapId] = [];

                    _cardLocations[mapId]
                        .Add(
                            new Card {
                                Id            = card.RowId,
                                Name          = name,
                                Position      = position,
                                UnlockQuestId = resident.Quest.Row,
                            }
                        );
                }
            );
    }

    public unsafe List<WorldMarker> GetMarkers()
    {
        if (false == Enabled
         || !_cardLocations.TryGetValue(_zoneManager.CurrentZone.Id, out List<Card>? cards))
            return [];

        var uiState = UIState.Instance();
        var markers = new List<WorldMarker>();

        foreach (var card in cards) {
            if (uiState->IsTripleTriadCardUnlocked((ushort)card.Id)) continue;

            bool isLocked = !QuestManager.IsQuestComplete(card.UnlockQuestId);
            if (isLocked && !ShowLockedCards) continue;

            var     name     = $"{card.Name}";
            string? subLabel = null;

            if (isLocked) {
                subLabel = "Quest Required: "
                  + (_dataManager.GetExcelSheet<Quest>()!.GetRow(card.UnlockQuestId)?.Name.ToString()
                     ?? "Unknown Quest");
            }

            name = string.Join("\n", name.Split('\n').Distinct());

            markers.Add(
                new WorldMarker {
                    IconId   = 71102,
                    Label    = name,
                    SubLabel = subLabel,
                    Position = card.Position,

                    MinOpacity      = 0f,
                    MaxOpacity      = isLocked ? 0.66f : 1f,
                    MinFadeDistance = 0,
                    MaxFadeDistance = 15,
                }
            );
        }

        return markers;
    }

    private struct Card
    {
        public uint    Id;
        public string  Name;
        public Vector3 Position;
        public uint    UnlockQuestId;
    }
}
