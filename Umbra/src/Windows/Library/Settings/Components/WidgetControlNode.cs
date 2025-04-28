using Dalamud.Game.Text.SeStringHandling;
using ImGuiNET;
using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Dialogs;
using Una.Drawing;

namespace Umbra.Windows.Settings.Components;

/// <summary>
/// Represents a widget control node that is used in the settings modules for
/// reorganizing a toolbar.
/// </summary>
public class WidgetControlNode : UdtNode
{
    public ToolbarWidget Widget { get; }

    private WidgetManager Manager { get; } = Framework.Service<WidgetManager>();

    private readonly Node _labelNode;

    public WidgetControlNode(ToolbarWidget widget) : base("umbra.windows.settings.components.widget_control.xml")
    {
        Id        = $"WidgetControl-{widget.Id}";
        Widget    = widget;
        SortIndex = widget.SortIndex;

        _labelNode           = QuerySelector(".label")!;
        _labelNode.NodeValue = Widget.Info.Name;

        if (widget.Info.IsDeprecated) {
            var d = QuerySelector(".deprecated")!;
            d.Style.IsVisible = true;
            d.NodeValue       = new SeStringBuilder().AddIcon(BitmapFontIcon.Warning).Build();
            d.Tooltip         = widget.Info.DeprecatedMessage;
        }

        QuerySelector("#delete-button")!.OnClick += _ => OnDeleteButtonClicked();
        QuerySelector("#copy-button")!.OnClick   += _ => OnCopyButtonClicked();
        QuerySelector("#edit-button")!.OnClick   += _ => OnEditButtonClicked();

        OnDoubleClick += _ => OnEditButtonClicked();
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        if (IsDisposed || Widget.IsDisposed) return;

        SortIndex = Widget.SortIndex;

        try {
            _labelNode.NodeValue = Widget.GetInstanceName();
        } catch {
            _labelNode.NodeValue = Widget.Info.Name;
        }
    }

    private void OnEditButtonClicked()
    {
        Widget.OpenSettingsWindow();
    }

    private void OnCopyButtonClicked()
    {
        Manager.CreateCopyOfWidget(Widget.Id);
    }

    private void OnDeleteButtonClicked()
    {
        if (ImGui.GetIO().KeyShift) {
            Manager.RemoveWidget(Widget.Id);
            Dispose();
            return;
        }

        Framework.Service<WindowManager>().Present<ConfirmationWindow>("DeleteWidgetInstanceConfirmation2", new(
            I18N.Translate("ConfirmationDialog.Title"),
            I18N.Translate("ConfirmationDialog.DeleteWidget.Message", Widget.GetInstanceName()),
            I18N.Translate("Delete"),
            I18N.Translate("Cancel"),
            I18N.Translate("ConfirmationDialog.OmitDialog")
        ), window => {
            if (!window.Confirmed) return;
            Manager.RemoveWidget(Widget.Id);
            Dispose();
        });
    }
}
