using System.Diagnostics;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Volume",
    "Widget.Volume.Name",
    "Widget.Volume.Description",
    ["volume", "audio", "channels", "sound", "sfx", "bgm"]
)]
internal sealed partial class VolumeWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override VolumeWidgetPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text;

    protected override string DefaultSizingMode => SizingModeFit;
    protected override int    DefaultWidth      => 32;

    private readonly IGameConfig _gameConfig = Framework.Service<IGameConfig>();

    protected override void OnLoad()
    {
        SetFontAwesomeIcon(FontAwesomeIcon.VolumeUp);
        SetText(I18N.Translate("Widget.Volume.Label"));

        Node.OnRightClick += _ => ToggleMute();
    }

    protected override void OnDraw()
    {
        // Set icon size to 32x32 regardless of widget size. This
        //   prevents the widget from resizing when the icon changes.
        Node iconNode = Node.QuerySelector(".icon")!;
        iconNode.Style.Size = new(32, 32);

        SetFontAwesomeIcon(GetVolumeIcon());

        Popup.ShowOptions = GetConfigValue<bool>("ShowOptions");
        Popup.ShowBgm     = GetConfigValue<bool>("ShowBgm");
        Popup.ShowSfx     = GetConfigValue<bool>("ShowSfx");
        Popup.ShowVoc     = GetConfigValue<bool>("ShowVoc");
        Popup.ShowAmb     = GetConfigValue<bool>("ShowAmb");
        Popup.ShowSys     = GetConfigValue<bool>("ShowSys");
        Popup.ShowPerf    = GetConfigValue<bool>("ShowPerf");
        Popup.ShowPresets = GetConfigValue<bool>("ShowPresets");
        Popup.ValueStep   = GetConfigValue<int>("ValueStep");
        Popup.UpIcon      = GetConfigValue<FontAwesomeIcon>("UpIcon");
        Popup.DownIcon    = GetConfigValue<FontAwesomeIcon>("DownIcon");
        Popup.OffIcon     = GetConfigValue<FontAwesomeIcon>("OffIcon");
        Popup.MuteIcon    = GetConfigValue<FontAwesomeIcon>("MuteIcon");
    }

    private void ToggleMute()
    {
        string channelName = GetMuteConfigName();

        _gameConfig.System.Set(channelName, !_gameConfig.System.GetBool(channelName));
    }

    private FontAwesomeIcon GetVolumeIcon()
    {
        if (_gameConfig.System.GetBool(GetMuteConfigName())) {
            return GetConfigValue<FontAwesomeIcon>("MuteIcon");
        }

        return _gameConfig.System.GetUInt(GetVolumeConfigName()) switch {
            0    => GetConfigValue<FontAwesomeIcon>("OffIcon"),
            < 50 => GetConfigValue<FontAwesomeIcon>("DownIcon"),
            _    => GetConfigValue<FontAwesomeIcon>("UpIcon")
        };
    }

    private string GetMuteConfigName()
    {
        return GetConfigValue<string>("RightClickBehavior") switch {
            "Master" => "IsSndMaster",
            "BGM"    => "IsSndBgm",
            "SFX"    => "IsSndSe",
            "VOC"    => "IsSndVoice",
            "AMB"    => "IsSndEnv",
            "SYS"    => "IsSndSystem",
            "PERF"   => "IsSndPerform",
            _        => throw new InvalidOperationException("Invalid volume channel selected.")
        };
    }

    private string GetVolumeConfigName()
    {
        return GetConfigValue<string>("RightClickBehavior") switch {
            "Master" => "SoundMaster",
            "BGM"    => "SoundBgm",
            "SFX"    => "SoundSe",
            "VOC"    => "SoundVoice",
            "AMB"    => "SoundEnv",
            "SYS"    => "SoundSystem",
            "PERF"   => "SoundPerform",
            _        => throw new InvalidOperationException("Invalid volume channel selected.")
        };
    }
}
