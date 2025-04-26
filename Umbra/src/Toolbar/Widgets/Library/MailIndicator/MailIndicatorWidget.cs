using Dalamud.Interface;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets;

[ToolbarWidget(
    "MailIndicator",
    "Widget.MailIndicator.Name",
    "Widget.MailIndicator.Description",
    ["mail", "letter", "envelope", "post", "moogle"]
)]
internal partial class MailIndicatorWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon | StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Envelope;

    protected override void OnDraw()
    {
        uint unreadMailCount = GetUnreadMailCount();

        SetDisabled(unreadMailCount == 0);

        Node.Tooltip = I18N.Translate($"Widget.MailIndicator.Tooltip.{(unreadMailCount == 1 ? "Singular" : "Plural")}", unreadMailCount.ToString());
        IsVisible    = GetConfigValue<bool>("AlwaysShow") || (unreadMailCount > 0);
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                "AlwaysShow",
                I18N.Translate("Widget.MailIndicator.Config.AlwaysShow.Name"),
                I18N.Translate("Widget.MailIndicator.Config.AlwaysShow.Description"),
                false
            ),
        ];
    }
}
