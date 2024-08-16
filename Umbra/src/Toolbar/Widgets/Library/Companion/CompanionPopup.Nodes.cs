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
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class CompanionPopup
{
    protected override Node Node { get; } = new() {
        Stylesheet = CompanionPopupStylesheet,
        ClassList  = ["popup"],
        ChildNodes = [
            new() {
                ClassList = ["header"],
                ChildNodes = [
                    new() {
                        ClassList = ["header-text"],
                        ChildNodes = [
                            new() {
                                ClassList = ["header-text-name"],
                                NodeValue = "Companion Name",
                            },
                            new() {
                                ClassList = ["header-text-info"],
                                NodeValue = "Level 15, 42% XP",
                            },
                        ],
                    },
                    new() {
                        ClassList = ["header-time-left"],
                        NodeValue = "55:32"
                    },
                ],
            },
            new() {
                Id         = "StanceButtons",
                ClassList  = ["body"],
                ChildNodes = []
            },
            new() {
                Id         = "FoodButtons",
                ClassList  = ["body"],
                ChildNodes = []
            },
            new() {
                ClassList = ["footer"],
                ChildNodes = [
                    new ButtonNode("InfoButton", I18N.Translate("Widget.Companion.InfoWindow")),
                    new ButtonNode("FeedButton", I18N.Translate("Widget.Companion.Feed")),
                    new ButtonNode("DismissButton", I18N.Translate("Widget.Companion.Withdraw")) {
                        Style = new() { Anchor = Anchor.TopRight }
                    },
                ]
            }
        ]
    };

    private void CreateStanceButton(uint actionId)
    {
        Node node = new() {
            Id        = $"Stance_{actionId}",
            ClassList = ["stance-button"],
            Tooltip   = Companion.GetStanceName(actionId),
            ChildNodes = [
                new() {
                    ClassList = ["button--icon"],
                    Style = new() {
                        IconId = Companion.GetStanceIcon(actionId),
                    }
                }
            ],
        };

        node.OnClick += _ => Companion.SetStance(actionId);

        node.BeforeDraw += n => {
            node.QuerySelector(".button--icon")!.Style.ImageGrayscale =
                !n.IsMouseOver && (!Companion.CanSetStance(actionId) || actionId != Companion.ActiveCommand);

            node.Style.Opacity = Companion.CanSetStance(actionId) ? 1 : 0.33f;
        };

        Node.QuerySelector("#StanceButtons")!.AppendChild(node);
    }

    private void CreateFoodButton(CompanionFood foodType)
    {
        Item? item = DataManager.GetExcelSheet<Item>()!.GetRow((uint)foodType);
        if (item == null) return;

        Node node = new() {
            Id        = $"Food_{foodType}",
            ClassList = ["food-button"],
            Tooltip   = $"{item.Name.ToDalamudString().TextValue}\n\n{item.Description.ToDalamudString().TextValue.Split("\n").LastOrDefault() ?? ""}",
            ChildNodes = [
                new() {
                    ClassList = ["button--icon"],
                    Style = new() { IconId = item.Icon }
                },
                new() {
                    ClassList = ["button--count"],
                }
            ],
        };

        node.OnMouseUp += _ => {
            if (Companion.HasCompanionFood(foodType)) Companion.UseCompanionFood(foodType);
        };

        Node.QuerySelector("#FoodButtons")!.AppendChild(node);
    }
}
