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
using Dalamud.Interface;
using Una.Drawing;

namespace Umbra.Windows.Components;

internal class CheckboxNode : Node
{
    public event Action<bool>? OnValueChanged;

    public bool Value {
        get => _value;
        set {
            if (_value == value) return;
            _value                               = value;
            QuerySelector("Checkbox")!.NodeValue = value ? "☑" : "☐";
            OnValueChanged?.Invoke(value);
        }
    }

    public string Label {
        get => (string)(LabelNode.NodeValue ?? "");
        set => LabelNode.NodeValue = value;
    }

    public string? Description {
        get => (string?)DescriptionNode.NodeValue;
        set => DescriptionNode.NodeValue = value;
    }

    private bool _value;

    public CheckboxNode(string id, bool value, string label, string? description = null)
    {
        _value = value;

        Id         = id;
        ClassList  = ["checkbox"];
        Stylesheet = CheckboxStylesheet;

        ChildNodes = [
            new() {
                Id          = "Checkbox",
                ClassList   = ["checkbox--box"],
                InheritTags = true,
            },
            new() {
                Id          = "Text",
                ClassList   = ["checkbox--text"],
                InheritTags = true,
                ChildNodes = [
                    new() {
                        Id          = "Label",
                        ClassList   = ["checkbox--text--label"],
                        InheritTags = true,
                        NodeValue   = label,
                    },
                    new() {
                        Id          = "Description",
                        ClassList   = ["checkbox--text--description"],
                        InheritTags = true,
                        NodeValue   = description,
                    },
                ],
            },
        ];

        Label       = label;
        Description = description;

        OnClick += _ => { Value = !Value; };

        BeforeReflow += _ => {
            int maxWidth = ParentNode!.Bounds.ContentSize.Width - ParentNode!.ComputedStyle.Padding.HorizontalSize;
            int padding  = ComputedStyle.Gap + BoxNode.OuterWidth;
            int width    = (int)((maxWidth - padding) / ScaleFactor);
            int labelHeight;

            BoxNode.NodeValue = Value ? FontAwesomeIcon.Check.ToIconString() : null;

            if (LabelNode.Style.Size?.Width == width && DescriptionNode.Style.Size?.Width == width) {
                return false;
            }

            if (string.IsNullOrEmpty((string?)DescriptionNode.NodeValue)) {
                DescriptionNode.Style.IsVisible = false;
                labelHeight                     = 24;
            } else {
                DescriptionNode.Style.IsVisible = true;
                labelHeight                     = 0;
            }

            LabelNode.Style.Size       = new(width, labelHeight);
            DescriptionNode.Style.Size = new(width, 0);

            return true;
        };
    }

    private Node BoxNode         => QuerySelector(".checkbox--box")!;
    private Node LabelNode       => QuerySelector(".checkbox--text--label")!;
    private Node DescriptionNode => QuerySelector(".checkbox--text--description")!;

    private static Stylesheet CheckboxStylesheet { get; } = new(
        [
            new(
                ".checkbox",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".checkbox--box",
                new() {
                    Flow            = Flow.Horizontal,
                    Anchor          = Anchor.TopLeft,
                    Size            = new(24, 24),
                    BorderRadius    = 6,
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    Font            = 2,
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    Color           = new("Input.Text"),
                    TextAlign       = Anchor.MiddleCenter,
                    TextOffset      = new(0, 0),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".checkbox--box:hover",
                new() {
                    BackgroundColor = new("Input.BackgroundHover"),
                    StrokeColor     = new("Input.BorderHover"),
                    Color           = new("Input.TextHover"),
                }
            ),
            new(
                ".checkbox--text",
                new() {
                    Flow   = Flow.Vertical,
                    Anchor = Anchor.TopLeft,
                    Gap    = 4,
                }
            ),
            new(
                ".checkbox--text--label",
                new() {
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOverflow = false,
                    FontSize     = 14,
                    Color        = new("Input.Text"),
                    WordWrap     = false,
                }
            ),
            new(
                ".checkbox--text--label:hover",
                new() {
                    Color = new("Input.TextHover"),
                }
            ),
            new(
                ".checkbox--text--description",
                new() {
                    Anchor          = Anchor.TopLeft,
                    FontSize        = 11,
                    Color           = new("Input.TextMuted"),
                    TextOverflow    = false,
                    WordWrap        = true,
                    LineHeight      = 1.5f,
                }
            ),
        ]
    );
}
