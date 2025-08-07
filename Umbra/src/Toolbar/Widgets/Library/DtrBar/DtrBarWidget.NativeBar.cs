using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Umbra.Widgets;

internal unsafe partial class DtrBarWidget
{
    private void UpdateNativeServerInfoBar()
    {
        if (! GetConfigValue<bool>("HideNative")) {
            SetNativeServerInfoBarVisibility(true);
            return;
        }

        SetNativeServerInfoBarVisibility(!(Toolbar.Enabled && Framework.Service<UmbraVisibility>().IsToolbarVisible()));
    }

    private void SetNativeServerInfoBarVisibility(bool isVisible)
    {
        var dtrBar = (AtkUnitBase*) _gameGui!.GetAddonByName("_DTR").Address;
        if (dtrBar != null && dtrBar->IsVisible != isVisible) {
            dtrBar->IsVisible = isVisible;
        }
    }
}
