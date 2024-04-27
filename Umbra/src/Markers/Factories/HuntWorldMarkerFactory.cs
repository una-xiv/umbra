// FFXIV Empherial UI                       _____           _           _     _
//     An XIV-Launcher plugin              |   __|_____ ___| |_ ___ ___|_|___| |
//                                         |   __|     | . |   | -_|  _| | .'| |
// github.com/empherial/empherial-ui       |_____|_|_|_|  _|_|_|___|_| |_|__,|_|
// --------------------------------------------------- |_|

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.Text;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;

namespace Umbra.Markers;

[Service]
internal sealed class HuntWorldMarkerFactory(IDataManager dataManager) : IWorldMarkerFactory
{
    [ConfigVariable("Markers.Hunt.Enabled", "EnabledMarkers")]
    private static bool Enabled { get; set; } = true;

    private readonly Dictionary<uint, NotoriousMonster?> _notoriousMonstersCache = [];

    private readonly Dictionary<byte, string> _rankPrefixes = new() {
        { 0, SeIconChar.BoxedLetterC.ToIconString() },
        { 1, SeIconChar.BoxedLetterB.ToIconString() },
        { 2, SeIconChar.BoxedLetterA.ToIconString() },
        { 3, SeIconChar.BoxedLetterS.ToIconString() },
        { 4, SeIconChar.BoxedLetterS.ToIconString() + SeIconChar.BoxedLetterS.ToIconString() }
    };

    public unsafe List<WorldMarker> GetMarkers()
    {
        if (false == Enabled) return [];

        List<WorldMarker> markers = [];

        var cm = CharacterManager.Instance();
        if (cm == null) return [..markers];

        foreach (var chara in cm->BattleCharaListSpan) {
            BattleChara* bc = chara.Value;

            if (bc == null
             || 0  == bc->Character.GameObject.DataID)
                continue;

            var nm = GetNotoriousMonster(bc->Character.GameObject.DataID);

            if (nm == null) continue;

            var name = MemoryHelper.ReadSeStringNullTerminated((nint)bc->Character.GameObject.Name).ToString();
            var rank = _rankPrefixes[nm.Rank];
            var p    = bc->Character.GameObject.Position;

            markers.Add(
                new WorldMarker {
                    Position = new Vector3(p.X, p.Y + 1.8f, p.Z),
                    IconId   = GetMarkerIcon(nm.Rank, bc->Character.IsHostile),
                    Label    = $"{rank} {name}{(bc->Character.InCombat ? " (In Combat)" : "")}",

                    MinOpacity      = 0f,
                    MaxOpacity      = 1f,
                    MinFadeDistance = 20f,
                    MaxFadeDistance = 35f,
                }
            );
        }

        return markers;
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
