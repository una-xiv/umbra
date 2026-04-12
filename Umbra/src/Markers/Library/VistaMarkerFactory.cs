using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Umbra.Markers.Library;

[Service]
public class VistaMarkerFactory : WorldMarkerFactory, IDisposable
{
    public override string Id          { get; } = "Vista";
    public override string Name        { get; } = I18N.Translate("Markers.Vista.Name");
    public override string Description { get; } = I18N.Translate("Markers.Vista.Description");

    private Dictionary<uint, List<Adventure>> VistaByMap  { get; } = [];
    private IDataManager                      DataManager { get; }
    private IZoneManager                      ZoneManager { get; }

    /// <inheritdoc/>
    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            ..DefaultFadeConfigVariables,
        ];
    }

    public VistaMarkerFactory(IDataManager dataManager, IZoneManager zoneManager)
    {
        DataManager = dataManager;
        ZoneManager = zoneManager;

        foreach (var vista in dataManager.GetExcelSheet<Adventure>()) {
            uint mapId = vista.Level.Value.Map.RowId;

            if (!VistaByMap.TryGetValue(mapId, out var list)) {
                VistaByMap[mapId] = list = ( []);
            }

            list.Add(vista);
        }
    }

    public override void Dispose()
    {
        VistaByMap.Clear();
        base.Dispose();
    }

    [OnTick(interval: 1000)]
    private unsafe void OnTick()
    {
        if (!ZoneManager.HasCurrentZone || !GetConfigValue<bool>("Enabled")) {
            RemoveAllMarkers();
            return;
        }

        PlayerState* ps              = PlayerState.Instance();
        uint         mapId           = ZoneManager.CurrentZone.Id;
        var          showDirection   = GetConfigValue<bool>("ShowOnCompass");
        var          fadeDistance    = GetConfigValue<int>("FadeDistance");
        var          fadeAttenuation = GetConfigValue<int>("FadeAttenuation");
        var          maxVisDistance  = GetConfigValue<int>("MaxVisibleDistance");

        List<string>    usedIds = [];
        List<Adventure> vistas  = VistaByMap.GetValueOrDefault(mapId, []);

        foreach (var vista in vistas) {
            if (IsAdventureComplete(vista)) continue;

            string id = $"Vista_{vista.RowId}";
            usedIds.Add(id);

            Level? level = vista.Level.Value;
            if (level == null) continue;

            string subLabel = "";

            if (!IsVistaLogUnlocked(vista)) {
                subLabel += (vista.RowId - 2162688) switch {
                    < 20 => $"{I18N.Translate("Markers.Vista.Missing")} ",
                    _ => $"{I18N.Translate("Markers.Vista.Locked")} ",
                };
            }
            if (!IsVistaTime(vista) && vista is { MinTime: > 0, MaxTime: > 0 }) {
                subLabel += GetVistaTime(vista);
            }

            if (!IsVistaWeather(vista) && AdventureToWeatherIds.TryGetValue(vista.RowId, out var weatherIds)) {
                var weatherString = string.Join(
                    ", ",
                    weatherIds
                        .OrderBy(i => i)
                        .Select(i => DataManager.GetExcelSheet<Weather>().FindRow(i))
                        .Where(weather => weather != null)
                        .Cast<Weather>()
                        .Select(weather => weather.Name)
                );

                subLabel += $" ({weatherString})";
            }

            if (vista.Emote.RowId > 0) {
                var emote = vista.Emote.Value;

                if (emote.TextCommand.ValueNullable != null) {
                    subLabel += $" {emote.TextCommand.Value.Command.ExtractText()}";
                }
            }

            subLabel = subLabel.Trim();

            SetMarker(
                new() {
                    Key                = id,
                    Position           = new(level.Value.X, level.Value.Y, level.Value.Z),
                    IconId             = 66413,
                    Label              = $"{vista.Name.ExtractText()}",
                    SubLabel           = string.IsNullOrEmpty(subLabel) ? null : subLabel,
                    MapId              = mapId,
                    FadeDistance       = new(fadeDistance, fadeDistance + fadeAttenuation),
                    ShowOnCompass      = showDirection,
                    MaxVisibleDistance = maxVisDistance,
                    IsDisabled         = !IsVistaActive(vista),
                }
            );
        }

        RemoveMarkersExcept(usedIds);
    }

    private static string GetVistaTime(Adventure vista)
    {
        if (vista is { MinTime: 0, MaxTime: 0 }) return "";

        TimeSpan? start = ConvertAdventureTime(vista.MinTime);
        TimeSpan? end   = ConvertAdventureTime(vista.MaxTime);

        string? startTime = start?.ToString(@"hh\:mm");
        string? endTime   = end?.ToString(@"hh\:mm");

        if (startTime == null || endTime == null) return "";
        return $"[{startTime} - {endTime}]";
    }

    private static TimeSpan? ConvertAdventureTime(ushort timeNum)
    {
        string str = timeNum.ToString().PadLeft(4, '0');

        if (!byte.TryParse(str[..2], out var hours) || !byte.TryParse(str[2..], out var mins)) {
            return null;
        }

        return new(hours, mins, 0);
    }

    private bool IsVistaActive(Adventure vista)
    {
        return IsVistaLogUnlocked(vista) && IsVistaTime(vista) && IsVistaWeather(vista);
    }

    private static unsafe DateTime GetEorzeaTime()
    {
        var fw = Framework.Instance();

        if (fw == null) {
            return DateTime.MinValue;
        }

        long eorzeaTime = fw->ClientTime.EorzeaTime;
        long hours      = eorzeaTime / 3600 % 24;
        long minutes    = eorzeaTime / 60   % 60;
        long seconds    = eorzeaTime        % 60;

        return new(1, 1, 1, (int)hours, (int)minutes, (int)seconds);
    }

    private static bool IsVistaTime(Adventure vista)
    {
        if (vista.MinTime == 0 && vista.MaxTime == 0) return true;

        TimeSpan? start = ConvertAdventureTime(vista.MinTime);
        TimeSpan? end   = ConvertAdventureTime(vista.MaxTime);

        if (start == null || end == null) return true;

        TimeSpan now = GetEorzeaTime().TimeOfDay;

        if (start <= end) return start <= now && now <= end;
        else return now >= start || now <= end;
    }

    private bool IsVistaWeather(Adventure vista)
    {
        if (!ZoneManager!.HasCurrentZone) return false;
        var zone = ZoneManager.CurrentZone;

        var currentWeather = zone.CurrentWeather;
        if (currentWeather == null) return false;

        if (!AdventureToWeatherIds.TryGetValue(vista.RowId, out var weatherIds)) {
            // If no weather requirement, consider it valid
            return true;
        }

        return weatherIds.Contains(currentWeather.RowId);
    }

    private unsafe bool IsAdventureComplete(Adventure adv)
    {
        uint id = adv.RowId - 2162688;

        PlayerState* ps = PlayerState.Instance();
        return ps != null && ps->IsAdventureComplete(id);
    }

    private unsafe bool IsVistaLogUnlocked(Adventure adv)
    {
        uint id = adv.RowId - 2162688;

        PlayerState* ps = PlayerState.Instance();

        if (ps == null) return false;

        return ps->SightseeingLogUnlockState switch {
            0 => false,
            1 => id < 20 || id >= 80, // ARR first 20
            _ => true,
        };
    }

    private static readonly IReadOnlyDictionary<uint, uint[]> AdventureToWeatherIds = new Dictionary<uint, uint[]> {
        [2162688] = [2, 1],
        [2162689] = [1, 2],
        [2162690] = [7, 8],
        [2162691] = [2, 1],
        [2162692] = [3],
        [2162693] = [2, 1],
        [2162694] = [4],
        [2162695] = [2, 1],
        [2162696] = [3],
        [2162697] = [1, 2],
        [2162698] = [2, 1],
        [2162699] = [2, 1],
        [2162700] = [1, 2],
        [2162701] = [2, 1],
        [2162702] = [3],
        [2162703] = [2, 1],
        [2162704] = [4],
        [2162705] = [7, 8],
        [2162706] = [3],
        [2162707] = [2, 1],
        [2162708] = [2, 1],
        [2162709] = [1, 2],
        [2162710] = [7, 8],
        [2162711] = [1, 2],
        [2162712] = [7, 8],
        [2162713] = [1, 2],
        [2162714] = [6],
        [2162715] = [2, 1],
        [2162716] = [1, 2],
        [2162717] = [2, 1],
        [2162718] = [1, 2],
        [2162719] = [10],
        [2162720] = [2, 1],
        [2162721] = [3],
        [2162722] = [1, 2],
        [2162723] = [7, 8],
        [2162724] = [2, 1],
        [2162725] = [7, 8],
        [2162726] = [7, 8],
        [2162727] = [1, 2],
        [2162728] = [2, 1],
        [2162729] = [1, 2],
        [2162730] = [9],
        [2162731] = [10],
        [2162732] = [1, 2],
        [2162733] = [4],
        [2162734] = [2, 1],
        [2162735] = [2, 1],
        [2162736] = [1, 2],
        [2162737] = [3],
        [2162738] = [1, 2],
        [2162739] = [2, 1],
        [2162740] = [11],
        [2162741] = [1, 2],
        [2162742] = [1, 2],
        [2162743] = [2, 1],
        [2162744] = [8],
        [2162745] = [4],
        [2162746] = [2, 1],
        [2162747] = [14],
        [2162748] = [2, 1],
        [2162749] = [14],
        [2162750] = [1, 2],
        [2162751] = [2, 1],
        [2162752] = [2, 1],
        [2162753] = [3],
        [2162754] = [4],
        [2162755] = [1, 2],
        [2162756] = [4],
        [2162757] = [16],
        [2162758] = [2, 1],
        [2162759] = [1, 2],
        [2162760] = [16, 15],
        [2162761] = [2, 1],
        [2162762] = [2, 1],
        [2162763] = [1, 2],
        [2162764] = [17],
        [2162765] = [2, 1],
        [2162766] = [1, 2],
        [2162767] = [2, 1],
    };
}
