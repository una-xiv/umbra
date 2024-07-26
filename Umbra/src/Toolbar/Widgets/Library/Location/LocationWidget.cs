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
    private IPlayer?      _player;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _zoneManager = Framework.Service<IZoneManager>();
        _player      = Framework.Service<IPlayer>();

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

        SetGhost(!GetConfigValue<bool>("Decorate"));
        bool showDistrict = GetConfigValue<bool>("ShowDistrict");
        bool useTwoLabels = GetConfigValue<bool>("UseTwoLabels");

        if (useTwoLabels && showDistrict) {
            SetTwoLabels(name, zone.CurrentDistrictName);
            TopLabelNode.Style.TextOffset    = new(0, GetConfigValue<int>("TextYOffsetTop"));
            BottomLabelNode.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffsetBottom"));
        } else {
            SetLabel(showDistrict ? $"{name} - {zone.CurrentDistrictName}" : name);
            LabelNode.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffset"));
        }

        var textAlign = GetConfigValue<string>("TextAlign");

        switch (textAlign) {
            case "Left":
                SetTextAlignLeft();
                break;
            case "Center":
                SetTextAlignCenter();
                break;
            case "Right":
                SetTextAlignRight();
                break;
        }

        base.OnUpdate();
    }
}
