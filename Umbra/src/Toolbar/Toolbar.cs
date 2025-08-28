using Umbra.AuxBar;

namespace Umbra;

[Service]
internal sealed partial class Toolbar(AuxBarManager auxBars, IPlayer player, UmbraVisibility visibility) : IDisposable
{
    [OnDraw(executionOrder: int.MaxValue)]
    private void DrawToolbar()
    {
        UpdateToolbarWidth();
        UpdateToolbarNodeClassList();
        UpdateToolbarAutoHideOffset();

        if (Enabled && visibility.IsToolbarVisible()) {
            RenderToolbarNode();
        }
        
        RenderAuxBarNodes();
    }

    public void Dispose()
    {
        _toolbarNode.Dispose();
    }
}
