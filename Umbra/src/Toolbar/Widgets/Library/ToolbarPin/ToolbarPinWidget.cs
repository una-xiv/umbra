namespace Umbra.Widgets;

[ToolbarWidget(
    "ToolbarPin", 
    "Widget.ToolbarPin.Name", 
    "Widget.ToolbarPin.Description", 
    ["pin", "lock", "unlock", "autohide"]
)]
internal class ToolbarPinWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon;

    protected override void OnLoad()
    {
        Node.OnClick += _ => { ConfigManager.Set("Toolbar.IsAutoHideEnabled", !Toolbar.IsAutoHideEnabled); };
    }

    protected override void OnDraw()
    {
        SetFontAwesomeIcon(Toolbar.IsAutoHideEnabled
            ? GetConfigValue<FontAwesomeIcon>("UnlockIcon")
            : GetConfigValue<FontAwesomeIcon>("LockIcon")
        );
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new FaIconWidgetConfigVariable(
                "LockIcon",
                I18N.Translate("Widget.ToolbarPin.Config.LockIcon.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.LockIcon.Description"),
                FontAwesomeIcon.Lock
            ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") },
            new FaIconWidgetConfigVariable(
                "UnlockIcon",
                I18N.Translate("Widget.ToolbarPin.Config.UnlockIcon.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.UnlockIcon.Description"),
                FontAwesomeIcon.LockOpen
            ) { Category = I18N.Translate("Widgets.Standard.Config.Category.Icon") },

            ..base.GetConfigVariables(),
        ];
    }
}
