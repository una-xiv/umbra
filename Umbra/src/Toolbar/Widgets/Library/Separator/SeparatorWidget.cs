using System.Collections.Generic;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Separator",
    "Widget.Separator.Name",
    "Widget.Separator.Description",
    ["separator", "line", "divider", "spacer"]
)]
internal class SeparatorWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    public override Node Node { get; } = new() {
        Style = new() {
            Anchor = Anchor.MiddleLeft,
            Size   = new(2, SafeHeight),
        },
        ChildNodes = [
            new() {
                Id = "Line",
                Style = new() {
                    Anchor          = Anchor.MiddleCenter,
                    BackgroundColor = new("Toolbar.Border"),
                    Size            = new(1, SafeHeight),
                }
            }
        ]
    };

    protected override void Initialize() { }

    protected override void OnUpdate()
    {
        Node.Style.Size = new(GetConfigValue<int>("Width"), SafeHeight);
        Node line = Node.FindById("Line")!;

        line.Style.Size            = new(GetConfigValue<int>("LineWidth"), GetConfigValue<int>("LineHeight"));
        line.Style.BackgroundColor = new(GetConfigValue<string>("LineColor"));
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new IntegerWidgetConfigVariable(
                "Width",
                I18N.Translate("Widget.Separator.Config.Width.Name"),
                I18N.Translate("Widget.Separator.Config.Width.Description"),
                10,
                1,
                2000
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "LineWidth",
                I18N.Translate("Widget.Separator.Config.LineWidth.Name"),
                I18N.Translate("Widget.Separator.Config.LineWidth.Description"),
                1,
                1,
                2000
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new IntegerWidgetConfigVariable(
                "LineHeight",
                I18N.Translate("Widget.Separator.Config.LineHeight.Name"),
                I18N.Translate("Widget.Separator.Config.LineHeight.Description"),
                20,
                1,
                2000
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
            new SelectWidgetConfigVariable(
                "LineColor",
                I18N.Translate("Widget.Separator.Config.LineColor.Name"),
                I18N.Translate("Widget.Separator.Config.LineColor.Description"),
                "Widget.TextMuted",
                GetColorSelectOptions()
            ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        ];
    }
}
