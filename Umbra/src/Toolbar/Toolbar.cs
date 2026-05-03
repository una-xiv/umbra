using Umbra.AuxBar;

namespace Umbra;

[Service]
internal sealed partial class Toolbar(AuxBarManager auxBars, IPlayer player, UmbraVisibility visibility) : IDisposable
{
    // フレームごとに1回だけ計算し、複数の呼び出し元で再利用する
    private bool _cachedIsCursorNear;

    [OnDraw(executionOrder: int.MaxValue)]
    private void DrawToolbar()
    {
        if (!visibility.IsToolbarVisible()) return;

        Node.TooltipBackgroundColor = Color.GetNamedColor("Misc.TooltipBackground");
        Node.TooltipTextColor       = Color.GetNamedColor("Misc.TooltipText");

        // フレーム内で複数回呼ばれる IsCursorNearToolbar() を1回に集約
        _cachedIsCursorNear = IsCursorNearToolbar();

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
