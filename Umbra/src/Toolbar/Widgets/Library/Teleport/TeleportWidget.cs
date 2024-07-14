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
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("Teleport", "Widget.Teleport.Name", "Widget.Teleport.Description")]
internal sealed partial class TeleportWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override TeleportWidgetPopup Popup { get; } = new();

    private IPlayer Player { get; set; } = null!;

    private string TeleportName { get; set; } = null!;
    private uint   TeleportIcon { get; set; }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Player = Framework.Service<IPlayer>();

        var teleportAction = Framework.Service<IDataManager>().GetExcelSheet<GeneralAction>()!.GetRow(7)!;

        TeleportName = teleportAction.Name.ToString();
        TeleportIcon = (uint)teleportAction.Icon;
    }

    protected override void OnUpdate()
    {
        bool showText = GetConfigValue<string>("DisplayMode") != "IconOnly";
        bool showIcon = GetConfigValue<string>("DisplayMode") != "TextOnly";
        bool leftIcon = GetConfigValue<string>("IconLocation") == "Left";

        SetLabel(showText ? TeleportName : null);
        SetGhost(!GetConfigValue<bool>("Decorate"));

        LeftIconNode.Style.Margin  = new(0, 0, 0, showText ? -2 : 0);
        RightIconNode.Style.Margin = new(0, showText ? -2 : 0, 0, 0);
        LabelNode.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffset"));
        Node.Style.Padding         = new(0, showText ? 6 : 3);
        Node.Tooltip               = !showText ? TeleportName : null;

        LeftIconNode.Style.ImageGrayscale  = GetConfigValue<bool>("DesaturateIcon");
        RightIconNode.Style.ImageGrayscale = GetConfigValue<bool>("DesaturateIcon");

        if (showIcon) {
            var desaturate = GetConfigValue<bool>("DesaturateIcon");
            LeftIconNode.Style.ImageGrayscale  = desaturate;
            RightIconNode.Style.ImageGrayscale = desaturate;

            if (leftIcon) {
                SetLeftIcon(TeleportIcon);
                SetRightIcon(null);
            } else {
                SetLeftIcon(null);
                SetRightIcon(TeleportIcon);
            }
        } else {
            SetLeftIcon(null);
            SetRightIcon(null);
        }

        // No point in showing the menu if the player isn't allowed to teleport anyway.
        SetDisabled(!Player.CanUseTeleportAction);
    }
}
