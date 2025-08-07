using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Flag",
    "Widget.Flag.Name",
    "Widget.Flag.Description",
    ["flag", "marker", "teleport", "chat"]
)]
internal sealed unsafe partial class FlagWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    private IAetheryteList AetheryteList { get; } = Framework.Service<IAetheryteList>();
    private IPlayer        Player        { get; } = Framework.Service<IPlayer>();
    private IToastGui      ToastGui      { get; } = Framework.Service<IToastGui>();

    private long _lastBroadcast;

    protected override void OnLoad()
    {
        Node.Tooltip       =  I18N.Translate("Widget.Flag.Tooltip");
        Node.OnClick       += _ => OnClick();
        Node.OnRightClick  += _ => OnRightClick();
        Node.OnMiddleClick += _ => Broadcast();

        ZoneManager.ZoneChanged += OnZoneChanged;
    }

    protected override void OnUnload()
    {
        ZoneManager.ZoneChanged -= OnZoneChanged;
    }

    protected override void OnDraw()
    {
        if (AgentMap.Instance()->FlagMarkerCount == 0) {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        SetDisabled(Player is { IsBoundByDuty: false, CanUseTeleportAction: false });
        UpdateWidgetInfoState();

        FlagMapMarker marker = AgentMap.Instance()->FlagMapMarkers[0];

        SetGameIconId(marker.MapMarker.IconId);
        SetText($"{_zoneName}{(_aetheryteName is null ? "" : $" <{_flagCoords}>")}");
        SetSubText(_aetheryteName ?? $"<{_flagCoords}>");
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            new StringWidgetConfigVariable(
                "ChatMessagePrefix",
                I18N.Translate("Widget.Flag.Config.ChatMessagePrefix.Name"),
                I18N.Translate("Widget.Flag.Config.ChatMessagePrefix.Description"),
                ""
            )
        ];
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

    private static void OnRightClick()
    {
        if (!IsFlagMarkerSet()) return;
        AgentMap.Instance()->FlagMarkerCount = 0;
    }

    private void Broadcast()
    {
        if (!IsFlagMarkerSet()) return;

        // Make sure a chat prefix message has actually been set.
        string chatMessagePrefix = GetConfigValue<string>("ChatMessagePrefix");
        if (string.IsNullOrEmpty(chatMessagePrefix)) {
            return;
        }

        long now = DateTimeOffset.Now.ToUnixTimeSeconds();

        // Prohibit spamming the chat.
        if (now - _lastBroadcast < 3) {
            Framework.Service<IChatGui>().PrintError(I18N.Translate("Widget.Flag.Error.SpamProtect"));
            return;
        }

        _lastBroadcast = now;

        Framework.Service<IChatSender>().Send($"{chatMessagePrefix} <flag>");
    }

    private void OnZoneChanged(IZone _)
    {
        _aetheryteKey = null;
    }
}
