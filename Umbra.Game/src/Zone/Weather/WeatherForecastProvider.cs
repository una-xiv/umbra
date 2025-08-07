namespace Umbra.Game;

[Service]
internal unsafe class WeatherForecastProvider
{
    private const double Seconds       = 1;
    private const double Minutes       = 60 * Seconds;
    private const double WeatherPeriod = 23 * Minutes + 20 * Seconds;

    private readonly IDataManager _dataManager;

    public WeatherForecastProvider(IDataManager dataManager, IGameInteropProvider interopProvider)
    {
        _dataManager = dataManager;
        interopProvider.InitializeFromAttributes(this);
    }

    public List<WeatherForecast> GetWeatherForecast(ushort territoryId)
    {
        WeatherManager* wm = WeatherManager.Instance();
        if (null == wm) return [];

        byte currentWeatherId = wm->GetCurrentWeather();

        Weather currentWeather = _dataManager.GetExcelSheet<Weather>().GetRow(currentWeatherId);
        Weather lastWeather    = currentWeather;

        List<WeatherForecast> result = [BuildResultObject(currentWeather, GetRootTime(0))];

        try {
            for (var i = 1; i < 24; i++) {
                byte weatherId = wm->GetWeatherForDaytime(territoryId, i);
                var  weather   = _dataManager.GetExcelSheet<Weather>().FindRow(weatherId)!;
                var  time      = GetRootTime(i * WeatherPeriod);

                if (lastWeather.RowId != weather.Value.RowId) {
                    lastWeather = weather.Value;
                    result.Add(BuildResultObject(weather.Value, time));
                }
            }
        } catch (Exception e) {
            Logger.Error(e.Message);
        }

        return result;
    }

    private static WeatherForecast BuildResultObject(Weather weather, DateTime time)
    {
        var timeString = FormatForecastTime(time);
        var name       = weather.Name.ExtractText();
        var iconId     = (uint)weather.Icon;

        return new(time, timeString, name, iconId);
    }

    private static DateTime GetRootTime(double initialOffset)
    {
        var now      = DateTime.UtcNow;
        var rootTime = now.AddMilliseconds(-now.Millisecond).AddSeconds(initialOffset);
        var seconds  = (long)(rootTime - DateTime.UnixEpoch).TotalSeconds % WeatherPeriod;

        rootTime = rootTime.AddSeconds(-seconds);

        return rootTime;
    }

    private static string FormatForecastTime(DateTime forecastTime)
    {
        TimeSpan timeDifference = forecastTime - DateTime.UtcNow;
        double   totalMinutes   = timeDifference.TotalMinutes;

        switch (totalMinutes) {
            case <= 0.01:
                return I18N.Translate("WeatherForecast.Now");
            case < 1:
                return $"{I18N.Translate("WeatherForecast.LessThan")} {I18N.Translate("WeatherForecast.AMinute")}";
            case < 2:
                return $"{I18N.Translate("WeatherForecast.AMinute")}";
            case < 60:
                return $"{I18N.Translate("WeatherForecast.XMinutes", (int)totalMinutes)}";
        }

        var hours            = (int)(totalMinutes / 60);
        var remainingMinutes = (int)(totalMinutes % 60);

        if (remainingMinutes == 0)
            return hours == 1
                ? I18N.Translate("WeatherForecast.AnHour")
                : I18N.Translate("WeatherForecast.XHours", hours);

        string hoursStr = hours == 1
            ? I18N.Translate("WeatherForecast.AnHour")
            : I18N.Translate("WeatherForecast.XHours", hours);

        string minutesStr = remainingMinutes == 1
            ? I18N.Translate("WeatherForecast.AMinute")
            : I18N.Translate("WeatherForecast.XMinutes", remainingMinutes);

        return $"{hoursStr} {I18N.Translate("WeatherForecast.And")} {minutesStr}";
    }
}
