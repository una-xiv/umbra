using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Coordinates",
    "Widget.Coordinates.Name",
    "Widget.Coordinates.Description",
    ["coordinates", "position", "location", "map"]
)]
internal sealed class CoordinatesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    private IZoneManager ZoneManager { get; } = Framework.Service<IZoneManager>();

    protected override void OnDraw()
    {
        if (!ZoneManager.HasCurrentZone) {
            IsVisible = false;
            return;
        }

        IsVisible = true;
        
        Vector2 coords = ZoneManager.CurrentZone.PlayerCoordinates;
        string  x      = I18N.FormatNumber(coords.X);
        string  y      = I18N.FormatNumber(coords.Y);

        if (IsSubTextEnabled) {
            SetText($"{x}");
            SetSubText($"{y}");
        } else {
            SetText($"{x} / {y}");
        }
    }
}
