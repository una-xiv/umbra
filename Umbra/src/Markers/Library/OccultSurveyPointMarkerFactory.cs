using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace Umbra.Markers.Library;

[Service]
public class OccultSurveyPointMarkerFactory(IObjectTable objectTable, IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "OccultSurveyPointMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Occult.SurveyPoints.Name");
    public override string Description { get; } = I18N.Translate("Markers.Occult.SurveyPoints.Description");

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
            zoneManager.CurrentZone.TerritoryId != 1252 // South Horn
        ) {
            RemoveAllMarkers();
            return;
        }
        
        List<string> usedIds = [];

        var showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var fadeDistance    = GetConfigValue<int>("FadeDistance");
        var fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var maxVisDistance  = GetConfigValue<int>("MaxVisibleDistance");

        foreach (var obj in objectTable) {
            if (obj is { ObjectKind: ObjectKind.EventObj }) {
                var id = $"OSP_{obj.Position}";
                usedIds.Add(id);

                SetMarker(new() {
                    Key                = id,
                    MapId              = zoneManager.CurrentZone.Id,
                    IconId             = 60553,
                    Position           = obj.Position + new Vector3(0, 1.8f, 0),
                    Label              = obj.Name.TextValue,
                    ShowOnCompass      = showDirection,
                    FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                    MaxVisibleDistance = maxVisDistance,
                    IsVisible          = true,
                });
            }
        }

        RemoveMarkersExcept(usedIds);
    }
}
