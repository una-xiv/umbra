namespace Umbra.Windows.Library.Installer;

internal class InstallerConfigBackupPage : InstallerPage
{
    public override int Order => 1;

    protected override string UdtFile => "backup.xml";

    private const string ProfileName = "Umbra v2 Backup";

    private bool _makeBackup = true;
    
    public override bool CanActivate()
    {
        return !ConfigManager.GetProfileNames().Contains(ProfileName);
    }

    protected override void OnActivate()
    {
        Node.QuerySelector<CheckboxNode>(".ctrl-backup")!.OnValueChanged += v => _makeBackup = v;
    }

    protected override void OnDeactivate()
    {
        if (!_makeBackup || ConfigManager.GetProfileNames().Contains(ProfileName)) return;
        ConfigManager.CreateProfile(ProfileName);
    }
}
