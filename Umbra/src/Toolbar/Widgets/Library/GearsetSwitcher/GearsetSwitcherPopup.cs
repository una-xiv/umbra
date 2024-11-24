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
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class GearsetSwitcherPopup : WidgetPopup
{
    protected override Node Node { get; } = new() {
        ChildNodes = [
            new() {
                Id = "Background",
                Style = new() {
                    Anchor                  = Anchor.TopRight,
                    Margin                  = new() { Top = 90 },
                    BackgroundGradient      = GradientColor.Vertical(new(0x80605010), new(0)),
                    BackgroundGradientInset = new(4) { Top = 0 },
                }
            }
        ],
        Style = new() {
            Flow = Flow.Vertical
        }
    };

    private readonly IDataManager       _dataManager;
    private readonly IGearsetRepository _gearsetRepository;
    private readonly IPlayer            _player;

    private Gearset? _currentGearset;
    private Gearset? _ctxSelectedGearset;

    public GearsetSwitcherPopup()
    {
        _dataManager       = Framework.Service<IDataManager>();
        _gearsetRepository = Framework.Service<IGearsetRepository>();
        _player            = Framework.Service<IPlayer>();

        ForcePopupInMainViewport = true;

        CreateHeaderNode();
        CreateColumnsNode();

        CreateRoleContainer(GearsetCategory.Tank,     I18N.Translate("Widget.GearsetSwitcher.Role.Tank"));
        CreateRoleContainer(GearsetCategory.Healer,   I18N.Translate("Widget.GearsetSwitcher.Role.Healer"));
        CreateRoleContainer(GearsetCategory.Melee,    I18N.Translate("Widget.GearsetSwitcher.Role.Melee"));
        CreateRoleContainer(GearsetCategory.Ranged,   I18N.Translate("Widget.GearsetSwitcher.Role.PhysicalRanged"));
        CreateRoleContainer(GearsetCategory.Caster,   I18N.Translate("Widget.GearsetSwitcher.Role.MagicalRanged"));
        CreateRoleContainer(GearsetCategory.Crafter,  I18N.Translate("Widget.GearsetSwitcher.Role.Crafter"));
        CreateRoleContainer(GearsetCategory.Gatherer, I18N.Translate("Widget.GearsetSwitcher.Role.Gatherer"));

        _gearsetRepository.OnGearsetCreated  += OnGearsetCreated;
        _gearsetRepository.OnGearsetRemoved  += OnGearsetRemoved;
        _gearsetRepository.OnGearsetChanged  += OnGearsetChanged;
        _gearsetRepository.OnGearsetEquipped += OnGearsetEquipped;

        _gearsetRepository.GetGearsets().ForEach(OnGearsetCreated);
        _currentGearset = _gearsetRepository.CurrentGearset;

        ContextMenu = new(
            [
                new("LinkGlam") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.LinkGlamourPlate"),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        _gearsetRepository.OpenGlamourSetLinkWindow(_ctxSelectedGearset);
                        Close();
                    }
                },
                new("UnlinkGlam") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.UnlinkGlamourPlate", ""),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        _gearsetRepository.UnlinkGlamourSet(_ctxSelectedGearset);
                    }
                },
                new("EditBanner") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.EditPortrait"),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        _gearsetRepository.OpenPortraitEditorForGearset(_ctxSelectedGearset);
                        Close();
                    }
                },
                new("MoveUp") {
                    Label  = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.MoveUp"),
                    IconId = 60541u,
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        _gearsetRepository.MoveGearsetUp(_ctxSelectedGearset);
                    }
                },
                new("MoveDown") {
                    Label  = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.MoveDown"),
                    IconId = 60545u,
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        _gearsetRepository.MoveGearsetDown(_ctxSelectedGearset);
                    }
                },
                new("Rename") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.Rename"),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;

                        unsafe {
                            var result = stackalloc AtkValue[1];
                            var values = stackalloc AtkValue[2];
                            values[0].SetInt(10);                     // case
                            values[1].SetInt(_ctxSelectedGearset.Id); // gearsetIndex
                            AgentGearSet.Instance()->ReceiveEvent(result, values, 2, 0);
                        }
                    }
                },
                new("Delete") {
                    Label  = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.Delete"),
                    IconId = 61502u,
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;

                        _gearsetRepository.DeleteGearset(_ctxSelectedGearset);
                    },
                },
            ]
        );
    }

    public bool   AutoCloseOnChange           { get; set; }
    public bool   ShowRoleNames               { get; set; }
    public bool   ShowCurrentJobGradient      { get; set; }
    public bool   ShowWarningIcon             { get; set; } = true;
    public bool   ShowExperienceBar           { get; set; } = true;
    public bool   ShowExperiencePct           { get; set; } = true;
    public bool   ShowItemLevel               { get; set; } = true;
    public string GearsetButtonBackgroundType { get; set; } = "GradientV";
    public string GearsetFilterPrefix         { get; set; } = "";
    public int    GearsetNodeWidth            { get; set; } = 150;
    public int    GearsetNodeHeight           { get; set; } = 40;

    public string HeaderIconType      { get; set; } = "Default";
    public string ButtonIconType      { get; set; } = "Default";
    public string UldThemeTarget      { get; set; } = "Light";
    public bool   EnableRoleScrolling { get; set; }

    public int HeaderIconYOffset { get; set; }
    public int ButtonIconYOffset { get; set; }

    public string TankRoleLocation     { get; set; } = "LeftColumn";
    public string HealerRoleLocation   { get; set; } = "LeftColumn";
    public string MeleeRoleLocation    { get; set; } = "LeftColumn";
    public string RangedRoleLocation   { get; set; } = "MiddleColumn";
    public string CasterRoleLocation   { get; set; } = "MiddleColumn";
    public string CrafterRoleLocation  { get; set; } = "RightColumn";
    public string GathererRoleLocation { get; set; } = "RightColumn";

    public int TankRoleSortIndex     { get; set; } = 0;
    public int HealerRoleSortIndex   { get; set; } = 1;
    public int MeleeRoleSortIndex    { get; set; } = 2;
    public int RangedRoleSortIndex   { get; set; } = 0;
    public int CasterRoleSortIndex   { get; set; } = 1;
    public int CrafterRoleSortIndex  { get; set; } = 1;
    public int GathererRoleSortIndex { get; set; } = 0;

    public int TankMaxItems     { get; set; } = 3;
    public int HealerMaxItems   { get; set; } = 3;
    public int MeleeMaxItems    { get; set; } = 4;
    public int RangedMaxItems   { get; set; } = 5;
    public int CasterMaxItems   { get; set; } = 5;
    public int CrafterMaxItems  { get; set; } = 7;
    public int GathererMaxItems { get; set; } = 3;

    public bool ShowTank     { get; set; } = true;
    public bool ShowHealer   { get; set; } = true;
    public bool ShowMelee    { get; set; } = true;
    public bool ShowRanged   { get; set; } = true;
    public bool ShowCaster   { get; set; } = true;
    public bool ShowCrafter  { get; set; } = true;
    public bool ShowGatherer { get; set; } = true;

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        return _gearsetRepository.CurrentGearset is not null;
    }

    protected override void OnOpen()
    {
        _currentGearset = _gearsetRepository.CurrentGearset;
        SetBackgroundGradientFor(_currentGearset?.Category ?? GearsetCategory.None);
        UpdateNodes();

        Node openGlamButton = Node.QuerySelector("#OpenGlam")!;

        openGlamButton.Tooltip    = _dataManager.GetExcelSheet<GeneralAction>().GetRow(25).Name.ToString();
        openGlamButton.IsDisabled = !_player.IsGeneralActionUnlocked(25);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        if (_currentGearset != _gearsetRepository.CurrentGearset) {
            _currentGearset = _gearsetRepository.CurrentGearset;

            if (_currentGearset is not null) OnGearsetChanged(_currentGearset);
        }

        if (_currentGearset is null) {
            Close();
            return;
        }

        Node.QuerySelector("#HeaderIcon")!.Style.IconId =
            _player.GetJobInfo(_currentGearset.JobId).GetIcon(HeaderIconType);

        Node.QuerySelector("#HeaderIcon")!.Style.ImageOffset = new(0, HeaderIconYOffset);
        Node.QuerySelector("#OpenGlam")!.IsDisabled          = !_player.IsInSanctuary;

        Node.QuerySelector("#HeaderGearsetName")!.Style.Size = new(GearsetNodeWidth, 0);
        Node.QuerySelector("#HeaderGearsetInfo")!.Style.Size = new(GearsetNodeWidth, 0);


        // Assign role containers to the configured columns.
        foreach ((GearsetCategory category, Node node) in RoleContainers) {
            node.SortIndex = GetSortIndexForRole(category);
            var target = GetColumnForRole(category);

            node.Style.Size = new(GearsetNodeWidth, 0);
            node.QuerySelector("#RoleHeader")!.Style.Size = new(GearsetNodeWidth, GearsetNodeHeight);
            node.QuerySelector("#RoleBody")!.Style.Size   = new(GearsetNodeWidth + 8, 0);

            if (node.ParentNode != target) {
                target.AppendChild(node);
            }

            Node listNode = GetGearsetListNodeFor(category);
            int  setCount = listNode.ChildNodes.Count;

            listNode.ParentNode!.Overflow = !EnableRoleScrolling;

            if (EnableRoleScrolling) {
                int maxItems  = GetMaxItemsToDisplayForRole(category);
                int gapHeight = listNode.ComputedStyle.Gap;
                int setItems  = setCount < maxItems ? setCount : maxItems;
                int height    = (setItems * GearsetNodeHeight) + ((setItems - 1) * gapHeight);
                listNode.Style.Size = new(GearsetNodeWidth, height);
            } else {
                listNode.Style.Size = new(GearsetNodeWidth, 0);
            }

            node.Style.IsVisible                               = setCount > 0 && GetVisibilityForRole(category);
            node.QuerySelector("#RoleHeader")!.Style.IsVisible = ShowRoleNames;
        }

        UpdateNodes();

        // Find back the enum value or fallback to initial old value
        if (!Enum.TryParse(UldThemeTarget, out UldStyle uldStyle)) {
            uldStyle = UldStyle.Light;
        }

        Node.QuerySelector("BestInSlot")!.Style.UldStyle = uldStyle;
        Node.QuerySelector("OpenGlam")!.Style.UldStyle   = uldStyle;
        Node.QuerySelector("Update")!.Style.UldStyle     = uldStyle;
        Node.QuerySelector("Duplicate")!.Style.UldStyle  = uldStyle;
    }

    private void UpdateNodes()
    {
        List<Gearset> toRemove = [];

        foreach (Gearset gearset in _gearsetRepository.GetGearsets()) {
            if (ShouldRenderGearset(gearset) && !NodeByGearset.ContainsKey(gearset)) {
                OnGearsetCreated(gearset);
            }
        }

        foreach ((Gearset gearset, GearsetNode node) in NodeByGearset) {
            if (!ShouldRenderGearset(gearset)) {
                toRemove.Add(gearset);
                continue;
            }

            node.NodeWidth         = GearsetNodeWidth;
            node.NodeHeight        = GearsetNodeHeight;
            node.ButtonIconType    = ButtonIconType;
            node.ButtonIconYOffset = ButtonIconYOffset;
            node.BackgroundType    = GearsetButtonBackgroundType;
            node.ShowItemLevel     = ShowItemLevel;
            node.ShowWarningIcon   = ShowWarningIcon;
            node.ShowExperienceBar = ShowExperienceBar;
            node.ShowExperiencePct = ShowExperiencePct;

            node.Update();
        }

        foreach (Gearset gearset in toRemove) {
            OnGearsetRemoved(gearset);
        }
    }

    /// <inheritdoc/>
    protected override void OnDisposed()
    {
        _gearsetRepository.OnGearsetCreated  -= OnGearsetCreated;
        _gearsetRepository.OnGearsetRemoved  -= OnGearsetRemoved;
        _gearsetRepository.OnGearsetChanged  -= OnGearsetChanged;
        _gearsetRepository.OnGearsetEquipped -= OnGearsetEquipped;

        GearsetsByCategory.Clear();
        NodeByGearset.Clear();
        RoleContainers.Clear();
    }

    private void OnGearsetEquipped(Gearset _)
    {
        if (AutoCloseOnChange) Close();
    }

    private void SetBackgroundGradientFor(GearsetCategory category)
    {
        Node bg = Node.QuerySelector("#Background")!;
        Node hg = Node.QuerySelector("#Header")!;

        if (!ShowCurrentJobGradient) {
            hg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
            bg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
            return;
        }

        switch (category) {
            case GearsetCategory.Tank:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),           new("Role.Tank"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.Tank"), new(0));
                break;
            case GearsetCategory.Healer:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),             new("Role.Healer"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.Healer"), new(0));
                break;
            case GearsetCategory.Melee:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),               new("Role.MeleeDps"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.MeleeDps"), new(0));
                break;
            case GearsetCategory.Ranged:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new("Role.PhysicalRangedDps"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.PhysicalRangedDps"), new(0));
                break;
            case GearsetCategory.Caster:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new("Role.MagicalRangedDps"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.MagicalRangedDps"), new(0));
                break;
            case GearsetCategory.Crafter:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),              new("Role.Crafter"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.Crafter"), new(0));
                break;
            case GearsetCategory.Gatherer:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),               new("Role.Gatherer"));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new("Role.Gatherer"), new(0));
                break;
            default:
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
                break;
        }
    }
}
