using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace Umbra.Markers.Library;

[Service]
#pragma warning disable SeStringEvaluator
internal class AetherCurrentsWorldMarkerFactory(IDataManager dataManager, ISeStringEvaluator seStringEvaluator, IZoneManager zoneManager) : WorldMarkerFactory
#pragma warning restore SeStringEvaluator
{
    /// <inheritdoc/>
    public override string Id { get; } = "AetherCurrents";

    /// <inheritdoc/>
    public override string Name { get; } = I18N.Translate("Markers.AetherCurrents.Name");

    /// <inheritdoc/>
    public override string Description { get; } = I18N.Translate("Markers.AetherCurrents.Description");

    private readonly Dictionary<uint, AetherCurrent> _aetherCurrents = [];

    /// <inheritdoc/>
    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    protected override void OnZoneChanged(IZone zone)
    {
        _aetherCurrents.Clear();

        foreach (var list in dataManager.GetExcelSheet<AetherCurrentCompFlgSet>().Where(c => c.Territory.RowId == zone.TerritoryId)) {
            foreach (var ac in list.AetherCurrents) {
                if (!ac.IsValid || ac.Value.Quest.IsValid) continue;

                var eObj = dataManager.GetExcelSheet<EObj>().FirstOrNull(e => e.Data == ac.RowId);
                if (eObj == null) continue;

                var level = dataManager.GetExcelSheet<Level>().FirstOrNull(l => l.Object.RowId == eObj.Value.RowId);
                if (level == null) continue;

                _aetherCurrents.Add(ac.RowId, new(
                    seStringEvaluator.EvaluateFromAddon(2025, [ObjectKind.EventObj.GetObjStrId(eObj.Value.RowId)]).ExtractText().StripSoftHyphen(), 
                    new(level.Value.X, level.Value.Y, level.Value.Z)
                ));
            }
        }
    }

    private uint _lastMapId;

    [OnTick(interval: 16)]
    private unsafe void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        if (zoneManager.CurrentZone.Id != _lastMapId) {
            _lastMapId = zoneManager.CurrentZone.Id;
            RemoveAllMarkers();
        }

        List<string> activeIds = [];

        bool showOnCompass  = GetConfigValue<bool>("ShowOnCompass");
        var  fadeDist       = GetConfigValue<int>("FadeDistance");
        var  fadeAttn       = GetConfigValue<int>("FadeAttenuation");
        var  maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        PlayerState* ps = PlayerState.Instance();

        foreach (var (acId, ac) in _aetherCurrents) {
            string key = $"AC_{_lastMapId}_{acId}";
            activeIds.Add(key);

            if (ps->IsAetherCurrentUnlocked(acId)) continue;
            
            SetMarker(
                new() {
                    Key                = key,
                    Position           = ac.Position,
                    IconId             = 60033u,
                    Label              = ac.Name,
                    MapId              = _lastMapId,
                    FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass      = showOnCompass,
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private struct AetherCurrent(string name, Vector3 pos)
    {
        public readonly string  Name     = name;
        public readonly Vector3 Position = pos;
    }
}
