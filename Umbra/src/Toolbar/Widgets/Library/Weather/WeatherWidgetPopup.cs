namespace Umbra.Widgets;

internal partial class WeatherWidgetPopup : WidgetPopup
{
    public uint               MaxEntries { get; set; } = 8;
    public I18N.TimeAgoFormat TimeFormat { get; set; } = I18N.TimeAgoFormat.Long;

    protected override Node Node { get; } = UmbraDrawing.DocumentFrom("umbra.widgets.popup_weather.xml").RootNode!;

    private readonly IZoneManager _zoneManager = Framework.Service<IZoneManager>();

    protected override bool CanOpen()
    {
        if (MaxEntries == 0) return false;

        return _zoneManager is { HasCurrentZone: true, CurrentZone: { WeatherForecast.Count: > 1, CurrentWeather: not null } };
    }

    protected override void OnUpdate()
    {
        Node header   = Node.QuerySelector(".header")!;
        Node bgTop    = Node.QuerySelector(".background > .bg-top")!;
        Node bgBottom = Node.QuerySelector(".background > .bg-bottom")!;

        List<WeatherForecast> forecast        = _zoneManager.CurrentZone.WeatherForecast;
        WeatherForecast?      currentForecast = _zoneManager.CurrentZone.CurrentWeather;
        
        header.QuerySelector(".icon")!.Style.IconId     = currentForecast!.IconId;
        header.QuerySelector(".text-top")!.NodeValue    = currentForecast.Name;
        header.QuerySelector(".text-bottom")!.NodeValue = _zoneManager.CurrentZone.Name;

        uint color = GetDominantColor(currentForecast.IconId);
        
        bgTop.Style.BackgroundGradient    = GradientColor.Vertical(new(0), new((uint)(0x9F << 24 | color)));
        bgBottom.Style.BackgroundGradient = GradientColor.Vertical(new((uint)(0xB0 << 24 | color)), new(0));

        for (var i = 1; i < 9; i++) {
            Node node = Node.QuerySelector($"#ForecastItem{i}")!;

            WeatherForecast? forecastItem = forecast.ElementAtOrDefault(i);

            node.Style.IsVisible = i <= MaxEntries && forecastItem is not null;
            if (forecastItem is null) continue;

            node.QuerySelector(".icon")!.Style.IconId             = forecastItem.IconId;
            node.QuerySelector(".text > .text-top")!.NodeValue    = forecastItem.Name;
            node.QuerySelector(".text > .text-bottom")!.NodeValue = I18N.FormatTimeAgo(forecastItem.TimeSpan, TimeFormat);
        }
    }
}
