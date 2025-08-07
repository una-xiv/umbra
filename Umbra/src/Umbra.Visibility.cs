using Dalamud.Game.ClientState.Conditions;

namespace Umbra;

[Service]
public sealed class UmbraVisibility
{
    [ConfigVariable("General.ShowToolbarInCutscenes", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarInCutscenes { get; set; } = false;

    [ConfigVariable("General.ShowToolbarInGPose", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarInGPose { get; set; } = false;

    [ConfigVariable("General.ShowToolbarOnUserHide", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarOnUserHide { get; set; } = false;

    [ConfigVariable("General.ShowToolbarBetweenAreas", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarBetweenAreas { get; set; } = false;

    [ConfigVariable("General.ShowToolbarDuringIdleCam", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarDuringIdleCam { get; set; } = false;

    [ConfigVariable("General.ShowToolbarInDuty", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarInDuty { get; set; } = true;

    [ConfigVariable("General.ShowToolbarInCombat", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarInCombat { get; set; } = true;

    [ConfigVariable("General.ShowMarkersInCutscenes", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersInCutscenes { get; set; } = false;

    [ConfigVariable("General.ShowMarkersInGPose", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersInGPose { get; set; } = false;

    [ConfigVariable("General.ShowMarkersOnUserHide", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersOnUserHide { get; set; } = false;

    [ConfigVariable("General.ShowMarkersBetweenAreas", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersBetweenAreas { get; set; } = false;

    [ConfigVariable("General.ShowMarkersDuringIdleCam", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersDuringIdleCam { get; set; } = false;

    [ConfigVariable("General.ShowMarkersInDuty", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersInDuty { get; set; } = true;

    [ConfigVariable("General.ShowMarkersInCombat", "General", "MarkersVisibilitySettings")]
    public static bool ShowMarkersInCombat { get; set; } = true;

    private readonly IClientState _clientState;
    private readonly IGameGui     _gameGui;
    private readonly IPlayer      _player;
    private readonly ICondition   _condition;

    public UmbraVisibility(IClientState clientState, IGameGui gameGui, IPlayer player, ICondition condition)
    {
        _clientState = clientState;
        _gameGui     = gameGui;
        _player      = player;
        _condition   = condition;

        Framework.DalamudPlugin.UiBuilder.DisableAutomaticUiHide = true;
        Framework.DalamudPlugin.UiBuilder.DisableCutsceneUiHide  = true;
        Framework.DalamudPlugin.UiBuilder.DisableGposeUiHide     = true;
        Framework.DalamudPlugin.UiBuilder.DisableUserUiHide      = true;
    }

    public bool IsToolbarVisible()
    {
        // Always disable when visiting the aesthetician.
        if (_condition[ConditionFlag.CreatingCharacter]) return false;

        if (_clientState.IsGPosing && !ShowToolbarInGPose) return false;

        if (_gameGui.GameUiHidden && !ShowToolbarOnUserHide) return false;

        if (_player.IsInCutscene && !ShowToolbarInCutscenes) return false;

        if (_player.IsBetweenAreas && !ShowToolbarBetweenAreas) return false;

        if (_player.IsInIdleCam && !ShowToolbarDuringIdleCam) return false;

        if (_player.IsInCombat && !ShowToolbarInCombat) return false;

        if (_player.IsBoundByDuty && !ShowToolbarInDuty) return false;

        return true;
    }

    public bool AreMarkersVisible()
    {
        // Always disable when visiting the aesthetician.
        if (_condition[ConditionFlag.CreatingCharacter]) return false;

        // Always disable markers when in PvP.
        if (_player.IsInPvP) return false;

        if (_clientState.IsGPosing && !ShowMarkersInGPose) return false;

        if (_gameGui.GameUiHidden && !ShowMarkersOnUserHide) return false;

        if (_player.IsInCutscene && !ShowMarkersInCutscenes) return false;

        if (_player.IsBetweenAreas && !ShowMarkersBetweenAreas) return false;

        if (_player.IsInIdleCam && !ShowMarkersDuringIdleCam) return false;

        if (_player.IsInCombat && !ShowMarkersInCombat) return false;

        if (_player.IsBoundByDuty && !ShowMarkersInDuty) return false;

        return true;
    }
}
