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
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using Una.Drawing;

namespace Umbra.Windows.Components;

internal class ColorInputNode : Node
{
    public event Action<uint>? OnValueChanged;

    public uint Value {
        get => _value;
        set {
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

    private uint _value;

    public ColorInputNode(string id, uint value, string label, string? description = null)
    {
        _value = value;

        Id         = id;
        ClassList  = ["color"];
        Stylesheet = ColorInputStylesheet;

        ChildNodes = [
            new() {
                Id          = "Checkbox",
                ClassList   = ["input--box"],
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
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])  OnValueChanged -= (Action<uint>)handler;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        Vector4 value      = ImGui.ColorConvertU32ToFloat4(Value);
        Node    box        = QuerySelector(".input--box")!;

        box.Style.BackgroundColor = new(Value);

        var bounds   = Bounds.PaddingRect;
        var popupId  = $"##{Id}";
        var pickerId = $"##{Id}";

        ImGui.SetCursorScreenPos(bounds.TopLeft);

        ImGui.PushID($"ColorPicker{Id}");

        if (ImGui.InvisibleButton($"##{Id}", new(bounds.Width, bounds.Height))) {
            ImGui.OpenPopup(popupId);
        }

        if (ImGui.BeginPopup(popupId)) {
            ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1);
            ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding,   8);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,   new Vector2(4, 4));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,    new Vector2(4, 4));

            if (ImGui.ColorPicker4(
                    pickerId,
                    ref value,
                    ImGuiColorEditFlags.NoLabel
                    | ImGuiColorEditFlags.AlphaBar
                    | ImGuiColorEditFlags.DisplayMask
                    | ImGuiColorEditFlags.AlphaPreview
                    | ImGuiColorEditFlags.NoSidePreview
                    | ImGuiColorEditFlags.NoSmallPreview
                    | ImGuiColorEditFlags.NoTooltip
                )) {
                uint val = ImGui.ColorConvertFloat4ToU32(value);
                Value = val;
            }

            ImGui.EndPopup();
            ImGui.PopStyleVar(4);
        }

        ImGui.PopID();
    }

    private Node BoxNode         => QuerySelector(".input--box")!;
    private Node LabelNode       => QuerySelector(".input--text--label")!;
    private Node DescriptionNode => QuerySelector(".input--text--description")!;

    private static Stylesheet ColorInputStylesheet { get; } = new(
        [
            new(
                ".color",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".input--box",
                new() {
                    Flow            = Flow.Horizontal,
                    Anchor          = Anchor.TopLeft,
                    Size            = new(28, 28),
                    BorderRadius    = 5,
                    StrokeWidth     = 3,
                    StrokeInset     = -1,
                    BackgroundColor = new(0),
                    StrokeColor     = new("Input.Background"),
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
