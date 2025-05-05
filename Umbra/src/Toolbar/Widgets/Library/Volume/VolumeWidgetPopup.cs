using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed class VolumeWidgetPopup : WidgetPopup
{
    public bool ShowOptions { get; set; }
    public bool ShowBgm     { get; set; } = true;
    public bool ShowSfx     { get; set; } = true;
    public bool ShowVoc     { get; set; } = true;
    public bool ShowAmb     { get; set; } = true;
    public bool ShowSys     { get; set; } = true;
    public bool ShowPerf    { get; set; } = true;
    public int  ValueStep   { get; set; } = 1;

    public FontAwesomeIcon UpIcon   { get; set; } = FontAwesomeIcon.VolumeUp;
    public FontAwesomeIcon DownIcon { get; set; } = FontAwesomeIcon.VolumeDown;
    public FontAwesomeIcon OffIcon  { get; set; } = FontAwesomeIcon.VolumeOff;
    public FontAwesomeIcon MuteIcon { get; set; } = FontAwesomeIcon.VolumeMute;

    private static readonly UdtDocument Document = UmbraDrawing.DocumentFrom("umbra.widgets.popup_volume.xml");

    protected override Node Node { get; } = Document.RootNode!;

    private readonly List<AudioChannel> _channels   = [];
    private readonly IGameConfig        _gameConfig = Framework.Service<IGameConfig>();
 
    private bool _isBound;
    
    protected override void OnUpdate()
    {
        foreach (var channel in _channels) {
            channel.Node.Style.IsVisible = channel.Name switch {
                "BGM"  => ShowBgm,
                "SFX"  => ShowSfx,
                "VOC"  => ShowVoc,
                "AMB"  => ShowAmb,
                "SYS"  => ShowSys,
                "PERF" => ShowPerf,
                _      => channel.Node.Style.IsVisible
            };

            channel.Node.QuerySelector<VerticalSliderNode>(".slider")!.Step = ValueStep;

            channel.Node.QuerySelector(".value")!.NodeValue =
                $"{_gameConfig.System.GetUInt(channel.VolumeConfigName)}%";

            channel.Node.QuerySelector(".mute-button")!.NodeValue =
                GetVolumeIcon(channel.VolumeConfigName, channel.MuteConfigName).ToIconString();

            Node bgBtn = channel.Node.QuerySelector(".bg-button")!;
            bool isBg  = _gameConfig.System.GetBool(channel.BgConfigName);

            bgBtn.NodeValue = isBg ? FontAwesomeIcon.PlusCircle.ToIconString() : FontAwesomeIcon.Minus.ToIconString();
            bgBtn.Tooltip   = I18N.Translate(isBg ? "Widget.Volume.Tooltip.Bg.On" : "Widget.Volume.Tooltip.Bg.Off");
        }

        Node.QuerySelector(".separator")!.Style.IsVisible    = HasVisibleChannels;
        Node.QuerySelector(".options-list")!.Style.IsVisible = ShowOptions;
    }

    protected override void OnOpen()
    {
        if (!_isBound) {
            _isBound = true;
            BindChannelWidget("Master", "SoundMaster", "IsSndMaster", "IsSoundAlways");
            BindChannelWidget("BGM", "SoundBgm", "IsSndBgm", "IsSoundBgmAlways");
            BindChannelWidget("SFX", "SoundSe", "IsSndSe", "IsSoundSeAlways");
            BindChannelWidget("VOC", "SoundVoice", "IsSndVoice", "IsSoundVoiceAlways");
            BindChannelWidget("AMB", "SoundEnv", "IsSndEnv", "IsSoundEnvAlways");
            BindChannelWidget("SYS", "SoundSystem", "IsSndSystem", "IsSoundSystemAlways");
            BindChannelWidget("PERF", "SoundPerform", "IsSndPerform", "IsSoundPerformAlways");

            Node optionsList = Node.QuerySelector(".options-list")!;
            optionsList.AppendChild(CreateToggleOption("SoundChocobo"));
            optionsList.AppendChild(CreateToggleOption("SoundFieldBattle"));
            optionsList.AppendChild(CreateToggleOption("SoundHousing"));
        }
        
        foreach (var channel in _channels) {
            channel.Node.QuerySelector<VerticalSliderNode>(".slider")!.SetValue(
                (int)_gameConfig.System.GetUInt(channel.VolumeConfigName)
            );
        }
    }

    protected override void OnClose() { }

    private bool HasVisibleChannels => ShowBgm || ShowSfx || ShowVoc || ShowAmb || ShowSys || ShowPerf;

    private void BindChannelWidget(string id, string volumeConfigName, string muteConfigName, string bgConfigName)
    {
        Node node = Node.QuerySelector($"#{id}")!;

        Node muteButton = node.QuerySelector(".mute-button")!;
        Node bgButton   = node.QuerySelector(".bg-button")!;

        muteButton.OnClick += _ => {
            _gameConfig.System.Set(muteConfigName, !_gameConfig.System.GetBool(muteConfigName));
        };
        muteButton.OnRightClick += _ => {
            _gameConfig.System.Set(muteConfigName, !_gameConfig.System.GetBool(muteConfigName));
        };
        bgButton.OnClick += _ => {
            _gameConfig.System.Set(bgConfigName, !_gameConfig.System.GetBool(bgConfigName));
        };
        bgButton.OnRightClick += _ => {
            _gameConfig.System.Set(bgConfigName, !_gameConfig.System.GetBool(bgConfigName));
        };

        node.QuerySelector<VerticalSliderNode>(".slider")!.OnValueChanged += value => {
            _gameConfig.System.Set(volumeConfigName, (uint)value);

            if (_gameConfig.System.GetBool(muteConfigName)) {
                _gameConfig.System.Set(muteConfigName, false);
            }
        };

        _channels.Add(
            new() {
                Name             = id,
                VolumeConfigName = volumeConfigName,
                MuteConfigName   = muteConfigName,
                BgConfigName     = bgConfigName,
                Node             = node
            }
        );
    }

    private CheckboxNode CreateToggleOption(string configName)
    {
        CheckboxNode node = new(
            configName,
            _gameConfig.System.GetBool(configName),
            I18N.Translate($"Widget.Volume.Option.{configName}"),
            I18N.Has($"Widget.Volume.Option.{configName}.Description")
                ? I18N.Translate($"Widget.Volume.Option.{configName}.Description")
                : null
        );

        node.OnValueChanged += v => { _gameConfig.System.Set(configName, v); };

        return node;
    }

    private FontAwesomeIcon GetVolumeIcon(string volumeConfigName, string muteConfigName)
    {
        if (_gameConfig.System.GetBool(muteConfigName)) {
            return MuteIcon;
        }

        return _gameConfig.System.GetUInt(volumeConfigName) switch {
            0    => OffIcon,
            < 50 => DownIcon,
            _    => UpIcon
        };
    }

    private readonly struct AudioChannel
    {
        public string Name             { get; init; }
        public string VolumeConfigName { get; init; }
        public string MuteConfigName   { get; init; }
        public string BgConfigName     { get; init; }
        public Node   Node             { get; init; }
    }
}
