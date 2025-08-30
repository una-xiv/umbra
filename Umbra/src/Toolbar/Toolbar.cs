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
        Node.TooltipTextColor       = Color.GetNamedColor("Window.TextMuted");

        UpdateToolbarWidth();
        UpdateToolbarNodeClassList();
        UpdateToolbarAutoHideOffset();

        if (Enabled) {
            RenderToolbarNode();
        }

        RenderAuxBarNodes();
    }

    public void Dispose()
    {
        _toolbarNode.Dispose();
    }
}
