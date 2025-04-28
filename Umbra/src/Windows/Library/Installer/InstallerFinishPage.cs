using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Settings;

namespace Umbra.Windows.Library.Installer;

internal sealed class InstallerFinishPage(IPlayer player, WindowManager windowManager) : InstallerPage
{
    public override int Order => int.MaxValue;

    protected override string UdtFile => "finish.xml";

    private bool _openSettings = false;
    
    protected override void OnActivate()
    {
        var welcomeText = I18N.Translate("Window.Installer.Finish.Title", player.Name.Split(' ')[0]);
        Node.QuerySelector(".text > .title")!.NodeValue = welcomeText;
    }

    protected override void OnDeactivate()
    {
        if (!_openSettings) return;
        windowManager.Present("UmbraSettings", new SettingsWindow());
    }
}
