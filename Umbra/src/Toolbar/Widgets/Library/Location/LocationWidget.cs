using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.UI;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Location",
    "Widget.Location.Name",
    "Widget.Location.Description",
    ["location", "zone", "district", "coordinates"]
)]
internal class LocationWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text | StandardWidgetFeatures.SubText;

    private readonly IZoneManager _zoneManager = Framework.Service<IZoneManager>();

    protected override void OnLoad()
    {
        Node.OnClick += _ => {
            unsafe {
                UIModule.Instance()->ExecuteMainCommand(16); // Open map.
            }
        };
    }

    protected override void OnDraw()
    {
        if (!_zoneManager.HasCurrentZone) return;
        var zone = _zoneManager.CurrentZone;

        string name = zone.Name;

        if (zone.InstanceId > 0) {
            name += " " + (char)(SeIconChar.Instance1 + ((byte)zone.InstanceId - 1));
        }

        string districtLabel = zone.CurrentDistrictName;

        if (GetConfigValue<bool>("ShowCoordinates")) {
            Vector2 coords = zone.PlayerCoordinates;
            districtLabel = $"X: {I18N.FormatNumber(coords.X)}, Y: {I18N.FormatNumber(coords.Y)}";
        }

        districtLabel = districtLabel.Trim();

        if (string.IsNullOrEmpty(districtLabel) && name.Contains(" - ")) {
            string[] chunks = name.Split(" - ");
            name          = chunks[0];
            districtLabel = string.Join(" - ", chunks[1..]);
        }

        SetText(name);
        SetSubText(districtLabel);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new BooleanWidgetConfigVariable(
                "ShowCoordinates",
                I18N.Translate("Widget.Location.Config.ShowCoordinates.Name"),
                I18N.Translate("Widget.Location.Config.ShowCoordinates.Description"),
                false
            ),
        ];
    }
}
