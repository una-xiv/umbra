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
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("Flag", "Widget.Flag.Name", "Widget.Flag.Description")]
internal sealed unsafe partial class FlagWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    private IAetheryteList AetheryteList { get; } = Framework.Service<IAetheryteList>();
    private IPlayer        Player        { get; } = Framework.Service<IPlayer>();
    private IToastGui      ToastGui      { get; } = Framework.Service<IToastGui>();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.Tooltip      =  I18N.Translate("Widget.Flag.Tooltip");
        Node.OnClick      += _ => OnClick();
        Node.OnRightClick += _ => OnRightClick();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        if (0 == AgentMap.Instance()->IsFlagMarkerSet) {
            Node.Style.IsVisible = false;
            return;
        }

        SetDisabled(Player is { IsBoundByDuty: false, CanUseTeleportAction: false });
        UpdateWidgetInfoState();

        LeftIconNode.Style.ImageGrayscale  = null == _aetheryteEntry;
        RightIconNode.Style.ImageGrayscale = null == _aetheryteEntry;
        TopLabelNode.Style.TextOffset      = new(0, GetConfigValue<int>("TextYOffsetTop"));
        BottomLabelNode.Style.TextOffset   = new(0, GetConfigValue<int>("TextYOffsetBottom"));
        Node.Style.IsVisible               = true;

        FlagMapMarker* marker = &AgentMap.Instance()->FlagMapMarker;

        SetGhost(!GetConfigValue<bool>("Decorate"));

        switch (GetConfigValue<string>("IconLocation")) {
            case "Left":
                SetLeftIcon(marker->MapMarker.IconId);
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(marker->MapMarker.IconId);
                break;
        }

        switch (GetConfigValue<string>("TextAlign")) {
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

        SetTwoLabels(
            $"{_zoneName}{(_aetheryteName is null ? "" : $" <{_flagCoords}>")}",
            _aetheryteName ?? $"<{_flagCoords}>"
        );

        base.OnUpdate();
    }

    private void OnClick()
    {
        if (!Player.CanUseTeleportAction) {
            ToastGui.ShowError(I18N.Translate("Widget.Flag.Error.CannotTeleport"));
            return;
        }

        if (_aetheryteEntry is null) {
            ToastGui.ShowError(I18N.Translate("Widget.Flag.Error.NoAetheryte"));
            return;
        }

        Telepo.Instance()->Teleport(_aetheryteEntry.AetheryteId, _aetheryteEntry.SubIndex);
    }

    /// <summary>
    /// Removes the flag marker if set.
    /// </summary>
    private static void OnRightClick()
    {
        if (!IsFlagMarkerSet()) return;
        AgentMap.Instance()->IsFlagMarkerSet = 0;
    }
}
