using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.RetainerList;

[ToolbarWidget("RetainerList", "Widget.RetainerList.Name", "Widget.RetainerList.Description")]
internal partial class RetainerListWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    /// <inheritdoc/>
    public override RetainerListPopup Popup { get; } = new();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetLabel(Info.Name);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        // Not allowed to use the retainer widget while on a different world or in an instance.
        // The game performs the same checks when opening the timer window. (Thanks Hasel!)
        SetDisabled(Player.CurrentWorldName != Player.HomeWorldName || Player.IsBoundByInstancedDuty);

        Popup.JobIconType = GetConfigValue<string>("IconType");

        UpdateIcons();
        SetGhost(!GetConfigValue<bool>("Decorate"));

        string displayMode = GetConfigValue<string>("DisplayMode");
        bool   showLabel   = displayMode is "TextOnly" or "TextAndIcon";

        Node.Tooltip               = showLabel ? null : Info.Name;
        LabelNode.Style.IsVisible  = showLabel;
        LabelNode.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffset"));
        LeftIconNode.Style.Margin  = new() { Left  = showLabel ? -3 : 0 };
        RightIconNode.Style.Margin = new() { Right = showLabel ? -3 : 0 };
        Node.Style.Padding         = new(0, showLabel ? 6 : 4);
    }

    private void UpdateIcons()
    {
        if (GetConfigValue<string>("DisplayMode") is "TextOnly") {
            SetLeftIcon(null);
            SetRightIcon(null);
            return;
        }

        bool useGrayscaleIcons = GetConfigValue<bool>("DesaturateIcon");

        LeftIconNode.Style.ImageGrayscale = useGrayscaleIcons;
        RightIconNode.Style.ImageGrayscale = useGrayscaleIcons;

        switch (GetConfigValue<string>("IconLocation")) {
            case "Left":
                SetLeftIcon(60560u);
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(60560u);
                break;
        }
    }
}
