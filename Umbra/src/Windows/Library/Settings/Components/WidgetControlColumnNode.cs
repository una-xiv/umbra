using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Immutable;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Dialogs;
using Umbra.Windows.Library.WidgetBrowser;
using Una.Drawing;

namespace Umbra.Windows.Settings.Components;

public class WidgetControlColumnNode : UdtNode
{
    private WidgetManager WidgetManager { get; } = Framework.Service<WidgetManager>();
    private WindowManager WindowManager { get; } = Framework.Service<WindowManager>();

    private string Location   { get; }
    private string ColumnName { get; }

    private double _clipboardCheckTime;

    public WidgetControlColumnNode(string location, string label) : base("umbra.windows.settings.components.widget_control_column.xml")
    {
        Location   = location;
        ColumnName = label;

        QuerySelector(".header")!.NodeValue = label;

        QuerySelector("#add-button")!.OnClick     += _ => OnAddButtonClicked();
        QuerySelector("#pst-button")!.OnClick     += _ => OnPasteButtonClicked();
        QuerySelector("#clr-button")!.OnMouseDown += _ => OnClearButtonClicked();
        QuerySelector(".widgets")!.OnSorted       += OnWidgetListSorted;
        QuerySelector(".widgets")!.OnChildAdded   += OnWidgetAddedToList;

        WidgetManager.OnWidgetCreated   += OnWidgetInstanceCreated;
        WidgetManager.OnWidgetRemoved   += OnWidgetInstanceRemoved;
        WidgetManager.OnWidgetRelocated += OnWidgetInstanceRelocated;

        foreach (var instance in WidgetManager.GetWidgetInstances()) OnWidgetInstanceCreated(instance);
    }

    protected override void OnDisposed()
    {
        WidgetManager.OnWidgetCreated   -= OnWidgetInstanceCreated;
        WidgetManager.OnWidgetRemoved   -= OnWidgetInstanceRemoved;
        WidgetManager.OnWidgetRelocated -= OnWidgetInstanceRelocated;

        base.OnDisposed();
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now - _clipboardCheckTime > 0) {
            _clipboardCheckTime = now;
            QuerySelector("#pst-button")!.Style.IsVisible = WidgetManager.CanCreateInstanceFromClipboard();
        }
    }

    private void OnWidgetInstanceCreated(ToolbarWidget widget)
    {
        if (widget.Location != Location) return;
        QuerySelector(".widgets")!.AppendChild(new WidgetControlNode(widget));
    }

    private void OnWidgetInstanceRemoved(ToolbarWidget widget)
    {
        QuerySelector($"#WidgetControl-{widget.Id}")?.Dispose();
    }

    private void OnWidgetInstanceRelocated(ToolbarWidget widget, string newLocation)
    {
    }

    private void OnAddButtonClicked()
    {
        Framework.Service<WindowManager>().Present("AddWidgetWindow", new WidgetBrowserWindow(), window => {
            if (null == window.SelectedWidgetId) return;

            var widget = WidgetManager.GetWidgetInfoList().FirstOrDefault(wi => wi.Id == window.SelectedWidgetId);
            if (null == widget) return;

            WidgetManager.CreateWidget(widget.Id, Location);
        });
    }

    private void OnPasteButtonClicked()
    {
        if (!WidgetManager.CanCreateInstanceFromClipboard()) return;
        WidgetManager.CreateInstanceFromClipboard(Location);
    }

    private void OnWidgetListSorted(Node list)
    {
        int sortIndex = 0;

        foreach (var node in list.ChildNodes.ToImmutableArray()) {
            if (node is not WidgetControlNode control) continue;

            control.Widget.SortIndex = sortIndex;
            WidgetManager.SaveWidgetState(control.Widget.Id);
            sortIndex++;
        }

        WidgetManager.SaveState();
    }

    private void OnWidgetAddedToList(Node node)
    {
        if (node is not WidgetControlNode control) return;
        control.Widget.Location = Location;

        WidgetManager.SaveWidgetState(control.Widget.Id);
        WidgetManager.SaveState();
    }

    private void OnClearButtonClicked()
    {
        WindowManager.Present("DeleteAllWidgetsFromColumn", new ConfirmationWindow(
            I18N.Translate("ConfirmationDialog.Title"),
            I18N.Translate("ConfirmationDialog.DeleteAllWidgets.Message", ColumnName),
            I18N.Translate("Delete"),
            I18N.Translate("Cancel")
        ), window => {
            if (!window.Confirmed) return;

            foreach (var node in QuerySelector(".widgets")!.ChildNodes.ToImmutableArray()) {
                if (node is not WidgetControlNode control) continue;
                node.Dispose();
                WidgetManager.RemoveWidget(control.Widget.Id);
            }
        });
    }
}
