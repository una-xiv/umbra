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
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Game;

namespace Umbra;

[Service]
internal sealed partial class GearsetWidget : IToolbarWidget, IDisposable
{
    [ConfigVariable(
        "Toolbar.Widget.Gearset.Enabled",
        "Toolbar Widgets",
        "Show gearset switcher",
        "Display a widget that allows you to switch between gearsets."
    )]
    private static bool Enabled { get; set; } = true;

    [ConfigVariable("Toolbar.Height")] private static int Height { get; set; } = 32;

    private readonly GearsetRepository            _repository;
    private readonly ToolbarDropdownContext       _dropdownContext;
    private readonly Dictionary<Gearset, Element> _gearsetElements = [];

    public GearsetWidget(GearsetRepository repository, ToolbarDropdownContext dropdownContext)
    {
        _repository      = repository;
        _dropdownContext = dropdownContext;

        dropdownContext.RegisterDropdownActivator(Element, DropdownElement);

        DropdownElement.AddNode(
            new RectNode(
                id: "CategoryGradient",
                color: 0x00000000,
                margin: new(left: 2, right: 2, top: 50),
                overflow: true,
                gradients: new(
                    topLeft: 0x803247AC,
                    topRight: 0x803247AC
                )
            )
        );

        repository.GetGearsets().ForEach(OnGearsetCreated);
        repository.OnGearsetCreated += OnGearsetCreated;
        repository.OnGearsetChanged += OnGearsetChanged;
        repository.OnGearsetRemoved += OnGearsetRemoved;

        var buttons = DropdownElement.Content.Get("Header.Body.Info.Buttons");
        buttons.Get("MoveUp").OnClick      += () => repository.MoveEquippedGearsetUp();
        buttons.Get("MoveDown").OnClick    += () => repository.MoveEquippedGearsetDown();
        buttons.Get("Update").OnClick      += () => repository.UpdateEquippedGearset();
        buttons.Get("Duplicate").OnClick   += () => repository.DuplicateEquippedGearset();
        buttons.Get("Delete").OnRightClick += () => repository.DeleteEquippedGearset();
    }

    public void Dispose()
    {
        _repository.OnGearsetCreated -= OnGearsetCreated;
        _repository.OnGearsetChanged -= OnGearsetChanged;
        _repository.OnGearsetRemoved -= OnGearsetRemoved;

        ClearGearsetElements();
    }

    [OnDraw]
    public void OnDraw()
    {
        if (!Enabled) {
            Element.IsVisible = false;
            return;
        }

        var hasGearsetEquipped = _repository.CurrentGearset != null;

        Element.IsVisible = hasGearsetEquipped;

        if (!hasGearsetEquipped) {
            if (DropdownElement.IsVisible) _dropdownContext.Clear();
            return;
        }

        // Debug.
        // DropdownContext.Activate(DropdownElement, Element);

        UpdateToolbarButton();
        UpdateDropdownHeader();
        UpdateColumnVisibility();
        // Repository.GetGearsets().ForEach(OnGearsetChanged);
    }

    private void UpdateToolbarButton()
    {
        Element.Size = new(0, Height - 6);

        Element.Get("Icon").Size                          = new(Height - 6, Height - 6);
        Element.Get("Icon").GetNode<IconNode>().IconId    = _repository.CurrentGearset!.JobId + 62100u;
        Element.Get("Info.Name").GetNode<TextNode>().Text = _repository.CurrentGearset!.Name;
        Element.Get("Info.Sub").GetNode<TextNode>().Text  = $"Ilvl. {_repository.CurrentGearset!.ItemLevel}";

        Element.Get("Info.Name").Size                      = new Size(0, Height >= 31 ? Height / 2 - 3 : Height - 9);
        Element.Get("Info.Name").GetNode<TextNode>().Align = Height >= 31 ? Align.BottomRight : Align.MiddleRight;
        Element.Get("Info.Name").GetNode<TextNode>().Font  = Height >= 40 ? Font.Axis : Font.AxisSmall;

        Element.Get("Info.Sub").IsVisible                = Height >= 31;
        Element.Get("Info.Sub").Size                     = new Size(0, Height / 2 - 3);
        Element.Get("Info.Sub").GetNode<TextNode>().Font = Height >= 40 ? Font.AxisSmall : Font.AxisExtraSmall;
    }

    private void UpdateDropdownHeader()
    {
        var gearset = _repository.CurrentGearset!;
        var color   = GearsetCategoryRepository.GetCategoryColor(gearset.Category).ApplyAlphaComponent(0.35f);
        var header  = DropdownElement.Content.Get("Header");

        // Update gradient background.
        header.Get("Body").GetNode<RectNode>().Gradients = new(
            bottomLeft: color.ApplyAlphaComponent(0.8f),
            bottomRight: color.ApplyAlphaComponent(0.8f)
        );

        DropdownElement.GetNode<RectNode>("CategoryGradient").Gradients = new(topLeft: color, topRight: color);

        // Update icon.
        header.Get("Body.Icon").GetNode<IconNode>().IconId    = gearset.JobId + 62100u;
        header.Get("Body.Info.Name").GetNode<TextNode>().Text = gearset.Name;

        header.Get("Body.Info.Job").GetNode<TextNode>().Text =
            $"Level {gearset.JobLevel} {gearset.JobName}{(gearset.JobXp > 0 ? $" - {gearset.JobXp}% XP" : "")}";

        header.Get("Body.ItemLevel").GetNode<TextNode>().Text = $"{gearset.ItemLevel}";

        // Update buttons.
        header.Get("Body.Info.Buttons.Delete").IsDisabled   = _repository.GetGearsets().Count           <= 1;
        header.Get("Body.Info.Buttons.MoveUp").IsDisabled   = _repository.FindPrevIdInCategory(gearset) == null;
        header.Get("Body.Info.Buttons.MoveDown").IsDisabled = _repository.FindNextIdInCategory(gearset) == null;
    }

    private static void UpdateColumnVisibility()
    {
        var left   = DropdownElement.Content.Get("Columns.Left");
        var center = DropdownElement.Content.Get("Columns.Center");
        var right  = DropdownElement.Content.Get("Columns.Right");

        var tanks     = left.Get($"Category{GearsetCategory.Tank}");
        var healers   = left.Get($"Category{GearsetCategory.Healer}");
        var melees    = left.Get($"Category{GearsetCategory.Melee}");
        var ranged    = center.Get($"Category{GearsetCategory.Ranged}");
        var casters   = center.Get($"Category{GearsetCategory.Caster}");
        var crafters  = right.Get($"Category{GearsetCategory.Crafter}");
        var gatherers = right.Get($"Category{GearsetCategory.Gatherer}");

        tanks.IsVisible     = tanks.Get("Gearsets").Children.Count     > 0;
        healers.IsVisible   = healers.Get("Gearsets").Children.Count   > 0;
        melees.IsVisible    = melees.Get("Gearsets").Children.Count    > 0;
        ranged.IsVisible    = ranged.Get("Gearsets").Children.Count    > 0;
        casters.IsVisible   = casters.Get("Gearsets").Children.Count   > 0;
        crafters.IsVisible  = crafters.Get("Gearsets").Children.Count  > 0;
        gatherers.IsVisible = gatherers.Get("Gearsets").Children.Count > 0;

        left.IsVisible   = tanks.IsVisible    || healers.IsVisible || melees.IsVisible;
        center.IsVisible = ranged.IsVisible   || casters.IsVisible;
        right.IsVisible  = crafters.IsVisible || gatherers.IsVisible;
    }

    private void OnGearsetCreated(Gearset gearset)
    {
        var el = CreateGearsetElement(gearset);
        GetGearsetContainerElement(gearset).AddChild(el);
        _gearsetElements[gearset] = el;
    }

    private void OnGearsetChanged(Gearset gearset)
    {
        if (false == IsGearsetInCorrectContainer(gearset)) {
            OnGearsetRemoved(gearset);
            OnGearsetCreated(gearset);
            return;
        }

        var el = GetGearsetContainerElement(gearset).Get($"Gearset{gearset.Id}");

        el.GetNode<RectNode>("CategoryColor").Color   = GearsetCategoryRepository.GetCategoryColor(gearset.Category);
        el.GetNode<RectNode>("CategoryColor").Opacity = gearset.IsCurrent ? 0.45f : 0.15f;
        el.Get("Icon").GetNode<IconNode>().IconId     = gearset.JobId + 62000u;
        el.Get("Info.Name").GetNode<TextNode>().Text  = gearset.Name;
        el.Get("Info.Name").GetNode<TextNode>().Color = gearset.IsCurrent ? 0xFFFFFFFF : 0xFFC0C0C0;
        el.Get("Info.Sub").GetNode<TextNode>().Text   = $"Level {gearset.JobLevel} {gearset.JobName}";
        el.Get("ItemLevel").GetNode<TextNode>().Text  = $"{gearset.ItemLevel}";
    }

    private void OnGearsetRemoved(Gearset gearset)
    {
        Element el = _gearsetElements[gearset];

        el.Parent!.RemoveChild(el);
        _gearsetElements.Remove(gearset);
    }

    private static Element GetGearsetContainerElement(Gearset gearset)
    {
        return DropdownElement.Content.Get($"Columns.{GetColumnIdOf(gearset)}.Category{gearset.Category}.Gearsets");
    }

    private static bool IsGearsetInCorrectContainer(Gearset gearset)
    {
        return DropdownElement.Content.Has(
            $"Columns.{GetColumnIdOf(gearset)}.Category{gearset.Category}.Gearsets.Gearset{gearset.Id}"
        );
    }

    private static string GetColumnIdOf(Gearset gearset)
    {
        return gearset.Category switch {
            GearsetCategory.Tank     => "Left",
            GearsetCategory.Healer   => "Left",
            GearsetCategory.Melee    => "Left",
            GearsetCategory.Ranged   => "Center",
            GearsetCategory.Caster   => "Center",
            GearsetCategory.Crafter  => "Right",
            GearsetCategory.Gatherer => "Right",
            _                        => "None"
        };
    }

    private void ClearGearsetElements()
    {
        _gearsetElements.Values.ToList().ForEach(e => e.Parent?.RemoveChild(e));
        _gearsetElements.Clear();
    }
}
