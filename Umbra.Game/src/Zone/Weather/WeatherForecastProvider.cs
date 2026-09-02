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

    public void UpdateWeatherForecast(List<WeatherForecast> list, ushort territoryId)
    {
        WeatherManager* wm = WeatherManager.Instance();
        if (null == wm) {
            list.Clear();
            return;
        }

        byte currentWeatherId  = wm->GetCurrentWeather();
        Weather currentWeather = _dataManager.GetExcelSheet<Weather>().GetRow(currentWeatherId);
        Weather lastWeather    = currentWeather;

        void UpdateForecastEntry(int index, Weather weather, DateTime time)
        {
            var timeString = FormatForecastTime(time);
            var name       = weather.Name.ToString();
            var iconId     = (uint)weather.Icon;

            if (index < list.Count) {
                list[index].Update(time, timeString, name, iconId);
            } else {
                list.Add(new WeatherForecast(time, timeString, name, iconId));
            }
        }

        UpdateForecastEntry(0, currentWeather, GetRootTime(0));

        try {
            var index = 1;
            for (; index < 24; index++) {
                byte weatherId = wm->GetWeatherForDaytime(territoryId, index);
                var weather    = _dataManager.GetExcelSheet<Weather>().FindRow(weatherId)!;
                var time       = GetRootTime(index * WeatherPeriod);

                if (lastWeather.RowId != weather.Value.RowId) {
                    lastWeather = weather.Value;
                    UpdateForecastEntry(index, weather.Value, time);
                }
            }

            if (list.Count > index) {
                list.RemoveRange(index, list.Count - index);
            }
        } catch (Exception e) {
            Logger.Error(e.Message);
        }
    }

    private static DateTime GetRootTime(double initialOffset)
    {
        var now      = DateTime.UtcNow;
        var rootTime = now.AddMilliseconds(-now.Millisecond).AddSeconds(initialOffset);
        var seconds  = (long)(rootTime - DateTime.UnixEpoch).TotalSeconds % WeatherPeriod;

        return rootTime.AddSeconds(-seconds);
    }

    private static string FormatForecastTime(DateTime forecastTime)
    {
        return I18N.FormatTime(forecastTime - DateTime.UtcNow);
    }
}
