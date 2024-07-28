using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class PartyMemberWorldMarkerFactory(IPlayer player, IPartyList partyList, IZoneManager zoneManager)
    : WorldMarkerFactory
{
    /// <inheritdoc/>
    public override string Id { get; } = "PartyMembers";

    /// <inheritdoc/>
    public override string Name { get; } = I18N.Translate("Markers.PartyMembers.Name");

    /// <inheritdoc/>
    public override string Description { get; } = I18N.Translate("Markers.PartyMembers.Description");

    /// <inheritdoc/>
    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable(
                "ShowIcon",
                I18N.Translate("Markers.PartyMembers.Config.ShowIcon.Name"),
                I18N.Translate("Markers.PartyMembers.Config.ShowIcon.Description"),
                true
            ),
            new BooleanMarkerConfigVariable(
                "ShowName",
                I18N.Translate("Markers.PartyMembers.Config.ShowName.Name"),
                I18N.Translate("Markers.PartyMembers.Config.ShowName.Description"),
                true
            ),
            new IntegerMarkerConfigVariable(
                "FadeDistance",
                I18N.Translate("Settings.MarkersModule.Config.FadeDistance.Name"),
                I18N.Translate("Settings.MarkersModule.Config.FadeDistance.Description"),
                50,
                35,
                1000
            ),
            new IntegerMarkerConfigVariable(
                "FadeAttenuation",
                I18N.Translate("Settings.MarkersModule.Config.FadeAttenuation.Name"),
                I18N.Translate("Settings.MarkersModule.Config.FadeAttenuation.Description"),
                10,
                0,
                100
            ),
        ];
    }

    [OnTick(interval: 16)]
    private void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        if (partyList.Length == 0) {
            RemoveAllMarkers();
            return;
        }

        if (player.IsBetweenAreas) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist = GetConfigValue<int>("FadeDistance");
        var fadeAttn = GetConfigValue<int>("FadeAttenuation");

        List<string> usedKeys      = [];
        Vector3      playerPos     = player.Position;
        bool         showOnCompass = GetConfigValue<bool>("ShowOnCompass");
        bool         showIcon      = GetConfigValue<bool>("ShowIcon");
        bool         showName      = GetConfigValue<bool>("ShowName");
        uint         mapId         = zoneManager.CurrentZone.Id;

        foreach (var member in partyList) {
            if (member.MaxHP == 0 || member.ClassJob.Id == 0) continue; // Probably not in this area.

            // Remove markers for party members that are close to the player.
            if (Vector3.Distance(member.Position, playerPos) < 50) continue;

            string key = $"PM_{mapId}_{member.ContentId}";
            usedKeys.Add(key);

            SetMarker(
                new() {
                    Key           = key,
                    Label         = showName ? member.Name.TextValue : "",
                    IconId        = showIcon ? 62000u + member.ClassJob.Id : 0,
                    Position      = member.Position with { Y = member.Position.Y + 1.5f },
                    MapId         = zoneManager.CurrentZone.Id,
                    FadeDistance  = new(fadeDist, fadeDist + fadeAttn),
                    ShowOnCompass = showOnCompass,
                }
            );
        }

        RemoveMarkersExcept(usedKeys);
    }
}
