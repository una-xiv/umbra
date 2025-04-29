using Dalamud.Interface;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.SanctuaryIndicator;

[ToolbarWidget(
    "SanctuaryIndicator",
    "Widget.SanctuaryIndicator.Name",
    "Widget.SanctuaryIndicator.Description",
    ["sanctuary", "city", "town", "housing", "rest", "afk", "indicator"]
)]
internal sealed class SanctuaryIndicator(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon | StandardWidgetFeatures.CustomizableIcon;

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.Moon;

    protected override void OnDraw()
    {
        Node.Style.IsVisible = Framework.Service<IPlayer>().IsInSanctuary;
    }
}
