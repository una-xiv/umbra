using Dalamud.Game.Gui.Dtr;

namespace Umbra.Widgets;

[ToolbarWidget(
    "SingleDtrEntry",
    "Widget.DtrSingle.Name",
    "Widget.DtrSingle.Description",
    ["dtr", "server", "info", "plugin", "addon"]
)]
internal partial class DtrSingleWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultDisplayMode => DisplayModeTextOnly;

    private IDtrBar         DtrBar       { get; } = Framework.Service<IDtrBar>();
    private HashSet<string> KnownEntries { get; } = [];

    private IReadOnlyDtrBarEntry? DtrBarEntry { get; set; }

    protected override void OnDraw()
    {
        bool            isChanged      = false;
        HashSet<string> currentEntries = [];

        foreach (var e in DtrBar.Entries) {
            if (KnownEntries.Add(e.Title)) isChanged = true;
            currentEntries.Add(e.Title);
        }

        foreach (var e in KnownEntries.Except(currentEntries).ToArray()) {
            KnownEntries.Remove(e);
            isChanged = true;
        }

        if (isChanged) {
            UpdateConfigVariables();
        }

        string                title = GetConfigValue<string>("SelectedEntry");
        IReadOnlyDtrBarEntry? entry = DtrBar.Entries.FirstOrDefault(e => e.Title == title) ?? DtrBar.Entries.FirstOrDefault();

        if (entry != DtrBarEntry) {
            Node.OnClick      -= OnLeftClick;
            Node.OnRightClick -= OnRightClick;

            DtrBarEntry = entry;

            if (entry is { HasClickAction: true }) {
                Node.OnClick      += OnLeftClick;
                Node.OnRightClick += OnRightClick;
            }
        }

        if (DtrBarEntry == null) {
            SetText("");
            SetDisabled(true);
            return;
        }

        SetDisabled(false);
        SetText(DtrBarEntry.Text);
        SetTooltip(DtrBarEntry.Tooltip?.TextValue);
    }

    private void OnLeftClick(Node _)
    {
        if (DtrBarEntry is { HasClickAction: true }) {
            DtrBarEntry?.OnClick!(CreateDtrInteractionEvent(MouseClickType.Left));
        }
    }

    private void OnRightClick(Node _)
    {
        if (DtrBarEntry is { HasClickAction: true }) {
            DtrBarEntry?.OnClick!(CreateDtrInteractionEvent(MouseClickType.Right));
        }
    }

    private static DtrInteractionEvent CreateDtrInteractionEvent(MouseClickType clickType)
    {
        return new DtrInteractionEvent() {
            ClickType       = clickType,
            ModifierKeys    = GetModifierKeyState(),
            Position        = ImGui.GetMousePos(),
            ScrollDirection = MouseScrollDirection.None,
        };
    }

    private static ClickModifierKeys GetModifierKeyState()
    {
        bool shift = ImGui.IsKeyDown(ImGuiKey.LeftShift) || ImGui.IsKeyDown(ImGuiKey.RightShift);
        bool ctrl  = ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || ImGui.IsKeyDown(ImGuiKey.RightCtrl);
        bool alt   = ImGui.IsKeyDown(ImGuiKey.LeftAlt) || ImGui.IsKeyDown(ImGuiKey.RightAlt);

        ClickModifierKeys modifierKeys = ClickModifierKeys.None;

        if (shift) modifierKeys |= ClickModifierKeys.Shift;
        if (ctrl) modifierKeys  |= ClickModifierKeys.Ctrl;
        if (alt) modifierKeys   |= ClickModifierKeys.Alt;

        return modifierKeys;
    }
}
