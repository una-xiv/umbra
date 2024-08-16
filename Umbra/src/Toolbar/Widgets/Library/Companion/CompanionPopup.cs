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
using System;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed partial class CompanionPopup : WidgetPopup
{
    public bool ShowFoodButtons { get; set; }

    private IPlayer           Player      { get; } = Framework.Service<IPlayer>();
    private ICompanionManager Companion   { get; } = Framework.Service<ICompanionManager>();
    private IDataManager      DataManager { get; } = Framework.Service<IDataManager>();

    public CompanionPopup()
    {
        Node.QuerySelector("InfoButton")!.OnClick    += _ => Companion.OpenWindow();
        Node.QuerySelector("FeedButton")!.OnClick    += _ => Companion.Summon();
        Node.QuerySelector("DismissButton")!.OnClick += _ => Companion.Dismiss();

        CreateStanceButton(3);
        CreateStanceButton(4);
        CreateStanceButton(5);
        CreateStanceButton(6);
        CreateStanceButton(7);

        CreateFoodButton(CompanionFood.CurielRoot);
        CreateFoodButton(CompanionFood.SylkisBud);
        CreateFoodButton(CompanionFood.MimettGourd);
        CreateFoodButton(CompanionFood.Tantalplant);
        CreateFoodButton(CompanionFood.PahsanaFruit);
    }

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        return Companion.TimeLeft > 0;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        Node.QuerySelector("FeedButton")!.IsDisabled       = !Companion.CanSummon();
        Node.QuerySelector(".header-text-name")!.NodeValue = Companion.CompanionName;
        Node.QuerySelector(".header-time-left")!.NodeValue = Companion.TimeLeftString;

        Node infoNode = Node.QuerySelector(".header-text-info")!;

        if (Companion.Level < 20) {
            infoNode.Style.IsVisible = true;

            infoNode.NodeValue = I18N.Translate(
                "Widget.Companion.Info",
                Companion.Level,
                $"{Companion.CurrentXp / (float)Companion.RequiredXp:P0}"
            );
        } else {
            infoNode.Style.IsVisible = false;
        }

        Node.QuerySelector("#FoodButtons")!.Style.IsVisible = ShowFoodButtons;
        if (!ShowFoodButtons) return;

        foreach (CompanionFood foodType in Enum.GetValues<CompanionFood>()) {
            var node = Node.QuerySelector($"#Food_{foodType}");
            if (null == node) continue;

            int count = Player.GetItemCount((uint)foodType);

            node.QuerySelector(".button--icon")!.Style.ImageGrayscale = count == 0;
            node.QuerySelector(".button--count")!.NodeValue           = count > 0 ? $"{count}" : null;

            switch (count > 0) {
                case true when !node.ClassList.Contains("has-food"):
                    node.ClassList.Add("has-food");
                    break;
                case false when node.ClassList.Contains("has-food"):
                    node.ClassList.Remove("has-food");
                    break;
            }
        }
    }
}
