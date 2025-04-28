using Umbra.Common;
using Umbra.Widgets.System;
using Una.Drawing;

namespace Umbra.Windows.Library.Installer;

internal sealed class InstallerPrefillToolbarPage() : InstallerPage
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
                ConfigManager.Set("Toolbar.WidgetData", "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");
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
