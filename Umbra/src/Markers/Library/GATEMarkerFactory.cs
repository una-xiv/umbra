using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.GoldSaucer;
using Level = Lumina.Excel.Sheets.Level;

namespace Umbra.Markers.Library;

[Service]
internal class GateMarkerFactory : WorldMarkerFactory
{
    private readonly IDataManager dataManager;
    private readonly IZoneManager zoneManager;

    public GateMarkerFactory(IDataManager dataManager, IZoneManager zoneManager)
    {
        this.dataManager = dataManager;
        this.zoneManager = zoneManager;
    }

    public override string Id          { get; } = "GateMarker";
    public override string Name        { get; } = I18N.Translate("Markers.Gate.Name");
    public override string Description { get; } = I18N.Translate("Markers.Gate.Description");

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

    [OnTick(interval: 100)]
    private unsafe void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled")) {
            RemoveAllMarkers();
            return;
        }

        if (!zoneManager.HasCurrentZone || zoneManager.CurrentZone.Type != TerritoryIntendedUse.GoldSaucer) {
            RemoveAllMarkers();
            return;
        }

        var director = GoldSaucerManager.Instance()->CurrentGFateDirector;
        if (director == null || director->Flags.HasFlag(GFateDirectorFlag.IsJoined) || director->Flags.HasFlag(GFateDirectorFlag.IsFinished)) {
            RemoveAllMarkers();
            return;
        }

        if (!dataManager.GetExcelSheet<Level>().TryGetRow(director->MapMarkerLevelId, out var level)) {
            RemoveAllMarkers();
            return;
        }

        var fadeDist = GetConfigValue<int>("FadeDistance");
        var fadeAttn = GetConfigValue<int>("FadeAttenuation");

        var id = $"GFATE_{director->GetEventId().Id}";
        var label = director->MapMarkerTooltipText.AsReadOnlySeStringSpan().ToString();
        var endTime = DateTimeOffset.FromUnixTimeSeconds(director->EndTimestamp).DateTime;
        var subLabel = endTime >= DateTime.UtcNow
            ? (endTime - DateTime.UtcNow).ToString("mm\\:ss")
            : string.Empty;

        SetMarker(
            new() {
                Key = id,
                MapId = zoneManager.CurrentZone.Id,
                Position = new(level.X, level.Y, level.Z),
                IconId = 71311,
                Label = label,
                SubLabel = subLabel,
                FadeDistance = new(fadeDist, fadeDist + fadeAttn),
                ShowOnCompass = GetConfigValue<bool>("ShowOnCompass"),
                MaxVisibleDistance = GetConfigValue<int>("MaxVisibleDistance"),
            }
        );

        RemoveMarkersExcept([id]);
    }
}
