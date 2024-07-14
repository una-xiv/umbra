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
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.InventorySpace;

[ToolbarWidget("InventorySpace", "Widget.InventorySpace.Name", "Widget.InventorySpace.Description")]
internal partial class InventorySpaceWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    private IPlayer Player    { get; set; } = null!;
    private uint    FreeSpace { get; set; }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Player = Framework.Service<IPlayer>();

        Node.OnClick += _ => Framework.Service<IChatSender>().Send("/inventory");

        SetLeftIcon(2);
        SetLabel("140 / 140");
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        FreeSpace = Player.GetFreeInventorySpace();
        var iconLocation = GetConfigValue<string>("IconLocation");

        SetGhost(!GetConfigValue<bool>("Decorate"));

        switch (iconLocation) {
            case "Left":
                SetLeftIcon(GetIconId());
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(GetIconId());
                break;
        }

        var l = iconLocation == "Left" ? " " : "";
        var r = iconLocation == "Right" ? " " : "";
        SetLabel(GetConfigValue<bool>("ShowTotal") ? $"{l}{FreeSpace} / 140{r}" : $"{l}{FreeSpace}{r}");

        LabelNode.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffset"));
    }

    private uint GetIconId()
    {
        LeftIconNode.Style.ImageGrayscale  = false;
        RightIconNode.Style.ImageGrayscale = false;

        if (FreeSpace <= GetConfigValue<int>("CriticalThreshold")) {
            return 60074; // Critical
        }

        if (FreeSpace <= GetConfigValue<int>("WarningThreshold")) {
            return 60073; // Warning
        }

        LeftIconNode.Style.ImageGrayscale  = GetConfigValue<bool>("DesaturateIcon");
        RightIconNode.Style.ImageGrayscale = GetConfigValue<bool>("DesaturateIcon");

        return 2;
    }
}
