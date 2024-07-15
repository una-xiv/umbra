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

    public bool ShowOptions { get; set; } = false;
    public bool ShowBgm     { get; set; } = true;
    public bool ShowSfx     { get; set; } = true;
    public bool ShowVoc     { get; set; } = true;
    public bool ShowAmb     { get; set; } = true;
    public bool ShowSys     { get; set; } = true;
    public bool ShowPerf    { get; set; } = true;
    public int  ValueStep   { get; set; } = 1;

    private readonly List<AudioChannel> _channels = [];
    private readonly IGameConfig        _gameConfig;

    public VolumeWidgetPopup()
    {
        _gameConfig = Framework.Service<IGameConfig>();

        CreateChannelWidget("Master", "SoundMaster", "IsSndMaster");
        CreateSeparator();
        CreateChannelWidget("BGM",  "SoundBgm",     "IsSndBgm");
        CreateChannelWidget("SFX",  "SoundSe",      "IsSndSe");
        CreateChannelWidget("VOC",  "SoundVoice",   "IsSndVoice");
        CreateChannelWidget("AMB",  "SoundEnv",     "IsSndEnv");
        CreateChannelWidget("SYS",  "SoundSystem",  "IsSndSystem");
        CreateChannelWidget("PERF", "SoundPerform", "IsSndPerform");

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

    private void CreateChannelWidget(string id, string volumeConfigName, string muteConfigName)
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
                    ClassList = ["channel--mute"],
                    ChildNodes = [
                        new() {
                            ClassList = ["channel--mute-button"],
                            NodeValue = FontAwesomeIcon.VolumeMute.ToIconString()
                        }
                    ]
                }
            }
        };

        node.QuerySelector(".channel--mute-button")!.OnClick += _ => {
            bool isMuted = _gameConfig.System.GetBool(muteConfigName);
            _gameConfig.System.Set(muteConfigName, !isMuted);
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
            return FontAwesomeIcon.VolumeMute;
        }

        return _gameConfig.System.GetUInt(volumeConfigName) switch {
            0    => FontAwesomeIcon.VolumeOff,
            < 50 => FontAwesomeIcon.VolumeDown,
            _    => FontAwesomeIcon.VolumeUp
        };
    }

    private readonly struct AudioChannel
    {
        public string Name             { get; init; }
        public string VolumeConfigName { get; init; }
        public string MuteConfigName   { get; init; }
        public Node   Node             { get; init; }
    }
}
