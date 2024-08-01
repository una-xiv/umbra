using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class TreasureCofferMarkerFactory(IObjectTable objectTable, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "TreasureCoffers";
    public override string Name        { get; } = I18N.Translate("Markers.TreasureCoffer.Name");
    public override string Description { get; } = I18N.Translate("Markers.TreasureCoffer.Description");

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    [OnTick(interval: 500)]
    private void OnTick()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist      = GetConfigValue<int>("FadeDistance");
        var fadeAttn      = GetConfigValue<int>("FadeAttenuation");
        var showOnCompass = GetConfigValue<bool>("ShowOnCompass");

        List<string> usedIds = [];
        IZone        zone    = zoneManager.CurrentZone;

        foreach (IGameObject obj in objectTable) {
            if (!obj.IsValid() || obj.ObjectKind != ObjectKind.Treasure) continue;

            string key = $"TC_{zone.Id}_{(int)obj.Position.X}_{(int)obj.Position.Z}_{obj.DataId}";
            usedIds.Add(key);

            SetMarker(
                new() {
                    Key           = key,
                    MapId         = zone.Id,
                    Label         = obj.Name.TextValue,
                    IconId        = 60356u,
                    Position      = obj.Position,
                    FadeDistance  = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass = showOnCompass,
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }
}
