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
) : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    public override Node Node { get; } = new() {
        ClassList = ["toolbar-widget-spacer"],
        Style = new() {
            Anchor = Anchor.MiddleLeft,
            Size   = new(2, SafeHeight),
        },
    };

    protected override void Initialize()
    {
    }

    protected override void OnUpdate()
    {
        Node.IsDisabled = true;
        Node.Style.Size = new(GetConfigValue<int>("Width"), SafeHeight);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new IntegerWidgetConfigVariable(
                "Width",
                I18N.Translate("Widget.Spacer.Config.Width.Name"),
                I18N.Translate("Widget.Spacer.Config.Width.Description"),
                10,
                0,
                2000
            )
        ];
    }
}
