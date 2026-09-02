namespace Umbra.Game;

public class WeatherForecast(DateTime time, string timeString, string name, uint iconId)
{
    public DateTime Time     { get; set; } = time;
    public string TimeString { get; set; } = timeString;
    public string Name       { get; set; } = name;
    public uint IconId       { get; set; } = iconId;

    public TimeSpan TimeSpan => Time - DateTime.UtcNow;

    public void Update(DateTime time, string timeString, string name, uint iconId)
    {
        Time = time;
        TimeString = timeString;
        Name = name;
        IconId = iconId;
    }
}
