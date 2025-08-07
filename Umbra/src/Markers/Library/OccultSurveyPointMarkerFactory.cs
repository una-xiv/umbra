using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using EventHandler = FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler;
using Vector3 = FFXIVClientStructs.FFXIV.Common.Math.Vector3;

namespace Umbra.Markers.Library;

[Service]
public class OccultSurveyPointMarkerFactory(IZoneManager zoneManager) : WorldMarkerFactory
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
    private unsafe void OnTick()
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

        GameObjectManager* mgr = GameObjectManager.Instance();
        var count = mgr->Objects.GameObjectIdSortedCount;
        
        if (count == 0) {
            RemoveAllMarkers();
            return;
        }

        for (var i = 0; i < count; i++) {
            GameObject* obj = mgr->Objects.GameObjectIdSorted[i].Value;
            if (obj == null || obj->ObjectKind != ObjectKind.EventObj || !obj->GetIsTargetable()) continue;

            EventHandler* ev = obj->EventHandler;
            if (ev == null || !ev->UnkString0.ToString().Contains("CtsMkdRoaNotePlace")) continue;
            
            var id = $"OSP_{obj->Position}";
            usedIds.Add(id);
            
            SetMarker(new() {
                Key                = id,
                MapId              = zoneManager.CurrentZone.Id,
                IconId             = 60553,
                Position           = obj->Position + new Vector3(0, 1.8f, 0),
                Label              = obj->NameString,
                ShowOnCompass      = showDirection,
                FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                MaxVisibleDistance = maxVisDistance,
                IsVisible          = true,
            });
        }

        RemoveMarkersExcept(usedIds);
    }
}
