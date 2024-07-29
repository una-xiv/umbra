using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class AetherCurrentsWorldMarkerFactory(IObjectTable objectTable, IZoneManager zoneManager) : WorldMarkerFactory
{
    /// <inheritdoc/>
    public override string Id { get; } = "AetherCurrents";

    /// <inheritdoc/>
    public override string Name { get; } = I18N.Translate("Markers.AetherCurrents.Name");

    /// <inheritdoc/>
    public override string Description { get; } = I18N.Translate("Markers.AetherCurrents.Description");

    /// <inheritdoc/>
    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
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

        bool showOnCompass = GetConfigValue<bool>("ShowOnCompass");
        var  fadeDist      = GetConfigValue<int>("FadeDistance");
        var  fadeAttn      = GetConfigValue<int>("FadeAttenuation");

        PlayerState* ps = PlayerState.Instance();

        foreach (IGameObject gameObject in objectTable) {
            if (!gameObject.IsValid()) continue;

            var obj = (GameObject*)gameObject.Address;
            if (obj == null || obj->EventHandler == null) continue;

            if (obj->EventHandler->Info.EventId.ContentId != EventHandlerType.AetherCurrent) continue;

            uint aetherCurrentId = obj->EventHandler->Info.EventId;
            if (ps->IsAetherCurrentUnlocked(aetherCurrentId) || (obj->TargetableStatus & ObjectTargetableFlags.IsTargetable) == 0) continue;

            string key = $"AC_{_lastMapId}_{aetherCurrentId}";
            activeIds.Add(key);

            SetMarker(
                new() {
                    Key           = key,
                    Position      = obj->Position,
                    IconId        = 60033u,
                    Label         = obj->NameString,
                    MapId         = _lastMapId,
                    FadeDistance  = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass = showOnCompass,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }
}
