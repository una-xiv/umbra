using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.System;
using Umbra.Windows.Components;

namespace Umbra.Windows.Library.Installer;

internal sealed class InstallerConfigureToolbarPage() : InstallerPage
{
    public override int Order => 11;

    protected override string UdtFile => "configure_toolbar.xml";

    public override bool CanActivate() => Toolbar.Enabled;

    protected override void OnActivate()
    {
        CheckboxNode position      = Node.QuerySelector<CheckboxNode>("#ctrl-toolbar-position")!;
        CheckboxNode stretched     = Node.QuerySelector<CheckboxNode>("#ctrl-toolbar-stretched")!;
        CheckboxNode autoHide      = Node.QuerySelector<CheckboxNode>("#ctrl-toolbar-auto-hide")!;
        CheckboxNode quickSettings = Node.QuerySelector<CheckboxNode>("#ctrl-toolbar-quick-settings")!;

        position.Value = Toolbar.IsTopAligned;
        position.OnValueChanged += v => ConfigManager.Set("Toolbar.IsTopAligned", v);
        
        stretched.Value = Toolbar.IsStretched;
        stretched.OnValueChanged += v => ConfigManager.Set("Toolbar.IsStretched", v);
        
        autoHide.Value = Toolbar.IsAutoHideEnabled;
        autoHide.OnValueChanged += v => ConfigManager.Set("Toolbar.IsAutoHideEnabled", v);
        
        quickSettings.Value          =  WidgetManager.EnableQuickSettingAccess;
        quickSettings.OnValueChanged += v => ConfigManager.Set("Toolbar.EnableQuickSettingAccess", v);
    }

    protected override void OnDeactivate()
    {
    }
}
