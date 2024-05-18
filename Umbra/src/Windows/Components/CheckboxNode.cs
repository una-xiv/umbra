using System;
using System.Reflection.Emit;
using Dalamud.Interface;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Components;

public class CheckboxNode : Node
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

    private bool _value = false;

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

        OnClick += _ => {
            Value = !Value;
        };

        BeforeReflow += _ => {
            int maxWidth = ParentNode!.Bounds.ContentSize.Width - ParentNode!.ComputedStyle.Padding.HorizontalSize;
            int padding  = ComputedStyle.Gap + BoxNode.OuterWidth;
            int width    = maxWidth - padding;
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
        new() {
            {
                ".checkbox", new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            }, {
                ".checkbox--box", new() {
                    Flow            = Flow.Horizontal,
                    Anchor          = Anchor.TopLeft,
                    Size            = new(24, 24),
                    BorderRadius    = 6,
                    StrokeWidth     = 1,
                    StrokeInset     = 1,
                    Font            = 2,
                    BackgroundColor = new("Checkbox.Background"),
                    StrokeColor     = new("Checkbox.Border"),
                    Color           = new("Checkbox.Checkmark"),
                    TextAlign       = Anchor.MiddleCenter,
                    TextOffset      = new(0, -1),
                }
            }, {
                ".checkbox--box:hover", new() {
                    BackgroundColor = new("Checkbox.BackgroundHover"),
                    StrokeColor     = new("Checkbox.BorderHover"),
                    Color           = new("Checkbox.Checkmark"),
                }
            }, {
                ".checkbox--text", new() {
                    Flow   = Flow.Vertical,
                    Anchor = Anchor.TopLeft,
                    Gap    = 4,
                }
            }, {
                ".checkbox--text--label", new() {
                    Anchor       = Anchor.TopLeft,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOverflow = false,
                    FontSize     = 13,
                    Color        = new("Checkbox.TextLabel"),
                    WordWrap     = false,
                }
            }, {
                ".checkbox--text--label:hover", new() {
                    Color = new("Checkbox.TextLabelHover"),
                }
            }, {
                ".checkbox--text--description", new() {
                    Anchor       = Anchor.TopLeft,
                    FontSize     = 11,
                    Color        = new("Checkbox.TextDescription"),
                    TextOverflow = false,
                    WordWrap     = true,
                }
            }
        }
    );
}
