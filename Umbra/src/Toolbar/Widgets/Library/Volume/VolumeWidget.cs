using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using System;
using System.Diagnostics;
using Umbra.Common;

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
        StandardWidgetFeatures.Icon;

    private readonly IGameConfig _gameConfig = Framework.Service<IGameConfig>();

    protected override void OnLoad()
    {
        SetFontAwesomeIcon(FontAwesomeIcon.VolumeUp);

        Node.OnRightClick += _ => ToggleMute();
    }

    protected override void OnDraw()
    {
        SetFontAwesomeIcon(GetVolumeIcon("SoundMaster", "IsSndMaster"));

        Popup.ShowOptions = GetConfigValue<bool>("ShowOptions");
        Popup.ShowBgm     = GetConfigValue<bool>("ShowBgm");
        Popup.ShowSfx     = GetConfigValue<bool>("ShowSfx");
        Popup.ShowVoc     = GetConfigValue<bool>("ShowVoc");
        Popup.ShowAmb     = GetConfigValue<bool>("ShowAmb");
        Popup.ShowSys     = GetConfigValue<bool>("ShowSys");
        Popup.ShowPerf    = GetConfigValue<bool>("ShowPerf");
        Popup.ValueStep   = GetConfigValue<int>("ValueStep");
        Popup.UpIcon      = GetConfigValue<FontAwesomeIcon>("UpIcon");
        Popup.DownIcon    = GetConfigValue<FontAwesomeIcon>("DownIcon");
        Popup.OffIcon     = GetConfigValue<FontAwesomeIcon>("OffIcon");
        Popup.MuteIcon    = GetConfigValue<FontAwesomeIcon>("MuteIcon");
    }

    private void ToggleMute()
    {
        string channelName = GetConfigValue<string>("RightClickBehavior") switch 
        {
            "Master" => "IsSndMaster",
            "BGM"    => "IsSndBgm",
            "SFX"    => "IsSndSe",
            "VOC"    => "IsSndVoice",
            "AMB"    => "IsSndEnv",
            "SYS"    => "IsSndSystem",
            "PERF"   => "IsSndPerform",
            _        => throw new InvalidOperationException("Invalid volume channel selected.")
        };
        
        _gameConfig.System.Set(channelName, !_gameConfig.System.GetBool(channelName));
    }

    private FontAwesomeIcon GetVolumeIcon(string volumeConfigName, string muteConfigName)
    {
        if (_gameConfig.System.GetBool(muteConfigName)) {
            return GetConfigValue<FontAwesomeIcon>("MuteIcon");
        }

        return _gameConfig.System.GetUInt(volumeConfigName) switch {
            0    => GetConfigValue<FontAwesomeIcon>("OffIcon"),
            < 50 => GetConfigValue<FontAwesomeIcon>("DownIcon"),
            _    => GetConfigValue<FontAwesomeIcon>("UpIcon")
        };
    }
}
