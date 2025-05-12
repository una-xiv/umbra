using Umbra.Widgets.Popup;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private PopupDisplayMode DisplayMode            { get; set; } = PopupDisplayMode.Extended;
    private bool             ShowNotification       { get; set; } = true;
    private string           DefaultOpenedGroupName { get; set; } = "Auto";
    private bool             OpenCategoryOnHover    { get; set; } = false;
    private int              PopupHeight            { get; set; } = 400;
    private int              PopupFontSize          { get; set; } = 11;
    private bool             ShowMapNames           { get; set; } = true;

    protected override void UpdateConfigVariables(ToolbarWidget widget)
    {
        DisplayMode            = widget.GetConfigValue<PopupDisplayMode>("PopupDisplayMode");
        ShowNotification       = widget.GetConfigValue<bool>("ShowNotification");
        DefaultOpenedGroupName = widget.GetConfigValue<string>("DefaultOpenedGroupName");
        OpenCategoryOnHover    = widget.GetConfigValue<bool>("OpenCategoryOnHover");
        PopupHeight            = widget.GetConfigValue<int>("PopupHeight");
        PopupFontSize          = widget.GetConfigValue<int>("PopupFontSize");
        ShowMapNames           = widget.GetConfigValue<bool>("ShowMapNames");
    }
}
