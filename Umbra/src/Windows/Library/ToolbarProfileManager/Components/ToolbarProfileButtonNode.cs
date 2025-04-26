using ImGuiNET;
using Umbra.Common;
using Umbra.Widgets.System;
using Umbra.Windows.Dialogs;
using Umbra.Windows.Settings.Modules;

namespace Umbra.Windows.Library.ToolbarProfileManager.Components;

public class ToolbarProfileButtonNode : UdtNode
{
    public string Name { get; }

    private WidgetManager Manager { get; } = Framework.Service<WidgetManager>();

    public ToolbarProfileButtonNode(string name) : base("umbra.windows.toolbar_profiles.components.toolbar_profile_button.xml")
    {
        Name = name;

        QuerySelector(".name")!.NodeValue        =  name;
        QuerySelector("#delete-button")!.OnClick += _ => DeleteProfile();
        QuerySelector("#export-button")!.OnClick += _ => Manager.ExportProfileToClipboard();
    }

    protected override void OnDraw(ImDrawListPtr _)
    {
        ToggleClass("active", WidgetManager.ActiveProfile == Name);
        QuerySelector("#delete-button")!.Style.IsVisible = Manager.CanDeleteProfile(Name);
    }

    private void DeleteProfile()
    {
        if (ImGui.GetIO().KeyShift) {
            Manager.DeleteProfile(Name);
            return;
        }
        
        Framework.Service<WindowManager>().Present<ConfirmationWindow>("DeleteToolbarProfileConfirmation", new (
            "Are you sure?",
            "This will permanently delete the profile \"{Name}\". This operation cannot be undone.",
            "Delete",
            "Cancel",
            "Hold shift while clicking the delete button to omit this confirmation."
        ), window => {
            if (window.Confirmed) {
                Manager.DeleteProfile(Name);
            }
        });
    }
}
