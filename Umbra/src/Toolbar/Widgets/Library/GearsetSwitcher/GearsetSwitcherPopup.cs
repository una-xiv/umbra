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

using System;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class GearsetSwitcherPopup : WidgetPopup, IDisposable
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

    private readonly IGearsetRepository _gearsetRepository;

    private Gearset? _currentGearset;

    public GearsetSwitcherPopup()
    {
        _gearsetRepository = Framework.Service<IGearsetRepository>();

        CreateHeaderNode();
        CreateColumnsNode();

        CreateRoleContainer(GearsetCategory.Tank,     I18N.Translate("Widget.GearsetSwitcher.Role.Tank"));
        CreateRoleContainer(GearsetCategory.Healer,   I18N.Translate("Widget.GearsetSwitcher.Role.Healer"));
        CreateRoleContainer(GearsetCategory.Melee,    I18N.Translate("Widget.GearsetSwitcher.Role.Melee"));
        CreateRoleContainer(GearsetCategory.Ranged,   I18N.Translate("Widget.GearsetSwitcher.Role.PhysicalRanged"));
        CreateRoleContainer(GearsetCategory.Caster,   I18N.Translate("Widget.GearsetSwitcher.Role.MagicalRanged"));
        CreateRoleContainer(GearsetCategory.Crafter,  I18N.Translate("Widget.GearsetSwitcher.Role.Crafter"));
        CreateRoleContainer(GearsetCategory.Gatherer, I18N.Translate("Widget.GearsetSwitcher.Role.Gatherer"));

        _gearsetRepository.OnGearsetCreated += OnGearsetCreated;
        _gearsetRepository.OnGearsetRemoved += OnGearsetRemoved;
        _gearsetRepository.OnGearsetChanged += OnGearsetChanged;

        _gearsetRepository.GetGearsets().ForEach(OnGearsetCreated);
        _currentGearset = _gearsetRepository.CurrentGearset;
    }

    public bool AutoCloseOnChange { get; set; }
    public bool ShowRoleNames { get; set; }
    public bool ShowCurrentJobGradient { get; set; }
    public bool ShowGearsetGradient { get; set; }

    public bool UseAlternateHeaderIcon { get; set; }
    public bool UseAlternateButtonIcon { get; set; }

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
        SetBackgroundGradientFor(_currentGearset?.Category ?? GearsetCategory.None);
        UpdateNodes();
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
            _currentGearset.JobId + 62000u + (UseAlternateHeaderIcon ? 100u : 0u);

        Node.QuerySelector("#HeaderIcon")!.Style.ImageOffset = new(0, HeaderIconYOffset);

        // Assign role containers to the configured columns.
        foreach ((GearsetCategory category, Node node) in RoleContainers) {
            node.SortIndex = GetSortIndexForRole(category);
            var target = GetColumnForRole(category);

            if (node.ParentNode != target) {
                target.AppendChild(node);
            }

            Node listNode  = GetGearsetListNodeFor(category);
            int  maxItems  = GetMaxItemsToDisplayForRole(category);
            int  gapHeight = listNode.ComputedStyle.Gap;
            int  setCount  = listNode.ChildNodes.Count;
            int  setItems  = setCount < maxItems ? setCount : maxItems;
            int  height    = (setItems * GearsetNode.NodeHeight) + ((setItems - 1) * gapHeight);

            listNode.Style.Size  = new(GearsetNode.NodeWidth, height);
            node.Style.IsVisible = setCount > 0 && GetVisibilityForRole(category);

            node.QuerySelector("#RoleHeader")!.Style.IsVisible = ShowRoleNames;
        }

        UpdateNodes();
    }

    private void UpdateNodes()
    {
        foreach (GearsetNode node in NodeByGearset.Values) {
            node.UseAlternateButtonIcon = UseAlternateButtonIcon;
            node.ButtonIconYOffset      = ButtonIconYOffset;
            node.ShowGearsetGradient    = ShowGearsetGradient;
            node.Update();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _gearsetRepository.OnGearsetCreated -= OnGearsetCreated;
        _gearsetRepository.OnGearsetRemoved -= OnGearsetRemoved;
        _gearsetRepository.OnGearsetChanged -= OnGearsetChanged;

        GearsetsByCategory.Clear();
        NodeByGearset.Clear();
        RoleContainers.Clear();
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
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),          new(0x90a54a3b));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0xA0a54a3b), new(0));
                break;
            case GearsetCategory.Healer:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),          new(0x902e613b));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0xA02e613b), new(0));
                break;
            case GearsetCategory.Melee:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),          new(0x902e3069));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0xA02e3069), new(0));
                break;
            case GearsetCategory.Ranged:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),          new(0x902c89a6));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0xA02c89a6), new(0));
                break;
            case GearsetCategory.Caster:
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0),          new(0x90a72a5a));
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0xA0a72a5a), new(0));
                break;
            default:
                bg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
                hg.Style.BackgroundGradient = GradientColor.Vertical(new(0), new(0));
                break;
        }
    }
}
