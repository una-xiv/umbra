namespace Umbra.Game;

public class WeatherForecast(DateTime time, string timeString, string name, uint iconId)
{
    public DateTime Time       = time;
    public TimeSpan TimeSpan   = time - DateTime.UtcNow;
    public string   TimeString = timeString;
    public string   Name       = name;
    public uint     IconId     = iconId;
}
