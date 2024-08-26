using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.Coordinates;

[ToolbarWidget("Coordinates", "Widget.Coordinates.Name", "Widget.Coordinates.Description")]
internal sealed class CoordinatesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    protected override void Initialize() { }

    private IZoneManager ZoneManager { get; } = Framework.Service<IZoneManager>();

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
        ];
    }

    protected override void OnUpdate()
    {
        if (!ZoneManager.HasCurrentZone) {
            Node.Style.IsVisible = false;
            return;
        }

        Node.Style.IsVisible = true;

        Vector2 coords = ZoneManager.CurrentZone.PlayerCoordinates;
        SetLabel($"{coords.X.ToString("0.0", CultureInfo.InvariantCulture)}, {coords.Y.ToString("0.0", CultureInfo.InvariantCulture)}");

        base.OnUpdate();
    }
}
