namespace Umbra.Widgets;

[ToolbarWidget(
    "Spacer",
    "Widget.Spacer.Name",
    "Widget.Spacer.Description",
    ["separator", "divider", "spacer"]
)]
internal class SpacerWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    protected override StandardWidgetFeatures Features          => StandardWidgetFeatures.CustomizableSize;
    protected override bool                   DefaultDecorate   => false;
    protected override string                 DefaultSizingMode => "Fixed";
    protected override int                    DefaultWidth      => 10;
    
    protected override void OnDraw()
    {
    }
}
