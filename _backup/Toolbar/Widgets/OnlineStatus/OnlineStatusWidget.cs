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

using System.Runtime.CompilerServices;
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
        Element.OnRightClick += OpenSearchInfoWindow;

        AddStatusSwitchButton(GetStatusById(47), true);
        AddStatusSwitchButton(GetStatusById(17), true);
        AddStatusSwitchButton(GetStatusById(12), true);
        AddStatusSwitchButton(GetStatusById(22), true);
        AddStatusSwitchButton(GetStatusById(27), player.IsMentor);
        AddStatusSwitchButton(GetStatusById(28), player.IsBattleMentor);
        AddStatusSwitchButton(GetStatusById(30), player.IsBattleMentor);
        AddStatusSwitchButton(GetStatusById(29), player.IsTradeMentor);

        ctx.RegisterDropdownActivator(Element, _dropdownElement);
    }

    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        uint currentStatusId = _player.OnlineStatusId == 0 ? 47 : _player.OnlineStatusId;
        var status =
            _dataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.OnlineStatus>()!.GetRow(currentStatusId);

        if (null == status) {
            // This should theoretically never happen...
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;
        Element.Tooltip   = status.Name.ToString();

        Element.Get("Icon").Style.Image = status.Icon;

        if (_dropdownElement.IsVisible) {
            AddStatusSwitchButton(GetStatusById(27), _player.IsMentor);
            AddStatusSwitchButton(GetStatusById(28), _player.IsBattleMentor);
            AddStatusSwitchButton(GetStatusById(30), _player.IsBattleMentor);
            AddStatusSwitchButton(GetStatusById(29), _player.IsTradeMentor);
        }
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

    private void OpenSearchInfoWindow()
    {
        InfoModule* infoModule = InfoModule.Instance();
        if (infoModule->IsInCrossWorldDuty()) return;

        ulong localContentId = infoModule->GetLocalContentId();

        InfoProxyCommonList.CharacterData* characterData =
            InfoProxyParty.Instance()->InfoProxyCommonList.GetEntryByContentId(localContentId);

        if (characterData == null) return;

        var updateDataPacket = Unsafe.AsPointer(ref InfoProxySearchComment.Instance()->UpdateData);
        if (null == updateDataPacket) return;

        AgentDetail.Instance()->OpenForCharacterData(characterData, (InfoProxySearchComment.UpdateDataPacket*)updateDataPacket);
    }
}
