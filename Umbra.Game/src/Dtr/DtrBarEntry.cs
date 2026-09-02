using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Text.ReadOnly;

namespace Umbra.Game;

public class DtrBarEntry(IReadOnlyDtrBarEntry entry, int sortIndex)
{
    public string           Name          => DtrEntry.Title;
    public ReadOnlySeString Text          => DtrEntry.Text?.Encode() ?? [];
    public ReadOnlySeString TooltipText   => DtrEntry.Tooltip?.Encode() ?? [];
    public bool             IsInteractive => DtrEntry.HasClickAction;
    public int              SortIndex     { get; private set; } = sortIndex;
    public bool             IsVisible     { get; private set; } = entry is { Shown: true, UserHidden: false };


    private IReadOnlyDtrBarEntry DtrEntry { get; set; } = entry;

    internal void Update(IReadOnlyDtrBarEntry updatedEntry, int sortIndex)
    {
        DtrEntry  = updatedEntry;
        IsVisible = updatedEntry is { Shown: true, UserHidden: false };
        SortIndex = sortIndex;
    }

    public void InvokeClickAction(MouseClickType clickType = MouseClickType.Left, ClickModifierKeys modifierKeys = ClickModifierKeys.None)
    {
        if (!DtrEntry.HasClickAction) return;

        DtrEntry.OnClick?.Invoke(new DtrInteractionEvent {
            ClickType       = clickType,
            ModifierKeys    = modifierKeys,
            ScrollDirection = MouseScrollDirection.None,
            Position        = ImGui.GetMousePos()
        });
    }
}
