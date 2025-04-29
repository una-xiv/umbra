using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Una.Drawing;

namespace Umbra.Widgets;

public sealed partial class MenuPopup
{
    public interface IMenuItem
    {
        public Node Node { get; }
    }

    public class Button : IMenuItem
    {
        public string? Id {
            get => Node.Id;
            set => Node.Id = value;
        }
        
        public bool IsVisible {
            get => Node.IsVisible;
            set => Node.Style.IsVisible = value;
        }

        public bool IsDisabled {
            get => Node.IsDisabled;
            set => Node.IsDisabled = value;
        }

        public int SortIndex {
            get => Node.SortIndex;
            set => Node.SortIndex = value;
        }

        public string Label {
            get => Node.QuerySelector(".text")!.NodeValue?.ToString() ?? string.Empty;
            set => Node.QuerySelector(".text")!.NodeValue = value;
        }

        public string? AltText {
            get => Node.QuerySelector(".alt-text")!.NodeValue?.ToString() ?? string.Empty;
            set => Node.QuerySelector(".alt-text")!.NodeValue = value;
        }

        public bool Selected {
            get => Node.ClassList.Contains("selected");
            set => Node.ToggleClass("selected", value);
        }

        public Color? IconColor {
            get => IconNode.Style.Color;
            set
            {
                IconNode.Style.Color      = value;
                IconNode.Style.ImageColor = value;
            }
        }

        public bool ClosePopupOnClick { get; set; } = true;

        public object? Icon {
            get => _icon;
            set
            {
                _icon = value;
                bool isGameIcon   = false;
                bool isFaIcon     = false;
                bool isGlyphIcon  = false;
                bool isBitmapIcon = false;

                if (value is uint iconId) {
                    IconNode.NodeValue            = null;
                    IconNode.Style.IconId         = iconId;
                    IconNode.Style.BitmapFontIcon = null;
                    isGameIcon                    = true;
                } else if (value is FontAwesomeIcon faIcon) {
                    IconNode.NodeValue            = faIcon.ToIconString();
                    IconNode.Style.Font           = 2;
                    IconNode.Style.IconId         = null;
                    IconNode.Style.BitmapFontIcon = null;
                    isFaIcon                      = true;
                } else if (value is SeIconChar seIcon) {
                    IconNode.NodeValue            = seIcon.ToIconString();
                    IconNode.Style.Font           = 0;
                    IconNode.Style.IconId         = null;
                    IconNode.Style.BitmapFontIcon = null;
                    isGlyphIcon                   = true;
                } else if (value is BitmapFontIcon bitmap) {
                    IconNode.NodeValue            = null;
                    IconNode.Style.Font           = 0;
                    IconNode.Style.IconId         = null;
                    IconNode.Style.BitmapFontIcon = bitmap;
                    isBitmapIcon                  = true;
                } else {
                    IconNode.NodeValue            = null;
                    IconNode.Style.Font           = 0;
                    IconNode.Style.IconId         = null;
                    IconNode.Style.BitmapFontIcon = null;
                    isGameIcon                    = false;
                    isFaIcon                      = false;
                    isGlyphIcon                   = false;
                    isBitmapIcon                  = false;
                }

                IconNode.ToggleClass("game-icon", isGameIcon);
                IconNode.ToggleClass("fa-icon", isFaIcon);
                IconNode.ToggleClass("glyph-icon", isGlyphIcon);
                IconNode.ToggleClass("bitmap-icon", isBitmapIcon);
            }
        }

        public Action? OnClick {
            get => _onClickInternal;
            set
            {
                if (_onClick != null) {
                    Node.OnClick -= _onClick;
                }

                _onClickInternal = value;

                if (value == null) return;

                _onClick     =  _ => _onClickInternal?.Invoke();
                Node.OnClick += _onClick;
            }
        }

        private object?       _icon;
        private Action<Node>? _onClick;
        private Action?       _onClickInternal;

        private Node IconNode => Node.QuerySelector(".icon")!;

        public Node Node { get; } = new() {
            ClassList = ["button"],
            ChildNodes = [
                new() { ClassList = ["icon"] },
                new() { ClassList = ["text"] },
                new() { ClassList = ["alt-text"] }
            ]
        };

        public Button(string label)
        {
            Label = label;
        }
    }

    public class Separator : IMenuItem
    {
        public bool IsVisible {
            get => Node.IsVisible;
            set => Node.Style.IsVisible = value;
        }

        public int SortIndex {
            get => Node.SortIndex;
            set => Node.SortIndex = value;
        }

        public Node Node { get; } = new() {
            ClassList = ["separator"]
        };
    }

    public class Header : IMenuItem
    {
        public bool IsVisible {
            get => Node.IsVisible;
            set => Node.Style.IsVisible = value;
        }

        public string Label {
            get => Node.QuerySelector(".text")!.NodeValue?.ToString() ?? string.Empty;
            set => Node.QuerySelector(".text")!.NodeValue = value;
        }

        public Node Node { get; } = new() {
            ClassList = ["header"],
            ChildNodes = [
                new() { ClassList = ["line"] },
                new() { ClassList = ["text"] },
                new() { ClassList = ["line"] },
            ]
        };

        public Header(string label)
        {
            Label = label;
        }
    }

    public class Group : IMenuItem, IDisposable
    {
        public event Action<IMenuItem>? OnButtonClicked;

        private readonly Dictionary<Node, IMenuItem> _items = [];

        public string? Label {
            get => LabelNode.NodeValue?.ToString() ?? string.Empty;
            set => LabelNode.NodeValue = value;
        }

        public int SortIndex {
            get => Node.SortIndex;
            set => Node.SortIndex = value;
        }

        public Node Node { get; } = new() {
            ClassList = ["group"],
            ChildNodes = [
                new() {
                    ClassList = ["header"],
                    ChildNodes = [
                        new() { ClassList = ["line"] },
                        new() { ClassList = ["text"] },
                        new() { ClassList = ["line"] }
                    ]
                },
                new() { ClassList = ["content"] }
            ]
        };

        private Node HeaderNode  => Node.QuerySelector(".header")!;
        private Node LabelNode   => Node.QuerySelector(".header > .text")!;
        private Node ContentNode => Node.QuerySelector(".content")!;

        public Group(string label)
        {
            Label = label;

            Node.BeforeDraw += _ => {
                Node.Style.IsVisible       = ContentNode.ChildNodes.Any(c => c.IsVisible);
                HeaderNode.Style.IsVisible = !string.IsNullOrEmpty(Label);
            };

            Node.OnDispose += _ => Dispose();
        }

        public void Add(IMenuItem item)
        {
            if (item.Node.ParentNode == ContentNode) return;

            ContentNode.AppendChild(item.Node);
            _items.Add(item.Node, item);

            if (item is Button button) button.Node.OnClick += _ => OnButtonClicked?.Invoke(item);
        }

        public T Get<T>(string id) where T : IMenuItem
        {
            Node? node = ContentNode.QuerySelector(id);
            if (null != node && _items.TryGetValue(node, out var item)) {
                return (T)item;
            }

            throw new KeyNotFoundException($"Item with ID '{id}' not found in the group.");
        }

        public void Remove(IMenuItem item, bool dispose = false)
        {
            if (item.Node.ParentNode != ContentNode) return;

            _items.Remove(item.Node);
            item.Node.Remove(dispose);
        }

        public void RemoveById(string id, bool dispose = false)
        {
            foreach (var item in _items.Values.ToImmutableArray()) {
                if (item is Group group) {
                    group.RemoveById(id, dispose);
                } else if (item.Node.Id == id) {
                    Remove(item, dispose);
                }
            }
        }

        public void Dispose()
        {
            foreach (var item in _items.Keys.ToImmutableArray()) {
                item.Dispose();
                _items.Remove(item);
            }

            foreach (Delegate handler in OnButtonClicked?.GetInvocationList() ?? []) {
                if (handler is Action<IMenuItem> listener) OnButtonClicked -= listener;
            }
        }
    }
}
