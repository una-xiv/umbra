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
