using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Markers.Library;

[Service]
internal class FlagMarkerFactory : WorldMarkerFactory
{
    public override string Id          { get; } = "FlagMarker";
    public override string Name        { get; } = I18N.Translate("Markers.Flag.Name");
    public override string Description { get; } = I18N.Translate("Markers.Flag.Description");

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
        RemoveAllMarkers();
    }

    [OnTick(interval: 500)]
    private unsafe void OnUpdate()
    {
        var enabled = GetConfigValue<bool>("Enabled");

        AgentMap* agentMap = AgentMap.Instance();

        if (!enabled || agentMap is null || !agentMap->IsFlagMarkerSet) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");
        var key            = $"FlagMarker_{agentMap->FlagMapMarker.MapId}";

        SetMarker(
            new() {
                Key                = key,
                IconId             = agentMap->FlagMapMarker.MapMarker.IconId,
                MapId              = agentMap->FlagMapMarker.MapId,
                Position           = new(agentMap->FlagMapMarker.XFloat, 0, agentMap->FlagMapMarker.YFloat),
                FadeDistance       = new(fadeDist, fadeDist + Math.Max(1, GetConfigValue<int>("FadeAttenuation"))),
                ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                MaxVisibleDistance = maxVisDistance,
            }
        );

        RemoveMarkersExcept([key]);
    }
}
