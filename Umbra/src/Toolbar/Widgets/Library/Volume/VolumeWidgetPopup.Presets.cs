using System.Text.Json;
using Umbra.Windows;
using Umbra.Windows.Dialogs;

namespace Umbra.Widgets;

internal sealed partial class VolumeWidgetPopup
{
    private Dictionary<string, VolumePreset> _presets = [];

    private ToolbarWidget? _parentWidget;
    private string         _currentPresetId = string.Empty;

    public override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable("StoredPresets", "", null, "{}", 4096) { IsHidden = true },
            new StringWidgetConfigVariable("CurrentPresetId", "", null, "") { IsHidden       = true },
        ];
    }

    protected override void UpdateConfigVariables(ToolbarWidget widget)
    {
        if (_parentWidget == null) {
            _parentWidget = widget;

            string presetDataStr = widget.GetConfigValue<string>("StoredPresets");

            try {
                _presets = JsonSerializer.Deserialize<Dictionary<string, VolumePreset>>(presetDataStr) ?? [];
                if (_presets.Count == 0) {
                    CreateDefaultPresets();
                }
            } catch {
                CreateDefaultPresets();
            }

            string id = widget.GetConfigValue<string>("CurrentPresetId");

            if (string.Empty != id) {
                SetActivePreset(id, false);
            }
        }
    }

    private void ForcePresetSync()
    {
        if (string.IsNullOrEmpty(_currentPresetId)) return;
        if (!_presets.TryGetValue(_currentPresetId, out var currentPreset)) return;

        var shouldFlush = false;

        foreach (var channel in _channels) {
            shouldFlush |= EnsurePresetSync(currentPreset.Mutes, channel.MuteConfigName, _gameConfig.System.GetBool);
            shouldFlush |= EnsurePresetSync(currentPreset.Bgs, channel.BgConfigName, _gameConfig.System.GetBool);
            shouldFlush |= EnsurePresetSync(currentPreset.Volumes, channel.VolumeConfigName, name => (int)_gameConfig.System.GetUInt(name));
        }
        
        foreach (var config in currentPreset.Configs) {
            shouldFlush |= EnsurePresetSync(currentPreset.Configs, config.Key, _gameConfig.System.GetBool);
        }

        if (shouldFlush) FlushPresets();
    }

    private static bool EnsurePresetSync<T>(IDictionary<string, T> dict, string key, Func<string, T> getValue)
    {
        var value = getValue(key);

        if (!dict.TryGetValue(key, out var current)) {
            dict[key] = value;
            return true;
        }

        if (EqualityComparer<T>.Default.Equals(current, value)) return false;

        dict[key] = value;
        return true;
    }

    private void SetActivePreset(string id, bool persist = true)
    {
        if (id != string.Empty && id == _currentPresetId) {
            id = "";
        }

        foreach (var presetButton in Node.QuerySelectorAll(".preset")) {
            presetButton.ToggleClass("inactive", presetButton.Id != id);
        }

        _currentPresetId = id;

        if (persist) {
            _parentWidget?.SetConfigValue("CurrentPresetId", id);
        }

        if (id == "") {
            Node.QuerySelector(".preset-info")!.Style.IsVisible = false;
            return;
        }

        if (_presets.TryGetValue(id, out var preset)) {
            Node.QuerySelector(".preset-info")!.Style.IsVisible = true;
            Node.QuerySelector(".preset-name")!.NodeValue       = preset.Name;

            foreach (var channel in _channels) {
                if (preset.Volumes.TryGetValue(channel.VolumeConfigName, out var vol)) {
                    _gameConfig.System.Set(channel.VolumeConfigName, (uint)vol);
                    channel.Node.QuerySelector<VerticalSliderNode>(".slider")!.Value = vol;
                }

                if (preset.Mutes.TryGetValue(channel.MuteConfigName, out var mute)) {
                    _gameConfig.System.Set(channel.MuteConfigName, mute);
                }

                if (preset.Bgs.TryGetValue(channel.BgConfigName, out var bg)) {
                    _gameConfig.System.Set(channel.BgConfigName, bg);
                }
            }

            foreach (var (configName, value) in preset.Configs) {
                _gameConfig.System.Set(configName, value);
            }
        } else {
            Node.QuerySelector(".preset-info")!.Style.IsVisible = false;
        }
    }

    private void ShowRenamePresetPopup(Node node)
    {
        if (_currentPresetId == string.Empty) {
            return;
        }

        if (!_presets.TryGetValue(_currentPresetId, out var preset)) {
            return;
        }

        Framework.Service<WindowManager>().Present("RenameVolumePreset", new PromptWindow(
            I18N.Translate("Widget.RenamePreset.Title"),
            I18N.Translate("Widget.RenamePreset.Description"),
            I18N.Translate("Confirm"),
            I18N.Translate("Cancel"),
            "",
            preset.Name
        ), (prompt) => {
            if (prompt.Confirmed) {
                Logger.Info($"Changed name from '{preset.Name}' to '{prompt.Value}'");
                preset.Name = prompt.Value;
                FlushPresets();
                Node.QuerySelector<Node>(".preset-name")!.NodeValue = preset.Name;
            }
        });
    }

    private void CreateDefaultPresets()
    {
        _presets = [];

        int num = 0;
        foreach (var pb in Node.QuerySelectorAll(".preset")) {
            num++;
            _presets[pb.Id!] = new VolumePreset { Name = $"Preset #{num}" };
        }
    }

    private void SetPresetVolume(string configName, int value)
    {
        if (string.Empty == _currentPresetId) return;

        if (_presets.TryGetValue(_currentPresetId, out var preset)) {
            if (preset.Volumes.ContainsKey(configName)) {
                preset.Volumes[configName] = value;
                FlushPresets();
            } else {
                Logger.Warning($"Volume config name '{configName}' not found in preset.");
            }
        }
    }

    private void SetPresetMute(string configName, bool value)
    {
        if (string.Empty == _currentPresetId) return;

        if (_presets.TryGetValue(_currentPresetId, out var preset)) {
            if (preset.Mutes.ContainsKey(configName)) {
                preset.Mutes[configName] = value;
                FlushPresets();
            } else {
                Logger.Warning($"Mute config name '{configName}' not found in preset.");
            }
        }
    }

    private void SetPresetBg(string configName, bool value)
    {
        if (string.Empty == _currentPresetId) return;

        if (_presets.TryGetValue(_currentPresetId, out var preset)) {
            if (preset.Bgs.ContainsKey(configName)) {
                preset.Bgs[configName] = value;
                FlushPresets();
            } else {
                Logger.Warning($"Bg config name '{configName}' not found in preset.");
            }
        }
    }

    private void SetPresetConfig(string configName, bool value)
    {
        if (string.Empty == _currentPresetId) return;

        if (_presets.TryGetValue(_currentPresetId, out var preset)) {
            if (preset.Configs.ContainsKey(configName)) {
                preset.Configs[configName] = value;
                FlushPresets();
            } else {
                Logger.Warning($"Config name '{configName}' not found in preset.");
            }
        }
    }

    private void FlushPresets()
    {
        if (null == _parentWidget) {
            Logger.Warning("Attempt to flush presets but parent widget is null.");
            return;
        }

        _parentWidget?.SetConfigValue("StoredPresets", JsonSerializer.Serialize(_presets));
    }

    [Serializable]
    private class VolumePreset
    {
        public string Name { get; set; } = string.Empty;

        public Dictionary<string, int> Volumes { get; set; } = new() {
            { "SoundMaster", 100 },
            { "SoundBgm", 100 },
            { "SoundSe", 100 },
            { "SoundVoice", 100 },
            { "SoundEnv", 100 },
            { "SoundSystem", 100 },
            { "SoundPerform", 100 },
        };

        public Dictionary<string, bool> Mutes { get; set; } = new() {
            { "IsSndMaster", false },
            { "IsSndBgm", false },
            { "IsSndSe", false },
            { "IsSndVoice", false },
            { "IsSndEnv", false },
            { "IsSndSystem", false },
            { "IsSndPerform", false },
        };

        public Dictionary<string, bool> Bgs { get; set; } = new() {
            { "IsSoundAlways", false },
            { "IsSoundBgmAlways", false },
            { "IsSoundSeAlways", false },
            { "IsSoundVoiceAlways", false },
            { "IsSoundEnvAlways", false },
            { "IsSoundSystemAlways", false },
            { "IsSoundPerformAlways", false },
        };

        public Dictionary<string, bool> Configs { get; set; } = new() {
            { "SoundChocobo", false },
            { "SoundFieldBattle", false },
            { "SoundHousing", false },
        };
    }
}
