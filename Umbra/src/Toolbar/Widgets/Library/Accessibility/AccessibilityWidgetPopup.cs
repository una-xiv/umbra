using Dalamud.Plugin.Services;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class AccessibilityWidgetPopup : WidgetPopup
{
    protected sealed override Node Node { get; } = new() {
        Stylesheet = AccessibilityStylesheet,
        ClassList  = ["popup"],
        ChildNodes = [
            new() {
                ClassList = ["left-side"],
                ChildNodes = [
                    new() {
                        ClassList = ["slider"],
                        ChildNodes = [
                            new VerticalSliderNode() {
                                Id       = "Va",
                                MinValue = 0,
                                MaxValue = 100,
                                Style = new() {
                                    Size = new(30, 80),
                                }
                            },
                            new() {
                                ClassList = ["slider-label"],
                                NodeValue = "VAS",
                                Tooltip   = I18N.Translate("Widget.Accessibility.Config.VisualAlertsSize")
                            }
                        ]
                    },
                    new() {
                        ClassList = ["slider"],
                        ChildNodes = [
                            new VerticalSliderNode() {
                                Id       = "Vt",
                                MinValue = 0,
                                MaxValue = 100,
                                Style = new() {
                                    Size = new(30, 80),
                                }
                            },
                            new() {
                                ClassList = ["slider-label"],
                                NodeValue = "VAT",
                                Tooltip   = I18N.Translate("Widget.Accessibility.Config.VisualAlertsTransparency")
                            }
                        ]
                    },
                    new() {
                        ClassList = ["slider"],
                        ChildNodes = [
                            new VerticalSliderNode() {
                                Id       = "Cf",
                                MinValue = 0,
                                MaxValue = 100,
                                Style = new() {
                                    Size = new(30, 80),
                                }
                            },
                            new() {
                                ClassList = ["slider-label"],
                                NodeValue = "CF",
                                Tooltip   = I18N.Translate("Widget.Accessibility.Config.ColorFilterRange")
                            }
                        ]
                    }
                ]
            },
            new() {
                ClassList = ["right-side"],
                ChildNodes = [
                    new CheckboxNode("EnableAudioVis",       false, I18N.Translate("Widget.Accessibility.Config.VisualAlerts")),
                    new CheckboxNode("EnableColorBlindMode", false, I18N.Translate("Widget.Accessibility.Config.ColorFilter")),
                    new() {
                        Style = new() {
                            Size = new(200, 0)
                        },
                        ChildNodes = [
                            new SelectNode(
                                "ColorBlindMode",
                                "None",
                                ["None", "Protanopia", "Deuteranopia", "Tritanopia"],
                                ""
                            )
                        ]
                    }
                ]
            }
        ]
    };

    private readonly IGameConfig _gameConfig;

    public AccessibilityWidgetPopup()
    {
        _gameConfig = Framework.Service<IGameConfig>();

        var visualAlertsToggle   = Node.QuerySelector<CheckboxNode>("EnableAudioVis")!;
        var colorBlindModeToggle = Node.QuerySelector<CheckboxNode>("EnableColorBlindMode")!;
        var colorBlindModeSelect = Node.QuerySelector<SelectNode>("ColorBlindMode")!;
        var visualAlertsSize     = Node.QuerySelector<VerticalSliderNode>("Va")!;
        var visualAlertsTransp   = Node.QuerySelector<VerticalSliderNode>("Vt")!;
        var colorFilterRange     = Node.QuerySelector<VerticalSliderNode>("Cf")!;

        visualAlertsToggle.OnValueChanged += v => _gameConfig.System.Set("AccessibilitySoundVisualEnable", v);
        colorBlindModeToggle.OnValueChanged += v => _gameConfig.System.Set("AccessibilityColorBlindFilterEnable", v);
        visualAlertsSize.OnValueChanged += v => _gameConfig.System.Set("AccessibilitySoundVisualDispSize", (uint)v);
        visualAlertsTransp.OnValueChanged += v => _gameConfig.System.Set("AccessibilitySoundVisualPermeabilityRate", (uint)v);
        colorFilterRange.OnValueChanged += v => _gameConfig.System.Set("AccessibilityColorBlindFilterStrength", (uint)v);

        colorBlindModeSelect.OnValueChanged += v => {
            _gameConfig.System.Set(
                "AccessibilityColorBlindFilterType",
                v switch {
                    "None"          => 0,
                    "Protanopia"    => 1,
                    "Deuteranopia"  => 2,
                    "Tritanopia"    => 3,
                    _               => 0
                }
            );
        };
    }

    protected override void OnOpen()
    {
        var visualAlertsToggle   = Node.QuerySelector<CheckboxNode>("EnableAudioVis")!;
        var colorBlindModeToggle = Node.QuerySelector<CheckboxNode>("EnableColorBlindMode")!;
        var colorBlindModeSelect = Node.QuerySelector<SelectNode>("ColorBlindMode")!;
        var visualAlertsSize     = Node.QuerySelector<VerticalSliderNode>("Va")!;
        var visualAlertsTransp   = Node.QuerySelector<VerticalSliderNode>("Vt")!;
        var colorFilterRange     = Node.QuerySelector<VerticalSliderNode>("Cf")!;

        visualAlertsSize.Value     = (int)_gameConfig.System.GetUInt("AccessibilitySoundVisualDispSize");
        visualAlertsTransp.Value   = (int)_gameConfig.System.GetUInt("AccessibilitySoundVisualPermeabilityRate");
        colorFilterRange.Value     = (int)_gameConfig.System.GetUInt("AccessibilityColorBlindFilterStrength");
        visualAlertsToggle.Value   = _gameConfig.System.GetBool("AccessibilitySoundVisualEnable");
        colorBlindModeToggle.Value = _gameConfig.System.GetBool("AccessibilityColorBlindFilterEnable");
        colorBlindModeSelect.Value = _gameConfig.System.GetUInt("AccessibilityColorBlindFilterType") switch {
            0 => "None",
            1 => "Protanopia",
            2 => "Deuteranopia",
            3 => "Tritanopia",
            _ => "None"
        };
    }
}
