using System.Collections.Generic;
using Dalamud.Interface;
using Umbra.Style;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra.Widgets;

public abstract class IconToolbarWidget(
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
        ],
    };

    protected void SetIcon(FontAwesomeIcon icon)
    {
        Node iconNode = Node.QuerySelector("#Icon")!;
        iconNode.NodeValue = icon.ToIconString();
    }

    protected void SetGhost(bool ghost)
    {
        switch (ghost)
        {
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
}
