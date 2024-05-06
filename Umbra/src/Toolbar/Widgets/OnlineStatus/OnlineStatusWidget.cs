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

using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.OnlineStatus;

[Service]
internal unsafe partial class OnlineStatusWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.OnlineStatus.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    private readonly IPlayer             _player;
    private readonly IDataManager        _dataManager;
    private readonly IChatSender         _chatSender;
    private readonly ToolbarPopupContext _popupContext;

    public OnlineStatusWidget(IPlayer player, IDataManager dataManager, IChatSender chatSender, ToolbarPopupContext ctx)
    {
        _player       = player;
        _dataManager  = dataManager;
        _chatSender   = chatSender;
        _popupContext = ctx;

        Element.OnMouseEnter += OnMouseEnter;
        Element.OnMouseLeave += OnMouseLeave;

        AddStatusSwitchButton(GetStatusById(47), "/away off");
        AddStatusSwitchButton(GetStatusById(17), "/away");
        AddStatusSwitchButton(GetStatusById(12), "/busy");
        AddStatusSwitchButton(GetStatusById(22), "/roleplaying");

        ctx.RegisterDropdownActivator(Element, _dropdownElement);
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        uint id = _player.OnlineStatusId == 0 ? 47 : _player.OnlineStatusId;

        var status =
            _dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.OnlineStatus>()!.GetRow(id);

        if (null == status) {
            // This should theoretically never happen...
            Element.IsVisible = false;
            return;
        }

        // InfoProxySearchComment.Instance()->InfoProxyInterface;


        Element.IsVisible = true;
        Element.Tooltip   = status.Name.ToString();

        Element.Get("Icon").Style.Image = status.Icon;
    }

    public void OnUpdate() { }

    private void OnMouseEnter()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.BorderLight);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
    }

    private void OnMouseLeave()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
    }

    private Lumina.Excel.GeneratedSheets.OnlineStatus GetStatusById(uint id)
    {
        return _dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.OnlineStatus>()!.GetRow(id)!;
    }
}
