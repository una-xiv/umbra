using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using ImGuiNET;
using Lumina.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.WidgetBrowser;

public class WidgetBrowserWindow : Window
{
    public string? SelectedWidgetId { get; private set; }

    protected override string  UdtResourceName => "umbra.windows.widget_browser.window.xml";
    protected override string  Title           => I18N.Translate("Settings.AddWidgetWindow.Title");
    protected override Vector2 MinSize         => new(400, 250);
    protected override Vector2 MaxSize         => new(700, 800);
    protected override Vector2 DefaultSize     => new(500, 500);

    private WidgetManager Manager { get; } = Framework.Service<WidgetManager>();

    private readonly Dictionary<Node, WidgetInfo> _widgetNodes = [];

    private string? _selectedWidgetId;

    protected override ImGuiWindowFlags GetWindowFlags()
    {
        return ImGuiWindowFlags.Modal;
    }

    protected override void OnOpen()
    {
        RootNode.QuerySelector("#cancel-button")!.OnClick += _ => Dispose();
        RootNode.QuerySelector("#confirm-button")!.OnClick += _ => {
            SelectedWidgetId = _selectedWidgetId;
            Dispose();
        };

        RootNode.QuerySelector<StringInputNode>("#search")!.OnValueChanged += str => {
            foreach (Node node in RootNode.QuerySelectorAll(".widget")) {
                WidgetInfo info = _widgetNodes[node];

                node.Style.IsVisible = string.IsNullOrWhiteSpace(str)
                                       || info.Name.Contains(str, StringComparison.OrdinalIgnoreCase)
                                       || info.Tags.Any(tag => tag.Contains(str, StringComparison.OrdinalIgnoreCase));
            }
        };

        Manager.GetWidgetInfoList();

        foreach (var widget in Manager.GetWidgetInfoList().OrderBy(w => w.Name)) {
            CreateWidgetSelectorNode(widget);
        }
    }

    protected override void OnDraw()
    {
        WidgetInfo? widgetInfo    = _selectedWidgetId != null ? Manager.GetWidgetInfo(_selectedWidgetId) : null;
        ButtonNode  confirmButton = RootNode.QuerySelector<ButtonNode>("#confirm-button")!;

        confirmButton.IsDisabled = widgetInfo == null;
        confirmButton.Label = widgetInfo != null
            ? I18N.Translate("Settings.AddWidgetWindow.AddButton", widgetInfo.Name)
            : I18N.Translate("Settings.AddWidgetWindow.AddButtonNone");

        if (_selectedWidgetId == null) return;

        string selectedId = $"Widget-{Crc32.Get(_selectedWidgetId)}";
        foreach (var node in RootNode.QuerySelectorAll(".widget")) {
            node.ToggleClass("selected", node.Id == selectedId);
        }
    }

    private void CreateWidgetSelectorNode(WidgetInfo widget)
    {
        Node node = Document!.CreateNodeFromTemplate("widget", new() {
            { "label", widget.Name },
            { "description", widget.Description },
        });

        RootNode.QuerySelector("#widgets")!.AppendChild(node);

        node.Id      =  $"Widget-{Crc32.Get(widget.Id)}";
        node.OnClick += _ => { _selectedWidgetId = widget.Id; };
        node.OnDoubleClick += _ => {
            SelectedWidgetId = widget.Id;
            Dispose();
        };
        
        if (widget.IsDeprecated) {
            node.QuerySelector(".deprecated")!.Style.IsVisible = true;
            node.QuerySelector(".deprecated")!.NodeValue = new SeStringBuilder().AddIcon(BitmapFontIcon.Warning).AddText(widget.DeprecatedMessage).Build();
        }
        
        _widgetNodes[node] = widget;
    }
}
