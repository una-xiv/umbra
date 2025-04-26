﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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

using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.AddWidget;

internal class AddWidgetWindow(string locationId) : Window
{
    public event Action<string>? OnWidgetAdded;

    public string? SelectedWidgetId { get; private set; }

    protected override Vector2 MinSize     { get; } = new(400, 300);
    protected override Vector2 MaxSize     { get; } = new(600, 600);
    protected override Vector2 DefaultSize { get; } = new(400, 300);
    protected override string  Title       { get; } = I18N.Translate("Settings.AddWidgetWindow.Title");

    private WidgetInfo?   _selectedWidgetInfo;
    private WidgetManager WidgetManager { get; } = Framework.Service<WidgetManager>();

    private readonly Dictionary<Node, WidgetInfo> _widgetNodes = [];

    protected override Node Node { get; } = new() {
        Stylesheet = AddWidgetStylesheet,
        ClassList  = ["add-widget-window"],
        ChildNodes = [
            new() {
                Id = "SearchPanel",
                ChildNodes = [
                    new() {
                        Id        = "SearchIcon",
                        NodeValue = FontAwesomeIcon.Search.ToIconString(),
                    },
                    new() {
                        Id         = "SearchInputWrapper",
                        ChildNodes = [new StringInputNode("Search", "", 128, null, null, 0, true)]
                    }
                ]
            },
            new() {
                ClassList = ["add-widget-list--wrapper"],
                Overflow  = false,
                ChildNodes = [
                    new() {
                        ClassList = ["add-widget-list"],
                    }
                ]
            },
            new() {
                ClassList = ["add-widget-footer"],
                ChildNodes = [
                    new() {
                        ClassList = ["add-widget-footer--buttons", "left-side"],
                        ChildNodes = [
                            new ButtonNode(
                                "PasteButton",
                                I18N.Translate("Paste"),
                                FontAwesomeIcon.Paste
                            ),
                        ]
                    },
                    new() {
                        ClassList = ["add-widget-footer--buttons"],
                        ChildNodes = [
                            new ButtonNode("CancelButton", I18N.Translate("Cancel")),
                            new ButtonNode("AddButton",    I18N.Translate("Add"), FontAwesomeIcon.PlusCircle),
                        ]
                    },
                ]
            }
        ]
    };

    protected override void OnDisposed()
    {
        foreach (var handler in OnWidgetAdded?.GetInvocationList() ?? []) OnWidgetAdded -= (Action<string>)handler;

        _widgetNodes.Clear();
    }

    protected override void OnOpen()
    {
        WidgetManager
            .GetWidgetInfoList()
            .OrderBy(w => w.Name)
            .ToList()
            .ForEach(
                info => { Node.QuerySelector(".add-widget-list")!.AppendChild(CreateWidgetNode(info)); }
            );

        Node.QuerySelector("#CancelButton")!.OnMouseUp += _ => {
            SelectedWidgetId = null;
            Close();
        };

        Node addButton   = Node.QuerySelector("#AddButton")!;
        Node pasteButton = Node.QuerySelector("#PasteButton")!;

        addButton.Tooltip           = I18N.Translate("Settings.AddWidgetWindow.AddButtonTooltip");
        pasteButton.Tooltip         = I18N.Translate("Settings.AddWidgetWindow.AddButtonTooltip");
        pasteButton.Style.IsVisible = WidgetManager.CanCreateInstanceFromClipboard();

        addButton.QuerySelector("#Label")!.Style.MaxWidth = 150;

        pasteButton.OnMouseUp += _ => {
            WidgetManager.CreateInstanceFromClipboard(locationId);

            if (!ImGui.GetIO().KeyShift) {
                Close();
            }
        };

        addButton.OnMouseUp += _ => {
            if (_selectedWidgetInfo is null) return;
            OnWidgetAdded?.Invoke(_selectedWidgetInfo.Id);

            if (!ImGui.GetIO().KeyShift) {
                Close();
            }
        };

        Node.QuerySelector<StringInputNode>("#Search")!.OnValueChanged += OnSearchValueChanged;
    }

    protected override void OnUpdate(int instanceId)
    {
        Node.Style.Size                                             = ContentSize;
        Node.QuerySelector(".add-widget-list--wrapper")!.Style.Size = new(ContentSize.Width, ContentSize.Height - 95);
        Node.QuerySelector(".add-widget-footer")!.Style.Size        = new(ContentSize.Width, 50);
        Node.QuerySelector("#SearchPanel")!.Style.Size              = new(ContentSize.Width, 0);
        Node.QuerySelector("#SearchInputWrapper")!.Style.Size       = new(ContentSize.Width - 55, 0);
        Node.QuerySelector("#AddButton")!.IsDisabled                = _selectedWidgetInfo is null;

        var addButton = Node.QuerySelector<ButtonNode>("#AddButton")!;

        addButton.Label = _selectedWidgetInfo is not null
            ? I18N.Translate(
                "Settings.AddWidgetWindow.AddButton",
                _selectedWidgetInfo.Name.Length > 30 ? _selectedWidgetInfo.Name[..30] + "..." : _selectedWidgetInfo.Name
            )
            : I18N.Translate("Settings.AddWidgetWindow.AddButtonNone");

        foreach (var widgetNode in Node.QuerySelectorAll(".widget")) {
            widgetNode.Style.Size = new(ContentSize.Width - 30, 0);
        }

        foreach (var widgetNode in Node.QuerySelectorAll(".widget--name, .widget--description")) {
            widgetNode.Style.Size = new(ContentSize.Width - 60, 0);
        }
    }

    protected override void OnClose()
    {
        Node.QuerySelector<StringInputNode>("#Search")!.OnValueChanged -= OnSearchValueChanged;
    }

    private Node CreateWidgetNode(WidgetInfo info)
    {
        uint instanceCount = WidgetManager.GetWidgetInstanceCount(info.Id);

        Node node = new() {
            ClassList = ["widget"],
            ChildNodes = [
                new() {
                    ClassList = ["widget--name"],
                    NodeValue = info.Name,
                },
                new() {
                    ClassList = ["widget--tags"],
                    ChildNodes = [
                        ..info
                            .Tags.Take(3)
                            .Select(
                                tag => new Node() {
                                    ClassList = ["tag"],
                                    NodeValue = tag
                                }
                            )
                    ]
                },
                new() {
                    ClassList = ["widget--description"],
                    NodeValue = info.Description,
                    Style     = new() { IsVisible = false },
                },
                new() {
                    ClassList = ["widget-instance-count"],
                    NodeValue = $"{instanceCount}",
                    TagsList  = instanceCount > 0 ? ["used"] : [],
                }
            ]
        };

        if (info.Tags.Count > 3) {
            int remainingTags = info.Tags.Count - 3;

            node.QuerySelector(".widget--tags")!.AppendChild(
                new() {
                    ClassList = ["tag"],
                    NodeValue = $"+{remainingTags}",
                    Tooltip   = string.Join(", ", info.Tags.Skip(3)),
                }
            );
        }

        node.OnMouseUp += _ => {
            _selectedWidgetInfo = info;

            foreach (var widgetNode in Node.QuerySelectorAll(".widget")) {
                widgetNode.ClassList.Remove("selected");
            }

            node.ClassList.Add("selected");
        };

        node.BeforeDraw += _ => {
            bool isSelected = node.ClassList.Contains("selected");
            node.QuerySelector(".widget--description")!.Style.IsVisible = isSelected;
            node.QuerySelector(".widget--tags")!.Style.IsVisible        = isSelected;
        };

        _widgetNodes[node] = info;

        return node;
    }

    private void OnSearchValueChanged(string value)
    {
        foreach (string word in value.Split(' ')) {
            if (string.IsNullOrEmpty(word)) continue;

            foreach (Node node in Node.QuerySelectorAll(".widget")) {
                if (!_widgetNodes.TryGetValue(node, out WidgetInfo? info)) continue;

                node.Style.IsVisible = info.Name.Contains(word, StringComparison.OrdinalIgnoreCase)
                    || info.Tags.Any(t => t.Contains(word, StringComparison.OrdinalIgnoreCase));
            }
        }
    }

    private static Stylesheet AddWidgetStylesheet { get; } = new(
        [
            new(
                ".add-widget-window",
                new() {
                    Flow = Flow.Vertical,
                }
            ),
            new(
                "#SearchPanel",
                new() {
                    Flow            = Flow.Horizontal,
                    Size            = new(0, 45),
                    FontSize        = 16,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Bottom = new("Window.Border") },
                    BorderWidth     = new() { Bottom = 1 },
                    IsAntialiased   = false,
                    Padding         = new(10, 15),
                    Gap             = 5,
                }
            ),
            new(
                "#SearchInputWrapper",
                new() {
                    Flow = Flow.Horizontal,
                    Size = new(0, 30),
                }
            ),
            new(
                "#SearchIcon",
                new() {
                    Size         = new(26, 26),
                    Font         = 2,
                    FontSize     = 18,
                    Color        = new("Window.TextMuted"),
                    OutlineColor = new("Window.TextOutline"),
                    OutlineSize  = 1,
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, -1),
                }
            ),
            new(
                ".add-widget-list--wrapper",
                new() {
                    Flow                      = Flow.Vertical,
                    ScrollbarTrackColor       = new(0),
                    ScrollbarThumbColor       = new("Window.ScrollbarThumb"),
                    ScrollbarThumbHoverColor  = new("Window.ScrollbarThumbHover"),
                    ScrollbarThumbActiveColor = new("Window.ScrollbarThumbActive"),
                }
            ),
            new(
                ".add-widget-list",
                new() {
                    Flow    = Flow.Vertical,
                    Gap     = 15,
                    Padding = new(15),
                }
            ),
            new(
                ".add-widget-footer",
                new() {
                    Flow            = Flow.Horizontal,
                    Gap             = 15,
                    Padding         = new(0, 15),
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderColor     = new() { Top = new("Window.Border") },
                    BorderWidth     = new() { Top = 1 },
                    IsAntialiased   = false,
                }
            ),
            new(
                ".add-widget-footer--buttons",
                new() {
                    Anchor = Anchor.MiddleRight,
                    Gap    = 15,
                }
            ),
            new(
                ".add-widget-footer--buttons.left-side",
                new() {
                    Anchor = Anchor.MiddleLeft,
                }
            ),
            new(
                ".widget",
                new() {
                    Flow            = Flow.Vertical,
                    Gap             = 8,
                    BackgroundColor = new("Window.BackgroundLight"),
                    BorderRadius    = 7,
                    Padding         = new(15),
                }
            ),
            new(
                ".widget.selected",
                new() {
                    StrokeColor = new("Window.TextMuted"),
                    StrokeWidth = 1,
                }
            ),
            new(
                ".widget--name",
                new() {
                    FontSize     = 16,
                    Color        = new("Window.Text"),
                    TextOverflow = false,
                    WordWrap     = false,
                }
            ),
            new(
                ".widget--tags",
                new() {
                    Flow = Flow.Horizontal,
                    Gap  = 8,
                }
            ),
            new(
                ".tag",
                new() {
                    TextAlign       = Anchor.MiddleCenter,
                    BorderRadius    = 6,
                    Size            = new(0, 16),
                    Padding         = new(0, 6),
                    FontSize        = 10,
                    BackgroundColor = new("Window.Background"),
                    Color           = new("Window.TextMuted"),
                }
            ),
            new(
                ".widget--description",
                new() {
                    FontSize     = 11,
                    Color        = new("Window.TextMuted"),
                    TextOverflow = false,
                    WordWrap     = true,
                    LineHeight   = 1.5f,
                }
            ),
            new(
                ".widget-instance-count",
                new() {
                    Anchor        = Anchor.TopRight,
                    Color         = new("Window.Text"),
                    OutlineColor  = new("Window.TextOutline"),
                    OutlineSize   = 1,
                    FontSize      = 13,
                    Size          = new(28, 28),
                    TextAlign     = Anchor.MiddleCenter,
                    BorderRadius  = 14,
                    IsAntialiased = false,
                    Opacity       = 0,
                }
            ),
            new(
                ".widget-instance-count:used",
                new() {
                    BackgroundColor = new(0x50FFFFFF),
                    Opacity         = 1,
                }
            ),
        ]
    );
}
