﻿using Umbra.Widgets.Popup;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup
{
    private PopupDisplayMode DisplayMode            { get; set; } = PopupDisplayMode.Extended;
    private bool             ShowNotification       { get; set; } = true;
    private string           DefaultOpenedGroupName { get; set; } = "Auto";
    private bool             OpenCategoryOnHover    { get; set; } = false;

    protected override void UpdateConfigVariables(ToolbarWidget widget)
    {
        DisplayMode            = widget.GetConfigValue<PopupDisplayMode>("PopupDisplayMode");
        ShowNotification       = widget.GetConfigValue<bool>("ShowNotification");
        DefaultOpenedGroupName = widget.GetConfigValue<string>("DefaultOpenedGroupName");
        OpenCategoryOnHover    = widget.GetConfigValue<bool>("OpenCategoryOnHover");
    }
}
