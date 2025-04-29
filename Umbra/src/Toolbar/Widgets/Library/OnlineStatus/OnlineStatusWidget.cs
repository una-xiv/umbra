using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget(
    "OnlineStatus",
    "Widget.OnlineStatus.Name",
    "Widget.OnlineStatus.Description",
    ["status", "online", "busy", "away", "mentor", "party", "roleplay"]
)]
internal partial class OnlineStatusWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new() { UseGrayscaleIcons = false };

    protected override StandardWidgetFeatures Features => StandardWidgetFeatures.Icon | StandardWidgetFeatures.Text;

    private readonly Dictionary<uint, MenuPopup.Button> _statusButtons = [];

    private IPlayer _player = null!;

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        Node.OnRightClick += _ => OpenSearchInfoWindow();
        Node.Tooltip      =  I18N.Translate("Widget.OnlineStatus.Tooltip");

        Popup.UseGrayscaleIcons = false;

        _player = Framework.Service<IPlayer>();
    }

    /// <inheritdoc/>
    protected override void OnDraw()
    {
        SetPopupStatusOption(47, true);                                // Online
        SetPopupStatusOption(17, true);                                // Away from Keyboard
        SetPopupStatusOption(12, true);                                // Busy
        SetPopupStatusOption(22, true);                                // Role-Playing
        SetPopupStatusOption(21, _player.IsGeneralActionUnlocked(13)); // Looking to Meld Materia
        SetPopupStatusOption(23, !_player.IsInParty);                  // Looking for Party
        SetPopupStatusOption(32, _player.IsNovice);                    // New Adventurer
        SetPopupStatusOption(31, _player.IsReturner);                  // Returning
        SetPopupStatusOption(27, _player.IsMentor);                    // Mentor
        SetPopupStatusOption(28, _player.IsBattleMentor);              // PvE Battle Mentor
        SetPopupStatusOption(30, _player.IsBattleMentor);              // PvP Battle Mentor
        SetPopupStatusOption(29, _player.IsTradeMentor);               // Trade Mentor

        uint statusId = _player.OnlineStatusId == 0 ? 47 : _player.OnlineStatusId;
        var  status   = GetStatusById(statusId);

        SetText(status.Name.ExtractText());
        SetGameIconId(status.Icon);
    }

    private void SetPopupStatusOption(uint statusId, bool isAvailable)
    {
        var status = GetStatusById(statusId);

        if (!_statusButtons.TryGetValue(statusId, out var button)) {
            button = new MenuPopup.Button($"Status_{statusId}") {
                Label             = status.Name.ExtractText(),
                Icon              = status.Icon,
                ClosePopupOnClick = true,
                OnClick           = () => _player.SetOnlineStatus(statusId),
            };

            _statusButtons.Add(statusId, button);
            Popup.Add(button);
        }

        button.IsVisible = isAvailable;
    }
}
