using Umbra.AuxBar;

namespace Umbra;

[Service]
internal sealed partial class Toolbar(AuxBarManager auxBars, IPlayer player, UmbraVisibility visibility) : IDisposable
{
    [OnDraw(executionOrder: int.MaxValue)]
    private void DrawToolbar()
    {
        if (!visibility.IsToolbarVisible()) return;

        Node.TooltipBackgroundColor = Color.GetNamedColor("Misc.TooltipBackground");
        Node.TooltipTextColor       = Color.GetNamedColor("Misc.TooltipText");

        UpdateToolbarWidth();
        UpdateToolbarNodeClassList();
        UpdateToolbarAutoHideOffset();

        bool usingWindow = BeginWindowDrawList(out var drawList);

        if (Enabled) {
            RenderToolbarNode(drawList);
        }

        RenderAuxBarNodes(drawList);

        if (usingWindow) ImGui.End();
    }

    public void Dispose()
    {
        _toolbarNode.Dispose();
    }
}
