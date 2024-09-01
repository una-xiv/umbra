using System.Collections.Generic;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Style;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra.Widgets;

internal abstract class IconToolbarWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : ToolbarWidget(info, guid, configValues)
{
    public sealed override Node Node { get; } = new() {
        Stylesheet = WidgetStyles.IconButtonStylesheet,
        ClassList  = ["toolbar-widget-icon"],
        ChildNodes = [
            new() {
                Id        = "Icon",
                NodeValue = FontAwesomeIcon.Ankh.ToIconString(),
            },
        ]
    };

    protected void SetIcon(FontAwesomeIcon icon)
    {
        Node iconNode = Node.QuerySelector("#Icon")!;
        iconNode.NodeValue = icon.ToIconString();
    }

    protected void SetGhost(bool ghost)
    {
        switch (ghost) {
            case true when !Node.TagsList.Contains("ghost"):
                Node.TagsList.Add("ghost");
                break;
            case false when Node.TagsList.Contains("ghost"):
                Node.TagsList.Remove("ghost");
                break;
        }
    }

    protected void SetDisabled(bool disabled)
    {
        Node.IsDisabled = disabled;
    }

    protected void SetIconYOffset(int offset)
    {
        Node.QuerySelector("#Icon")!.Style.TextOffset = new(0, offset);
    }

    protected override void OnUpdate()
    {
        bool isGhost  = !GetConfigValue<bool>("Decorate");
        int  iconSize = GetConfigValue<int>("IconSize");
        int  hPadding = GetConfigValue<int>("HorizontalPadding");

        SetIconYOffset(GetConfigValue<int>("IconYOffset"));
        SetGhost(isGhost);

        Node.Style.Size         = new(isGhost ? 0 : SafeHeight + hPadding, SafeHeight);
        IconNode.Style.Padding  = new(0, hPadding);
        IconNode.Style.Size     = new(isGhost ? 0 : (SafeHeight - 2) + hPadding, SafeHeight - 2);
        IconNode.Style.FontSize = iconSize < 6 ? (SafeHeight) / 2 : iconSize;
    }

    protected Node IconNode => Node.FindById("Icon")!;

    protected IEnumerable<IWidgetConfigVariable> DefaultIconToolbarWidgetConfigVariables => [
        new BooleanWidgetConfigVariable(
            "Decorate",
            I18N.Translate("Widgets.IconButtonWidget.Config.Decorate.Name"),
            I18N.Translate("Widgets.IconButtonWidget.Config.Decorate.Description"),
            true
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "IconSize",
            I18N.Translate("Widgets.IconButtonWidget.Config.IconSize.Name"),
            I18N.Translate("Widgets.IconButtonWidget.Config.IconSize.Description"),
            0,
            0,
            64
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "IconYOffset",
            I18N.Translate("Widgets.IconButtonWidget.Config.IconYOffset.Name"),
            I18N.Translate("Widgets.IconButtonWidget.Config.IconYOffset.Description"),
            -1,
            -5,
            5
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
        new IntegerWidgetConfigVariable(
            "HorizontalPadding",
            I18N.Translate("Widgets.IconButtonWidget.Config.HorizontalPadding.Name"),
            I18N.Translate("Widgets.IconButtonWidget.Config.HorizontalPadding.Description"),
            0,
            0,
            100
        ) { Category = I18N.Translate("Widget.ConfigCategory.WidgetAppearance") },
    ];
}
