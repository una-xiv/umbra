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
using System;
using ImGuiNET;
using Umbra.Common;
using Umbra.Windows.Library.IconPicker;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class FaIconInputNode : Node
{
    public event Action<FontAwesomeIcon>? OnValueChanged;

    public FontAwesomeIcon Value {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
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

    private FontAwesomeIcon _value;

    public FaIconInputNode(string id, FontAwesomeIcon value, string label, string? description = null)
    {
        _value = value;

        Id         = id;
        ClassList  = ["input"];
        Stylesheet = ColorInputStylesheet;

        ChildNodes = [
            new() {
                Id          = "IconBox",
                ClassList   = ["input--box"],
                NodeValue   = value.ToIconString(),
                InheritTags = true,
            },
            new() {
                Id          = "Text",
                ClassList   = ["input--text"],
                InheritTags = true,
                ChildNodes = [
                    new() {
                        Id          = "Label",
                        ClassList   = ["input--text--label"],
                        InheritTags = true,
                        NodeValue   = label,
                    },
                    new() {
                        Id          = "Description",
                        ClassList   = ["input--text--description"],
                        InheritTags = true,
                        NodeValue   = description,
                    },
                ],
            },
        ];

        Label       = label;
        Description = description;

        BeforeReflow += _ => {
            int maxWidth = ParentNode!.Bounds.ContentSize.Width - ParentNode!.ComputedStyle.Padding.HorizontalSize;
            int padding  = ComputedStyle.Gap + BoxNode.OuterWidth;
            int width    = (int)((maxWidth - padding) / ScaleFactor);
            int labelHeight;

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

        OnMouseUp += _ => {
            Framework
                .Service<WindowManager>()
                .Present(
                    "IconPicker",
                    new FaIconPickerWindow(Value),
                    window => {
                        Value = window.Icon;
                    }
                );
        };
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? []) OnValueChanged -= (Action<FontAwesomeIcon>)handler;

        OnValueChanged = null;

        base.OnDisposed();
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        BoxNode.NodeValue = Value.ToIconString();
    }

    private Node BoxNode         => QuerySelector(".input--box")!;
    private Node LabelNode       => QuerySelector(".input--text--label")!;
    private Node DescriptionNode => QuerySelector(".input--text--description")!;

    private static Stylesheet ColorInputStylesheet { get; } = new(
        [
            new(
                ".input",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 4,
                }
            ),
            new(
                ".input--box",
                new() {
                    Flow            = Flow.Horizontal,
                    Anchor          = Anchor.TopLeft,
                    Font            = (int)FontId.FontAwesome,
                    FontSize        = 16,
                    TextAlign       = Anchor.MiddleCenter,
                    Size            = new(28, 28),
                    BorderRadius    = 5,
                    StrokeWidth     = 3,
                    StrokeInset     = -1,
                    BackgroundColor = new("Input.Background"),
                    StrokeColor     = new("Input.Border"),
                    Color           = new("Input.Text"),
                    IsAntialiased   = false,
                }
            ),
            new(
                ".input--box:hover",
                new() {
                    StrokeColor = new("Input.BorderHover"),
                    Color       = new("Input.TextHover"),
                }
            ),
            new(
                ".input--text",
                new() {
                    Flow   = Flow.Vertical,
                    Anchor = Anchor.TopLeft,
                    Gap    = 4,
                }
            ),
            new(
                ".input--text--label",
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
                ".input--text--label:hover",
                new() {
                    Color = new("Input.TextHover"),
                }
            ),
            new(
                ".input--text--description",
                new() {
                    Anchor       = Anchor.TopLeft,
                    FontSize     = 11,
                    Color        = new("Input.TextMuted"),
                    TextOverflow = false,
                    WordWrap     = true,
                    LineHeight   = 1.5f,
                }
            ),
        ]
    );
}
