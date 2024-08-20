using System;
using Una.Drawing;

namespace Umbra.Widgets;

public sealed class ContextMenuEntry(string? id) : IDisposable
{
    internal Action? OnInvoke;

    internal Node Node { get; } = new() {
        ClassList  = ["context-menu-entry"],
        Stylesheet = Stylesheet,
        ChildNodes = [
            new() { Id = "Icon", InheritTags  = true },
            new() { Id = "Label", InheritTags = true },
        ],
    };

    public void Dispose()
    {
        OnInvoke = null;
        Node.Dispose();
    }

    public string Id { get; init; } = id ?? $"CME_{Guid.NewGuid().ToString()}";

    public string? Label {
        get => (string)Node.FindById("Label")!.NodeValue!;
        set => Node.FindById("Label")!.NodeValue = value;
    }

    public uint? IconId {
        get => Node.FindById("Icon")!.Style.IconId;
        set => Node.FindById("Icon")!.Style.IconId = value;
    }

    public bool IsDisabled {
        get => Node.IsDisabled;
        set => Node.IsDisabled = value;
    }

    public bool IsVisible {
        get => Node.Style.IsVisible ?? true;
        set => Node.Style.IsVisible = value;
    }

    public string? Tooltip {
        get => Node.Tooltip;
        set => Node.Tooltip = value;
    }

    public Action? OnClick {
        get => _onClick;
        set
        {
            _onClick = value;

            if (null == value) {
                Node.OnMouseUp -= HandleClick;
                _isBound       =  false;
                return;
            }

            if (!_isBound) {
                Node.OnMouseUp += HandleClick;
                _isBound       =  true;
            }
        }
    }

    private Action? _onClick;
    private bool    _isBound;

    private void HandleClick(Node _)
    {
        if (!IsDisabled) {
            Node.TagsList.Remove("hover");
            _onClick?.Invoke();
            OnInvoke?.Invoke();
        }
    }

    private static Stylesheet Stylesheet { get; } = new(
        [
            new(
                ".context-menu-entry",
                new() {
                    Flow          = Flow.Horizontal,
                    Stretch       = true,
                    BorderRadius  = 5,
                    IsAntialiased = false,
                    Padding       = new(0, 4),
                }
            ),
            new(
                ".context-menu-entry:hover",
                new() {
                    BackgroundColor = new("Widget.PopupMenuBackgroundHover"),
                }
            ),
            new(
                ".context-menu-entry:disabled",
                new() {
                    Opacity = 0.85f,
                }
            ),
            new(
                "#Icon",
                new() {
                    Size   = new(18, 18),
                    Anchor = Anchor.MiddleLeft,
                }
            ),
            new(
                "#Icon:disabled",
                new() {
                    ImageGrayscale = true,
                }
            ),
            new(
                "#Label",
                new() {
                    Anchor       = Anchor.MiddleLeft,
                    FontSize     = 12,
                    Color        = new("Widget.PopupMenuText"),
                    OutlineColor = new("Widget.PopupMenuTextOutline"),
                    OutlineSize  = 1,
                    Size         = new(0, 24),
                    TextAlign    = Anchor.MiddleLeft,
                    Padding      = new(4),
                }
            ),
            new(
                "#Label:hover",
                new() {
                    Color        = new("Widget.PopupMenuTextHover"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineHover"),
                }
            ),
            new(
                "#Label:disabled",
                new() {
                    Color        = new("Widget.PopupMenuTextDisabled"),
                    OutlineColor = new("Widget.PopupMenuTextOutlineDisabled"),
                }
            )
        ]
    );
}
