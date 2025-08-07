using Umbra.Widgets.System;

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
