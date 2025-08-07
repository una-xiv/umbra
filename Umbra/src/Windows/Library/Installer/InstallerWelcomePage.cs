namespace Umbra.Windows.Library.Installer;

internal sealed class InstallerWelcomePage(IPlayer player) : InstallerPage
{
    public override int Order => 0;

    protected override string UdtFile => "welcome.xml";

    protected override void OnActivate()
    {
        var welcomeText = I18N.Translate("Window.Installer.Welcome.Title", player.Name.Split(' ')[0]);
        Node.QuerySelector(".text > .title")!.NodeValue = welcomeText;
    }

    protected override void OnDeactivate()
    {
    }
}
