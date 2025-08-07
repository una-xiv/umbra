namespace Umbra.Widgets;

internal class AccessibilityWidgetPopup : WidgetPopup
{
    protected sealed override Node Node { get; }
        = UmbraDrawing.DocumentFrom("umbra.widgets.popup_accessibility.xml").RootNode!;

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

        visualAlertsToggle.OnValueChanged   += v => _gameConfig.System.Set("AccessibilitySoundVisualEnable", v);
        colorBlindModeToggle.OnValueChanged += v => _gameConfig.System.Set("AccessibilityColorBlindFilterEnable", v);
        visualAlertsSize.OnValueChanged     += v => _gameConfig.System.Set("AccessibilitySoundVisualDispSize", (uint)v);
        visualAlertsTransp.OnValueChanged   += v => _gameConfig.System.Set("AccessibilitySoundVisualPermeabilityRate", (uint)v);
        colorFilterRange.OnValueChanged     += v => _gameConfig.System.Set("AccessibilityColorBlindFilterStrength", (uint)v);

        colorBlindModeSelect.OnValueChanged += v => {
            _gameConfig.System.Set(
                "AccessibilityColorBlindFilterType",
                v switch {
                    "None"         => 0,
                    "Protanopia"   => 1,
                    "Deuteranopia" => 2,
                    "Tritanopia"   => 3,
                    _              => 0
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
