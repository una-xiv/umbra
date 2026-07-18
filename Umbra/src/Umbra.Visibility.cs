using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using TerritoryIntendedUse = FFXIVClientStructs.FFXIV.Client.Enums.TerritoryIntendedUse;

namespace Umbra;

[Service]
public sealed class UmbraVisibility
{
    // フレームキャッシュ: 同一フレーム内での重複計算を避ける
    private bool _cachedToolbarVisible;
    private bool _cachedMarkersVisible;
    private bool _visibilityCacheValid;

    [OnDraw(executionOrder: int.MinValue)]
    private void InvalidateVisibilityCache()
    {
        _visibilityCacheValid = false;
    }


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

    [ConfigVariable("General.ShowToolbarInHudEditing", "General", "ToolbarVisibilitySettings")]
    public static bool ShowToolbarInHudEditing { get; set; } = false;

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

    public unsafe bool IsToolbarVisible()
    {
        if (!_visibilityCacheValid) ComputeVisibilityCache();
        return _cachedToolbarVisible;
    }

    public unsafe bool AreMarkersVisible()
    {
        if (!_visibilityCacheValid) ComputeVisibilityCache();
        return _cachedMarkersVisible;
    }

    private unsafe void ComputeVisibilityCache()
    {
        _visibilityCacheValid = true;

        // 美容師訪問中は常に非表示
        if (_condition[ConditionFlag.CreatingCharacter]) {
            _cachedToolbarVisible = false;
            _cachedMarkersVisible = false;
            return;
        }

        bool isInAirForceOne = GameMain.Instance()->CurrentTerritoryIntendedUseId == TerritoryIntendedUse.AirForceOne;
        bool isGPosing       = _clientState.IsGPosing;
        bool isUiHidden      = _gameGui.GameUiHidden;
        bool isInCutscene    = _player.IsInCutscene;
        bool isBetweenAreas  = _player.IsBetweenAreas;
        bool isInIdleCam     = _player.IsInIdleCam;
        bool isInCombat      = _player.IsInCombat;
        bool isBoundByDuty   = _player.IsBoundByDuty;
        bool isEditingHud    = RaptureAtkUnitManager.Instance()->IsEditingHudLayout;
        bool isInPvP         = _player.IsInPvP;

        _cachedToolbarVisible =
            !isInAirForceOne                                 &&
            !(isGPosing      && !ShowToolbarInGPose)         &&
            !(isUiHidden     && !ShowToolbarOnUserHide)      &&
            !(isInCutscene   && !ShowToolbarInCutscenes)     &&
            !(isBetweenAreas && !ShowToolbarBetweenAreas)    &&
            !(isInIdleCam    && !ShowToolbarDuringIdleCam)   &&
            !(isInCombat     && !ShowToolbarInCombat)        &&
            !(isBoundByDuty  && !ShowToolbarInDuty)          &&
            !(isEditingHud   && !ShowToolbarInHudEditing);

        _cachedMarkersVisible =
            !isInPvP                                         &&
            !isInAirForceOne                                 &&
            !(isGPosing      && !ShowMarkersInGPose)         &&
            !(isUiHidden     && !ShowMarkersOnUserHide)      &&
            !(isInCutscene   && !ShowMarkersInCutscenes)     &&
            !(isBetweenAreas && !ShowMarkersBetweenAreas)    &&
            !(isInIdleCam    && !ShowMarkersDuringIdleCam)   &&
            !(isInCombat     && !ShowMarkersInCombat)        &&
            !(isBoundByDuty  && !ShowMarkersInDuty);
    }
}
