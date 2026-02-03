using System;
using System.Collections.Generic;
using System.Text;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;

namespace Umbra.Markers.Library;

[Service]
internal class EurekaElementalMarkerFactory(IObjectTable objectTable, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id { get; } = "EurekaElementals";
    public override string Name { get; } = I18N.Translate("Markers.EurekaElementals.Name");
    public override string Description { get; } = I18N.Translate("Markers.EurekaElementals.Description");

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
        if (!GetConfigValue<bool>("Enabled") ||
            !zoneManager.HasCurrentZone ||
            !new[] { 732u, 763u, 795u, 827u }.Contains(zoneManager.CurrentZone.TerritoryId)) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist = GetConfigValue<int>("FadeDistance");
        var fadeAttn = GetConfigValue<int>("FadeAttenuation");
        var showOnCompass = GetConfigValue<bool>("ShowOnCompass");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        List<string> usedIds = [];
        IZone zone = zoneManager.CurrentZone;

        foreach (IGameObject obj in objectTable) {
            if (!obj.IsValid() || !new[] { 8260u, 9178u, 9589u, 10108u }.Contains(obj.BaseId)) continue;

            string key = $"EurekaElemental_{zone.Id}_{(int)obj.Position.X}_{(int)obj.Position.Z}_{obj.BaseId}";
            usedIds.Add(key);

            SetMarker(
                new() {
                    Key = key,
                    MapId = zone.Id,
                    Label = obj.Name.TextValue,
                    IconId = 215835u,
                    IconWidth = 24,
                    IconHeight = 32,
                    Position = obj.Position,
                    FadeDistance = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass = showOnCompass,
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }
}
