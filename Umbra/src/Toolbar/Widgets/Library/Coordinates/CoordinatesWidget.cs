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

    private IZoneManager ZoneManager { get; } = Framework.Service<IZoneManager>();

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "UseTwoLabels",
                I18N.Translate("Widget.Coordinates.Config.UseTwoLabels.Name"),
                I18N.Translate("Widget.Coordinates.Config.UseTwoLabels.Description"),
                false
            ),
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..TwoLabelTextOffsetVariables,
        ];
    }

    protected override void Initialize()
    {
        BottomLabelNode.Style.Color = new("Widget.Text");
    }

    protected override void OnUpdate()
    {
        if (!ZoneManager.HasCurrentZone) {
            Node.Style.IsVisible = false;
            return;
        }

        Node.Style.IsVisible        = true;

        Vector2 coords = ZoneManager.CurrentZone.PlayerCoordinates;
        string  x      = I18N.FormatNumber(coords.X);
        string  y      = I18N.FormatNumber(coords.Y);

        if (GetConfigValue<bool>("UseTwoLabels")) {
            SetTwoLabels(x, y);
        } else {
            SetLabel($"{x}, {y}");
        }

        base.OnUpdate();
    }
}
