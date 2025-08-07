using FFXIVClientStructs.FFXIV.Client.UI.Agent;

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

        if (!enabled || agentMap is null || agentMap->FlagMarkerCount == 0) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");
        var key            = $"FlagMarker_{agentMap->FlagMapMarkers[0].MapId}";

        SetMarker(
            new() {
                Key                = key,
                IconId             = agentMap->FlagMapMarkers[0].MapMarker.IconId,
                MapId              = agentMap->FlagMapMarkers[0].MapId,
                Position           = new(agentMap->FlagMapMarkers[0].XFloat, 0, agentMap->FlagMapMarkers[0].YFloat),
                FadeDistance       = new(fadeDist, fadeDist + Math.Max(1, GetConfigValue<int>("FadeAttenuation"))),
                ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                MaxVisibleDistance = maxVisDistance,
            }
        );

        RemoveMarkersExcept([key]);
    }
}
