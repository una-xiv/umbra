namespace Umbra.Widgets;

[ToolbarWidget(
    "Accessibility", 
    "Widget.Accessibility.Name", 
    "Widget.Accessibility.Description", 
    ["config", "settings", "accessibility"]
)]
internal sealed class AccessibilityWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override AccessibilityWidgetPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon | StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType => IconTypeFontAwesome;

    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Wheelchair;
}
