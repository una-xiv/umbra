using Umbra.Common;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra.Windows.Library.Installer;

internal sealed class InstallerPrefillToolbarPage(WidgetManager widgetManager) : InstallerPage
{
    public override int Order => 10;

    protected override string UdtFile => "prefill_toolbar.xml";

    private Node YesButton     => Node.QuerySelector("#btn-yes")!;
    private Node NoButton      => Node.QuerySelector("#btn-no")!;
    private Node DisableButton => Node.QuerySelector("#btn-disable")!;

    private string _selectedOption = string.Empty;

    public override bool CanProceed() => _selectedOption != string.Empty;

    protected override void OnActivate()
    {
        YesButton.OnClick     += _ => SelectOption("yes");
        NoButton.OnClick      += _ => SelectOption("no");
        DisableButton.OnClick += _ => SelectOption("disable");

        // In case the user has already selected an option and is returning to this page.
        SelectOption(_selectedOption);
    }

    protected override void OnDeactivate()
    {
        switch (_selectedOption) {
            case "disable":
                ConfigManager.Set("Toolbar.Enabled", false);
                break;
            case "no":
                ConfigManager.Set("Toolbar.Enabled", true);
                break;
            case "yes":
                ConfigManager.Set("Toolbar.Enabled", true);
                widgetManager.CreateProfileFromClipboard();
                break;
        }
    }

    private void SelectOption(string option)
    {
        _selectedOption = option;
        
        YesButton.ToggleClass("selected", option == "yes");
        NoButton.ToggleClass("selected", option == "no");
        DisableButton.ToggleClass("selected", option == "disable");
    }
}
