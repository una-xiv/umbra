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
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.Gearset;

[Service]
internal partial class GearsetWidget : IToolbarWidget
{
    [ConfigVariable("Toolbar.Widget.Gearset.Enabled", "EnabledWidgets")]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.Gearset.UseAlternateIcons.Header", "ToolbarSettings", "GearsetSwitcherSettings")]
    private static bool UseAlternateHeaderIcon { get; set; } = true;

    [ConfigVariable("Toolbar.Widget.Gearset.UseAlternateIcons.Buttons", "ToolbarSettings", "GearsetSwitcherSettings")]
    private static bool UseAlternateButtonIcons { get; set; } = false;

    private readonly IGearsetRepository                   _gearsetRepository;
    private readonly IGearsetCategoryRepository           _categoryRepository;
    private readonly Dictionary<ushort, Element>          _gearsetElements = [];
    private readonly Dictionary<GearsetCategory, Element> _gearsetGroups   = [];

    public GearsetWidget(
        IGearsetRepository  gearsetRepository, IGearsetCategoryRepository categoryRepository,
        ToolbarPopupContext popupContext
    )
    {
        _gearsetRepository  = gearsetRepository;
        _categoryRepository = categoryRepository;

        popupContext.RegisterDropdownActivator(Element, _dropdownElement);

        Element.OnMouseEnter += OnWidgetMouseEnter;
        Element.OnMouseLeave += OnWidgetMouseLeave;

        _gearsetGroups[GearsetCategory.Tank]     = _dropdownElement.FindFirst<Element>("Tank")!;
        _gearsetGroups[GearsetCategory.Healer]   = _dropdownElement.FindFirst<Element>("Healer")!;
        _gearsetGroups[GearsetCategory.Melee]    = _dropdownElement.FindFirst<Element>("Melee")!;
        _gearsetGroups[GearsetCategory.Ranged]   = _dropdownElement.FindFirst<Element>("Ranged")!;
        _gearsetGroups[GearsetCategory.Caster]   = _dropdownElement.FindFirst<Element>("Caster")!;
        _gearsetGroups[GearsetCategory.Crafter]  = _dropdownElement.FindFirst<Element>("Crafter")!;
        _gearsetGroups[GearsetCategory.Gatherer] = _dropdownElement.FindFirst<Element>("Gatherer")!;

        gearsetRepository.OnGearsetCreated += OnGearsetCreated;
        gearsetRepository.OnGearsetChanged += OnGearsetUpdated;
        gearsetRepository.OnGearsetRemoved += OnGearsetDeleted;

        _dropdownElement.Get("Header.Info.Buttons.Update").OnClick      += _gearsetRepository.UpdateEquippedGearset;
        _dropdownElement.Get("Header.Info.Buttons.Duplicate").OnClick   += _gearsetRepository.DuplicateEquippedGearset;
        _dropdownElement.Get("Header.Info.Buttons.MoveUp").OnClick      += _gearsetRepository.MoveEquippedGearsetUp;
        _dropdownElement.Get("Header.Info.Buttons.MoveDown").OnClick    += _gearsetRepository.MoveEquippedGearsetDown;
        _dropdownElement.Get("Header.Info.Buttons.Delete").OnRightClick += _gearsetRepository.DeleteEquippedGearset;
    }

    public void OnDraw()
    {
        if (!Enabled || null == _gearsetRepository.CurrentGearset) {
            Element.IsVisible = false;
            return;
        }

        Element.IsVisible = true;
        UpdateWidget();

        if (_dropdownElement.IsVisible) {
            UpdateDropdown();
        }
    }

    public void OnUpdate() { }

    private void OnWidgetMouseEnter()
    {
        Element.Get<BorderElement>().Color     = 0xFF6A6A6A;
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
    }

    private void OnWidgetMouseLeave()
    {
        Element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        Element.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
    }

    private void UpdateWidget()
    {
        if (null == _gearsetRepository.CurrentGearset) return;

        Game.Gearset gearset = _gearsetRepository.CurrentGearset;

        Element.Get("Container.Icon").Style.Image = gearset.JobId + (UseAlternateButtonIcons ? 62100u : 62000u);
        Element.Get("Container.Text.Name").Text   = gearset.Name;

        Element.Get("Container.Text.Status").Text =
            $"{(gearset.IsMaxLevel ? $"Ilv.{gearset.ItemLevel}" : $"Lv.{gearset.JobLevel}, {gearset.JobXp}% XP")}";
    }

    private void UpdateDropdown()
    {
        if (null  == _gearsetRepository.CurrentGearset) return;
        if (false == _dropdownElement.IsVisible) return;

        int tankCount     = TankGroup.Get("List").Children.Count();
        int healerCount   = HealerGroup.Get("List").Children.Count();
        int meleeCount    = MeleeGroup.Get("List").Children.Count();
        int rangedCount   = RangedGroup.Get("List").Children.Count();
        int casterCount   = CasterGroup.Get("List").Children.Count();
        int crafterCount  = CrafterGroup.Get("List").Children.Count();
        int gathererCount = GathererGroup.Get("List").Children.Count();

        TankGroup.IsVisible     = tankCount     > 0;
        HealerGroup.IsVisible   = healerCount   > 0;
        MeleeGroup.IsVisible    = meleeCount    > 0;
        RangedGroup.IsVisible   = rangedCount   > 0;
        CasterGroup.IsVisible   = casterCount   > 0;
        CrafterGroup.IsVisible  = crafterCount  > 0;
        GathererGroup.IsVisible = gathererCount > 0;

        LeftColumn.IsVisible   = tankCount    > 0 || healerCount   > 0 || meleeCount > 0;
        MiddleColumn.IsVisible = rangedCount  > 0 || casterCount   > 0;
        RightColumn.IsVisible  = crafterCount > 0 || gathererCount > 0;

        Game.Gearset gearset = _gearsetRepository.CurrentGearset;
        uint         color   = _categoryRepository.GetCategoryColor(gearset.Category);

        _dropdownElement.Get("Columns").Get<GradientElement>().Gradient =
            Gradient.Vertical(color.ApplyAlphaComponent(0.25f), 0);

        _dropdownElement.Get("Header").Get<GradientElement>().Gradient =
            Gradient.Vertical(0, color.ApplyAlphaComponent(0.20f));

        _dropdownElement.Get("Header.Icon.Image").Style.Image = gearset.JobId + (UseAlternateHeaderIcon ? 62100u : 62000u);
        _dropdownElement.Get("Header.Info.Name").Text         = gearset.Name;
        _dropdownElement.Get("Header.Info.Job").Text          = $"Lv.{gearset.JobLevel} {gearset.JobName}";
        _dropdownElement.Get("Header.ItemLevel").Text         = gearset.ItemLevel.ToString();

        _dropdownElement.Get("Header.Info.Buttons.MoveUp").IsDisabled =
            _gearsetRepository.FindPrevIdInCategory(gearset) == null;

        _dropdownElement.Get("Header.Info.Buttons.MoveDown").IsDisabled =
            _gearsetRepository.FindNextIdInCategory(gearset) == null;

        // This is stupid, but necessary due to a race-condition when new gearsets are created...
        _gearsetRepository.GetGearsets().ForEach(OnGearsetUpdated);
    }

    private void OnGearsetCreated(Game.Gearset gearset)
    {
        Element group   = _gearsetGroups[gearset.Category];
        Element element = BuildGearset(gearset);

        group.Get("List").AddChild(element);
        _gearsetElements[gearset.Id] = element;
    }

    private void OnGearsetUpdated(Game.Gearset gearset)
    {
        if (!_gearsetElements.TryGetValue(gearset.Id, out Element? element)) return;

        uint gsCol = _categoryRepository.GetCategoryColor(gearset.Category);

        element.SortIndex                                  = gearset.Id;
        element.Get("Icon").Get<BackgroundElement>().Color = gsCol;
        element.Get("Icon.Image").Style.Image              = gearset.JobId + (UseAlternateButtonIcons ? 62100u : 62000u);
        element.Get("Info.Name").Text                      = gearset.Name;
        element.Get("ItemLevel").Text                      = gearset.ItemLevel.ToString();

        if (gearset.Id == _gearsetRepository.CurrentGearset?.Id) {
            element.Get<BackgroundElement>().Color = gsCol.ApplyAlphaComponent(0.25f);
            element.Get<BorderElement>().Color     = gsCol.ApplyBrightness(1.25f);
        } else {
            element.Get<BackgroundElement>().Color = 0x10C0C0C0;
            element.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
        }

        if (gearset.IsMaxLevel) {
            element.Get("XpBarWrapper").IsVisible = false;
            element.Get("Info.Job").Text          = $"Level {gearset.JobLevel} {gearset.JobName}";
        } else {
            element.Get("XpBarWrapper").IsVisible = true;
            element.Get("Info.Job").Text = $"Lv.{gearset.JobLevel}";
            element.Get("XpBarWrapper.XpBar.Bar").Style.BackgroundColor = gsCol.ApplyBrightness(1.25f);
            element.Get("XpBarWrapper.XpBar.Bar").Size = new((int)((CellWidth - 80) * (gearset.JobXp / 100f)), 4);
        }
    }

    private void OnGearsetDeleted(Game.Gearset gearset)
    {
        if (!_gearsetElements.TryGetValue(gearset.Id, out Element? element)) return;

        element.Parent!.RemoveChild(element);
        _gearsetElements.Remove(gearset.Id);
    }
}
