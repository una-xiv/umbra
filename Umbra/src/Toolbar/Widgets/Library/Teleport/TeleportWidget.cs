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
using Una.Drawing;

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

        Node.OnRightClick += OpenTeleportWindow;
    }

    protected override void OnDisposed()
    {
        Node.OnRightClick -= OpenTeleportWindow;
    }

    protected override void OnUpdate()
    {
        Popup.ExpansionMenuPosition  = GetExpansionMenuPosition();
        Popup.MinimumColumns         = GetConfigValue<int>("MinimumColumns");
        Popup.OpenCategoryOnHover    = GetConfigValue<bool>("OpenCategoryOnHover");
        Popup.OpenFavoritesByDefault = GetConfigValue<bool>("OpenFavoritesByDefault");
        Popup.ShowMapNames           = GetConfigValue<bool>("ShowMapNames");
        Popup.ShowNotification       = GetConfigValue<bool>("ShowNotification");
        Popup.ColumnWidth            = GetConfigValue<int>("ColumnWidth");

        SetDisabled(!Player.CanUseTeleportAction);
        SetLabel(TeleportName);
        SetIcon(GetConfigValue<uint>("IconId"));

        base.OnUpdate();
    }

    private string GetExpansionMenuPosition()
    {
        return GetConfigValue<string>("ExpansionListPosition") switch {
            "Auto"  => Node.ParentNode!.Id == "Right" ? "Right" : "Left",
            "Left"  => "Left",
            "Right" => "Right",
            _       => "Top"
        };
    }

    private void OpenTeleportWindow(Node _)
    {
        Player.UseGeneralAction(7);
    }
}
