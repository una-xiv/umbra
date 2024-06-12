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

using Dalamud.Interface;
using Una.Drawing;

namespace Umbra.Windows.Components;

internal class ButtonNode : Node
{
    public string? Label {
        set => QuerySelector("Label")!.NodeValue = value;
    }

    public FontAwesomeIcon? Icon {
        set => QuerySelector("Icon")!.NodeValue = value?.ToIconString();
    }

    public bool IsGhost { get; set; }

    public ButtonNode(
        string id, string? label, FontAwesomeIcon? icon = null, bool isGhost = false, bool isSmall = false
    )
    {
        Id         = id;
        IsGhost    = isGhost;
        ClassList  = ["button"];
        Stylesheet = ButtonStylesheet;

        if (isSmall) TagsList.Add("small");

        ChildNodes = [
            new() { Id = "Icon", ClassList  = ["button--icon"], InheritTags  = true },
            new() { Id = "Label", ClassList = ["button--label"], InheritTags = true },
        ];

        Label = label;
        Icon  = icon;

        BeforeReflow += _ => {
            switch (IsGhost) {
                case true when !ClassList.Contains("ghost"):
                    ClassList.Add("ghost");
                    break;
                case false when ClassList.Contains("ghost"):
                    ClassList.Remove("ghost");
                    break;
            }

            QuerySelector("Icon")!.Style.IsVisible  = QuerySelector("Icon")!.NodeValue is not null;
            QuerySelector("Label")!.Style.IsVisible = QuerySelector("Label")!.NodeValue is not null;

            if (IsDisabled) {
                QuerySelector("Label")!.TagsList.Add("disabled");
                QuerySelector("Icon")!.TagsList.Add("disabled");
            } else {
                QuerySelector("Label")!.TagsList.Remove("disabled");
                QuerySelector("Icon")!.TagsList.Remove("disabled");
            }

            return true;
        };
    }

    private static Stylesheet ButtonStylesheet { get; } = new(
        [
            new(
                ".button",
                new() {
                    Size            = new(0, 28),
                    Padding         = new(0, 8),
                    BorderRadius    = 5,
                    StrokeInset     = 1,
                    StrokeWidth     = 1,
                    Gap             = 6,
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".button:small",
                new() {
                    Size    = new(0, 20),
                    Padding = new(0, 5),
                }
            ),
            new(
                ".button:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.BorderHover"),
                }
            ),
            new(
                ".button:disabled",
                new() {
                    Color           = new("Input.TextDisabled"),
                    OutlineColor    = new("Input.TextOutlineDisabled"),
                    BackgroundColor = new("Input.BackgroundDisabled"),
                    StrokeColor     = new("Input.BorderDisabled"),
                }
            ),
            new(
                ".button.ghost",
                new() {
                    BackgroundColor = new(0),
                    StrokeColor     = new(0),
                }
            ),
            new(
                ".button--icon",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleCenter,
                    FontSize     = 13,
                    Font         = 2,
                    Size         = new(0, 28),
                    Color        = new("Input.Text"),
                    OutlineColor = new("Input.TextOutline"),
                }
            ),
            new(
                ".button--label",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    TextAlign    = Anchor.MiddleCenter,
                    Size         = new(0, 28),
                    FontSize     = 13,
                    OutlineSize  = 1,
                    Color        = new("Input.Text"),
                    OutlineColor = new("Input.TextOutline"),
                }
            ),
            new(
                ".button--label:small",
                new() {
                    FontSize = 11,
                }
            ),
            new(
                ".button--icon:hover",
                new() {
                    Color        = new("Input.TextHover"),
                    OutlineColor = new("Input.TextOutlineHover"),
                }
            ),
            new(
                ".button--label:hover",
                new() {
                    Color        = new("Input.TextHover"),
                    OutlineColor = new("Input.TextOutlineHover"),
                }
            ),
            new(
                ".button--icon:disabled",
                new() {
                    Color        = new("Input.TextDisabled"),
                    OutlineColor = new("Input.TextOutlineDisabled"),
                }
            ),
            new(
                ".button--label:disabled",
                new() {
                    Color        = new("Input.TextDisabled"),
                    OutlineColor = new("Input.TextOutlineDisabled"),
                }
            )
        ]
    );
}
