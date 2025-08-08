using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Action = System.Action;

namespace Umbra.Game;

public class DtrBarEntry(IReadOnlyDtrBarEntry entry, int sortIndex)
{
    public string    Name { get; private set; } = entry.Title;
    public SeString? Text { get; private set; } = entry.Text;
    public SeString? TooltipText   { get; private set; } = entry.Tooltip;
    public int       SortIndex     { get; private set; } = sortIndex;
    public bool      IsVisible     { get; private set; } = entry is { Shown: true, UserHidden: false };
    public bool      IsInteractive { get; private set; } = entry.HasClickAction;

    private readonly Action? _onClick = () => {
        if (!entry.HasClickAction) return;
        entry.OnClick?.Invoke(new DtrInteractionEvent {
            ClickType = MouseClickType.Left,
            ModifierKeys = ClickModifierKeys.None,
            ScrollDirection = MouseScrollDirection.None,
            Position = ImGui.GetMousePos()
        });
    };

    internal void Update(IReadOnlyDtrBarEntry entry, int sortIndex)
    {
        Text          = entry.Text;
        TooltipText   = entry.Tooltip;
        IsVisible     = entry is { Shown: true, UserHidden: false };
        IsInteractive = entry.HasClickAction;
        SortIndex     = sortIndex;
    }

    public void InvokeClickAction()
    {
        _onClick?.Invoke();
    }
}
