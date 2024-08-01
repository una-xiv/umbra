/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.UI;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("Location", "Widget.Location.Name", "Widget.Location.Description")]
internal partial class LocationWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup { get; } = null;

    private IZoneManager? _zoneManager;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _zoneManager = Framework.Service<IZoneManager>();

        SetGhost(!GetConfigValue<bool>("Decorate"));
        SetTwoLabels("Location Name", "District Name");

        Node.OnClick += _ => {
            unsafe {
                // Open map.
                UIModule.Instance()->ExecuteMainCommand(16);
            }
        };
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        if (_zoneManager is null || !_zoneManager.HasCurrentZone) return;
        var zone = _zoneManager.CurrentZone;

        string name = zone.Name;

        if (zone.InstanceId > 0) {
            name += " " + (char)(SeIconChar.Instance1 + ((byte)zone.InstanceId - 1));
        }

        bool showDistrict = GetConfigValue<bool>("ShowDistrict");
        bool useTwoLabels = GetConfigValue<bool>("UseTwoLabels");

        string districtLabel = showDistrict ? zone.CurrentDistrictName : string.Empty;

        if (showDistrict && GetConfigValue<bool>("ShowCoordinates")) {
            Vector2 coords = zone.PlayerCoordinates;
            districtLabel = $"X: {coords.X:F1}, Y: {coords.Y:F1}";
        }

        if (useTwoLabels && showDistrict) {
            SetTwoLabels(name, districtLabel);
        } else {
            SetLabel(showDistrict ? $"{name} - {districtLabel}" : name);
        }

        base.OnUpdate();
    }
}
