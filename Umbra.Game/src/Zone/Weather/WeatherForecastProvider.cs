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
        return I18N.FormatTimeAgo(forecastTime - DateTime.UtcNow);
    }
}
