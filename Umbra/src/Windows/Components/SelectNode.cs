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
using System.Numerics;
using ImGuiNET;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Components;

internal class SelectNode : Node
{
    public event Action<string>? OnValueChanged;

    public string Value {
        get => _value;
        set {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public string? Label {
        get => (string?)LabelNode.NodeValue;
        set => LabelNode.NodeValue = value;
    }

    public string? Description {
        get => (string?)DescriptionNode.NodeValue;
        set => DescriptionNode.NodeValue = value;
    }

    public List<string> Choices {
        get => _choices;
        set {
            _isLocked = true;
            Framework.DalamudFramework.Run(
                () => {
                    _choices.Clear();
                    _choices.AddRange(value);
                    _isLocked = false;
                }
            );
        }
    }

    public void SetValueInternal(string c) => _value = c;

    private readonly List<string> _choices;
    private          string       _value;
    private          bool         _isLocked;

    public SelectNode(
        string id, string value, List<string> choices, string? label = null, string? description = null,
        int    leftMargin = 32
    )
    {
        _value   = value;
        _choices = choices;

        Id         = id;
        ClassList  = ["select"];
        Stylesheet = SelectStylesheet;

        Style = new() {
            Padding = new() { Left = leftMargin }
        };

        ChildNodes = [
            new() {
                Id        = "Label",
                ClassList = ["select--label"],
                NodeValue = label,
            },
            new() {
                ClassList = ["select--box"],
            },
            new() {
                ClassList = ["select--description"],
                NodeValue = description,
            },
        ];

        BeforeReflow += _ => {
            int maxWidth = ParentNode!.Bounds.ContentSize.Width - ParentNode!.ComputedStyle.Padding.HorizontalSize;
            int padding  = ComputedStyle.Gap + (int)(leftMargin * ScaleFactor);
            int width    = (int)((maxWidth - padding) / ScaleFactor);
            int labelHeight;

            if (LabelNode.Style.Size?.Width == width && DescriptionNode.Style.Size?.Width == width) {
                return false;
            }

            LabelNode.Style.IsVisible = !string.IsNullOrEmpty((string?)LabelNode.NodeValue);

            if (string.IsNullOrEmpty((string?)DescriptionNode.NodeValue)) {
                DescriptionNode.Style.IsVisible = false;
                labelHeight                     = 24;
            } else {
                DescriptionNode.Style.IsVisible = true;
                labelHeight                     = 0;
            }

            LabelNode.Style.Size       = new(width, labelHeight);
            DescriptionNode.Style.Size = new(width, 0);
            SelectBoxNode.Style.Size   = new(width, 26);

            return true;
        };
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        var bounds = QuerySelector(".select--box")!.Bounds;
        ImGui.SetCursorScreenPos(bounds.ContentRect.TopLeft);

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,  new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 5);

        ImGui.PushStyleColor(ImGuiCol.Text,           new Color("Input.Text").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.PopupBg,        new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.Button,         new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered,  new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBg,        new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive,  new Color("Input.Background").ToUInt());

        ImGui.SetNextItemWidth(bounds.ContentSize.Width);

        // Ensure the current value is amongst the choices.
        if (!_isLocked && !_choices.Contains(Value)) {
            Logger.Warning($"Selected value [{Value}] is not amongst children in {Id}. Possible values: {string.Join(", ", _choices)}");
            Value = _choices.Count > 0 ? _choices[0] : "";
        }

        if (ImGui.BeginCombo($"##{Id}", Value)) {
            foreach (string choice in _choices) {
                bool isSelected = Value == choice;

                if (ImGui.Selectable(choice, isSelected)) {
                    Value = choice;
                }

                if (isSelected) {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        ImGui.PopStyleVar(4);
        ImGui.PopStyleColor(7);
    }

    protected override void OnDisposed()
    {
        foreach (var handler in OnValueChanged?.GetInvocationList() ?? [])  OnValueChanged -= (Action<string>)handler;
    }

    private Node SelectBoxNode   => QuerySelector(".select--box")!;
    private Node LabelNode       => QuerySelector(".select--label")!;
    private Node DescriptionNode => QuerySelector(".select--description")!;

    private static Stylesheet SelectStylesheet { get; } = new(
        [
            new(
                ".select",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 6,
                    Padding = new() { Left = 32 }
                }
            ),
            new(
                ".select--box",
                new() {
                    Size = new(0, 26),
                }
            ),
            new(
                ".select--label",
                new() {
                    FontSize     = 14,
                    Color        = new("Window.Text"),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".select--description",
                new() {
                    FontSize     = 11,
                    Color        = new("Window.TextMuted"),
                    TextOverflow = false,
                    WordWrap     = true,
                    LineHeight   = 1.5f,
                }
            ),
        ]
    );
}
