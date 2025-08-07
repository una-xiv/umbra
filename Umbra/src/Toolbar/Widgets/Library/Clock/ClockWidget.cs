using Dalamud.Game.Text;
using Dalamud.Utility;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Clock",
    "Widget.Clock.Name",
    "Widget.Clock.Description",
    ["clock", "time"]
)]
internal partial class ClockWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.None;

    private readonly Node _clockNode = UmbraDrawing.DocumentFrom("umbra.widgets.clock.xml").RootNode!;

    private Node  PrefixNode => Node.QuerySelector("#prefix")!;
    private Node  TimeNode   => Node.QuerySelector("#time")!;
    private bool? _isInteractive;

    public override string GetInstanceName()
        => $"{Info.Name} - {I18N.Translate($"Widget.Clock.Config.TimeSource.{GetConfigValue<string>("TimeSource")}")}";

    protected override void OnLoad()
    {
        BodyNode.AppendChild(_clockNode);
    }

    protected override void OnDraw()
    {
        var useCustomPrefix = GetConfigValue<bool>("UseCustomPrefix");
        var isInteractive   = GetConfigValue<bool>("ClickToSwitch");

        bool isPrefixVisible =
            !useCustomPrefix || (useCustomPrefix && !GetConfigValue<string>("PrefixText").Trim().IsNullOrEmpty());

        PrefixNode.ToggleClass("native", useCustomPrefix);
        Node.ToggleClass("decorated", GetConfigValue<bool>("Decorate"));

        if (_isInteractive != isInteractive) {
            _isInteractive = isInteractive;

            if (isInteractive) {
                Node.OnClick += OnClick;
            } else {
                Node.OnClick -= OnClick;
            }
        }

        Node.Tooltip = isInteractive
            ? I18N.Translate($"Widget.Clock.Config.TimeSource.{GetConfigValue<string>("TimeSource")}")
            : null;

        DateTime time        = GetTime();
        var      use24H      = GetConfigValue<bool>("Use24HourFormat");
        var      showSeconds = GetConfigValue<bool>("ShowSeconds");
        var      am          = GetConfigValue<string>("AmLabel");
        var      pm          = GetConfigValue<string>("PmLabel");
        string   timeFormat  = (use24H ? "HH:mm" : "hh:mm") + (showSeconds ? ":ss" : string.Empty);
        string   suffix      = use24H ? string.Empty : (time.Hour >= 12 ? $" {pm}" : $" {am}");

        PrefixNode.NodeValue = useCustomPrefix ? GetConfigValue<string>("PrefixText") : GetPrefixIcon().ToIconString();
        TimeNode.NodeValue   = $"{time.ToString(timeFormat)}{suffix}";

        PrefixNode.Style.IsVisible  = isPrefixVisible;
        PrefixNode.Style.Font       = useCustomPrefix ? 3u : 4u;
        PrefixNode.Style.TextOffset = new(0, GetConfigValue<int>("PrefixYOffset"));
        TimeNode.Style.TextOffset   = new(0, GetConfigValue<int>("TextYOffset"));
        TimeNode.Style.Size         = new(GetConfigValue<int>("CustomWidth"), 0);
        TimeNode.Style.FontSize     = SafeHeight / 2;
    }

    private void OnClick(Node _)
    {
        var timeSource = GetConfigValue<string>("TimeSource");

        string nextTimeSource = timeSource switch {
            "ET" => "LT",
            "LT" => "ST",
            "ST" => "ET",
            _    => throw new ArgumentOutOfRangeException()
        };

        SetConfigValue("TimeSource", nextTimeSource);
    }

    private SeIconChar GetPrefixIcon()
    {
        var timeSource = GetConfigValue<string>("TimeSource");

        return I18N.GetCurrentLanguage() switch {
            "de" => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeDe,
                "LT" => SeIconChar.LocalTimeDe,
                "ST" => SeIconChar.ServerTimeDe,
                _    => throw new ArgumentOutOfRangeException()
            },
            "fr" => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeFr,
                "LT" => SeIconChar.LocalTimeFr,
                "ST" => SeIconChar.ServerTimeFr,
                _    => throw new ArgumentOutOfRangeException()
            },
            "jp" => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeJa,
                "LT" => SeIconChar.LocalTimeJa,
                "ST" => SeIconChar.ServerTimeJa,
                _    => throw new ArgumentOutOfRangeException()
            },
            _ => timeSource switch {
                "ET" => SeIconChar.EorzeaTimeEn,
                "LT" => SeIconChar.LocalTimeEn,
                "ST" => SeIconChar.ServerTimeEn,
                _    => throw new ArgumentOutOfRangeException()
            }
        };
    }
}
