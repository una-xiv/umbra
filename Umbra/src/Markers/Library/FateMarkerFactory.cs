using Dalamud.Game.Text;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;

namespace Umbra.Markers.Library;

[Service]
internal class FateMarkerFactory(IZoneManager zoneManager) : WorldMarkerFactory
{
    public override string Id          { get; } = "FateMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.Fate.Name");
    public override string Description { get; } = I18N.Translate("Markers.Fate.Description");

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

    [OnTick(interval: 1000)]
    private unsafe void OnUpdate()
    {
        var enabled        = GetConfigValue<bool>("Enabled");
        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        FateManager* fm = FateManager.Instance();

        if (!enabled || !zoneManager.HasCurrentZone || fm is null) {
            RemoveAllMarkers();
            return;
        }

        long now = DateTimeOffset.Now.ToUnixTimeSeconds();

        List<string> activeIds = [];

        foreach (FateContext* fate in fm->Fates.ToList()) {
            if (fate == null) continue;

            long startTime = fate->StartTimeEpoch;
            long endTime   = startTime + fate->Duration;

            if (startTime > 0 && endTime > 0 && (startTime > now || endTime < now)) continue;

            TimeSpan timeLeft = DateTimeOffset.FromUnixTimeSeconds(endTime).Subtract(DateTimeOffset.Now);

            var id       = $"FATE_{fate->FateId}";
            var progress = "";
            var state    = fate->State;

            if (fate->Progress > 0) {
                progress = $" - {fate->Progress}%";
            }

            activeIds.Add(id);
            string prefix = fate->IsBonus ? $"{SeIconChar.BoxedStar.ToIconString()} " : "";

            SetMarker(
                new() {
                    Key                = id,
                    IconId             = fate->IconId,
                    MapId              = zoneManager.CurrentZone.Id,
                    Label              = $"{prefix}{MemoryHelper.ReadSeString(&fate->Name)}",
                    SubLabel           = $"{state} - {timeLeft:mm\\:ss}{progress}",
                    Position           = fate->Location + new Vector3(0, 1.8f, 0),
                    FadeDistance       = new(fadeDist, fadeDist + Math.Max(1, GetConfigValue<int>("FadeAttenuation"))),
                    ShowOnCompass      = GetConfigValue<bool>("ShowOnCompass"),
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }
}
