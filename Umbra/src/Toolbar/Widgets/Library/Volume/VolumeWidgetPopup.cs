using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class VolumeWidgetPopup : WidgetPopup
{
    protected override Node Node { get; } = new() {
        Stylesheet = VolumeWidgetPopupStylesheet,
        ClassList  = ["popup"],
        ChildNodes = [
            new() { ClassList = ["channel-list"] },
            new() { ClassList = ["options-list"] },
        ]
    };

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

    private readonly List<AudioChannel> _channels = [];
    private readonly IGameConfig        _gameConfig;

    public VolumeWidgetPopup()
    {
        _gameConfig = Framework.Service<IGameConfig>();

        CreateChannelWidget("Master", "SoundMaster", "IsSndMaster", "IsSoundAlways");
        CreateSeparator();
        CreateChannelWidget("BGM",  "SoundBgm",     "IsSndBgm", "IsSoundBgmAlways");
        CreateChannelWidget("SFX",  "SoundSe",      "IsSndSe", "IsSoundSeAlways");
        CreateChannelWidget("VOC",  "SoundVoice",   "IsSndVoice", "IsSoundVoiceAlways");
        CreateChannelWidget("AMB",  "SoundEnv",     "IsSndEnv", "IsSoundEnvAlways");
        CreateChannelWidget("SYS",  "SoundSystem",  "IsSndSystem", "IsSoundSystemAlways");
        CreateChannelWidget("PERF", "SoundPerform", "IsSndPerform", "IsSoundPerformAlways");

        Node optionsList = Node.QuerySelector(".options-list")!;
        optionsList.AppendChild(CreateToggleOption("SoundChocobo"));
        optionsList.AppendChild(CreateToggleOption("SoundFieldBattle"));
        optionsList.AppendChild(CreateToggleOption("SoundHousing"));
    }

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

            channel.Node.QuerySelector<VerticalSliderNode>(".channel--slider")!.Step = ValueStep;

            channel.Node.QuerySelector(".channel--value")!.NodeValue =
                $"{_gameConfig.System.GetUInt(channel.VolumeConfigName)}%";

            channel.Node.QuerySelector(".channel--mute-button")!.NodeValue =
                GetVolumeIcon(channel.VolumeConfigName, channel.MuteConfigName).ToIconString();

            Node bgBtn = channel.Node.QuerySelector(".channel--bg-button")!;
            bool isBg  = _gameConfig.System.GetBool(channel.BgConfigName);

            bgBtn.NodeValue = isBg ? FontAwesomeIcon.PlusCircle.ToIconString() : FontAwesomeIcon.Minus.ToIconString();

            bgBtn.Tooltip = isBg
                ? I18N.Translate("Widget.Volume.Tooltip.Bg.On")
                : I18N.Translate("Widget.Volume.Tooltip.Bg.Off");
        }

        Node.QuerySelector(".separator")!.Style.IsVisible    = HasVisibleChannels;
        Node.QuerySelector(".options-list")!.Style.IsVisible = ShowOptions;
    }

    protected override void OnOpen()
    {
        foreach (var channel in _channels) {
            channel.Node.QuerySelector<VerticalSliderNode>(".channel--slider")!.SetValue(
                (int)_gameConfig.System.GetUInt(channel.VolumeConfigName)
            );
        }
    }

    protected override void OnClose() { }

    private bool HasVisibleChannels => ShowBgm || ShowSfx || ShowVoc || ShowAmb || ShowSys || ShowPerf;

    private void CreateChannelWidget(string id, string volumeConfigName, string muteConfigName, string bgConfigName)
    {
        Node node = new() {
            ClassList = ["channel"],
            ChildNodes = {
                new() {
                    ClassList = ["channel--name"],
                    NodeValue = I18N.Translate($"Widget.Volume.Channel.{id}")
                },
                new() {
                    ClassList = ["channel--value"],
                    NodeValue = "100%"
                },
                new VerticalSliderNode {
                    Id        = id,
                    ClassList = ["channel--slider"],
                    MinValue  = 0,
                    MaxValue  = 100,
                    Step      = ValueStep,
                },
                new() {
                    ClassList = ["channel--buttons"],
                    ChildNodes = [
                        new() {
                            ClassList = ["channel--mute-button", "channel--ctrl-button"],
                            NodeValue = FontAwesomeIcon.VolumeMute.ToIconString()
                        },
                        new() {
                            ClassList = ["channel--bg-button", "channel--ctrl-button"],
                            NodeValue = FontAwesomeIcon.PlusCircle.ToIconString(),
                            Tooltip   = I18N.Translate("Widget.Volume.Tooltip.Bg.On"),
                        },
                    ]
                }
            }
        };

        Node muteButton = node.QuerySelector(".channel--mute-button")!;
        Node bgButton   = node.QuerySelector(".channel--bg-button")!;

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

        node.QuerySelector<VerticalSliderNode>(".channel--slider")!.OnValueChanged += value => {
            _gameConfig.System.Set(volumeConfigName, (uint)value);

            if (_gameConfig.System.GetBool(muteConfigName)) {
                _gameConfig.System.Set(muteConfigName, false);
            }
        };

        Node.QuerySelector(".channel-list")!.AppendChild(node);

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

    private void CreateSeparator()
    {
        Node node = new() { ClassList = ["separator"] };

        node.BeforeReflow += _ => {
            int height = node.ParentNode!.InnerHeight;

            node.Bounds.ContentSize = new(1, height);
            node.Bounds.PaddingSize = new(1, height);
            node.Bounds.MarginSize  = new(1, height);

            return true;
        };

        Node.QuerySelector(".channel-list")!.AppendChild(node);
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
