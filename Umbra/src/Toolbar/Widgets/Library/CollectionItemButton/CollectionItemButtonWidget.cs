using Lumina.Data.Parsing;
using System.Collections.Generic;
using System.Numerics;

namespace Umbra.Widgets.Library.CollectionItemButton;

[ToolbarWidget("CollectionItemButton", "Widget.CollectionItemButton.Name", "Widget.CollectionItemButton.Description")]
internal sealed partial class CollectionItemButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup { get; } = null;

    protected override void Initialize()
    {
        LoadCollectionItems();
        SetLabel("Collection");

        Node.OnMouseUp += Invoke;
    }

    protected override void OnDisposed()
    {
        Node.OnMouseUp -= Invoke;
    }

    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));

        if (!Items.TryGetValue(GetConfigValue<string>("Item"), out var item)) return;

        var displayMode    = GetConfigValue<string>("DisplayMode");
        var iconLocation   = GetConfigValue<string>("IconLocation");
        var desaturateIcon = GetConfigValue<bool>("DesaturateIcon");
        var iconOffset     = new Vector2(0, GetConfigValue<int>("IconYOffset"));
        var textOffset     = new Vector2(0, GetConfigValue<int>("TextYOffset"));
        var hasLabel       = displayMode is "TextAndIcon" or "TextOnly";
        var hasIcon        = displayMode is "TextAndIcon" or "IconOnly";

        if (displayMode is "TextAndIcon" or "IconOnly") {
            switch (iconLocation) {
                case "Left":
                    SetLeftIcon(item.Icon);
                    SetRightIcon(null);
                    break;
                case "Right":
                    SetLeftIcon(null);
                    SetRightIcon(item.Icon);
                    break;
            }
        } else {
            SetLeftIcon(null);
            SetRightIcon(null);
        }

        SetLabel(hasLabel ? item.Name : null);

        LabelNode.Style.TextOffset         = textOffset;
        LeftIconNode.Style.ImageOffset     = iconOffset;
        RightIconNode.Style.ImageOffset    = iconOffset;
        LeftIconNode.Style.ImageGrayscale  = Node.IsDisabled || desaturateIcon;
        RightIconNode.Style.ImageGrayscale = Node.IsDisabled || desaturateIcon;
        LeftIconNode.Style.Margin          = new(0, 0, 0, 0);
        RightIconNode.Style.Margin         = new(0, 0, 0, 0);
        Node.Style.Padding                 = new() { Left = hasIcon ? 3 : 0, Right = hasIcon ? 3 : 0 };
        Node.Tooltip                       = displayMode is "IconOnly" ? item.Name : null;

        base.OnUpdate();
    }
}
