using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Lumina.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Umbra.Windows.Library.ToolbarProfileManager.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.ToolbarProfileManager;

public class ToolbarProfileManagerWindow : Window
{
    protected override string  UdtResourceName => "umbra.windows.toolbar_profiles.window.xml";
    protected override string  Title           => I18N.Translate("ToolbarProfilesWindow.Title");
    protected override Vector2 MinSize         => new(900, 300);
    protected override Vector2 MaxSize         => new(1200, 900);
    protected override Vector2 DefaultSize     => new(900, 500);

    private WidgetManager Manager => Framework.Service<WidgetManager>();

    private readonly Dictionary<string, (Node, Node)> _tabs = [];

    private Node ProfileListNode    => RootNode.QuerySelector("#profile-list")!;
    private Node ProfileEditNode    => RootNode.QuerySelector("#profile-edit")!;
    private Node JobAssociationNode => RootNode.QuerySelector("#jobs-edit")!;

    protected override void OnOpen()
    {
        CreateTabButton("profiles-tab", "Profiles", true);
        CreateTabButton("jobs-tab", "Job Associations");

        RefreshProfileList();
        CreateProfileEditNodes();
        CreateJobAssociationNodes();

        RootNode.QuerySelector("#close-button")!.OnClick += _ => Close();

        Manager.ProfileCreated += OnProfileCreated;
        Manager.ProfileRemoved += OnProfileDeleted;
    }

    protected override void OnClose()
    {
        Manager.ProfileCreated -= OnProfileCreated;
        Manager.ProfileRemoved -= OnProfileDeleted;
    }

    protected override void OnDraw()
    {
    }

    private void CreateTabButton(string id, string label, bool isActive = false)
    {
        Node button  = Document!.CreateNodeFromTemplate("tab-button", new() { { "name", label } });
        Node content = RootNode.QuerySelector($"#{id}")!;

        _tabs[id] = (button, content);

        if (isActive) {
            button.ToggleTag("selected");
            content.ToggleClass("hidden", false);
        }

        button.OnClick += _ => {
            foreach (var (tabId, (tab, node)) in _tabs) {
                tab.ToggleTag("selected", tabId == id);
                node.ToggleClass("hidden", tabId != id);
                CreateJobAssociationNodes();
            }
        };

        RootNode.QuerySelector(".tab-bar > .button-list")!.AppendChild(button);
    }

    private void RefreshProfileList()
    {
        ProfileListNode.Clear();

        foreach (var name in Manager.GetProfileNames()) {
            OnProfileCreated(name);
        }
    }

    private void CreateProfileEditNodes()
    {
        CreateNewProfilePanel();
    }

    private void CreateNewProfilePanel()
    {
        Node panelNode   = CreateCollapsiblePanel(ProfileEditNode, I18N.Translate("ToolbarProfilesWindow.CreateProfile"));
        Node buttonsNode = new() { Style = new() { Gap = 8 } };

        StringInputNode inputNode = new(
            "ProfileName",
            "",
            24,
            I18N.Translate("ToolbarProfilesWindow.CreateProfileInput.Name"),
            I18N.Translate("ToolbarProfilesWindow.CreateProfileInput.Description"),
            0,
            true
        );

        ButtonNode createBlankButton = new(
            "CreateProfileButtonBlank",
            I18N.Translate("ToolbarProfilesWindow.CreateProfileButton.Blank")
        ) { IsDisabled = true };

        ButtonNode createCopyOfButton = new(
            "CreateProfileButtonCopy",
            I18N.Translate("ToolbarProfilesWindow.CreateProfileButton.CopyOf", "Default")
        ) { IsDisabled = true };

        ButtonNode createFromClipboardButton = new(
            "CreateProfileButtonClipboard",
            I18N.Translate("ToolbarProfilesWindow.CreateProfileButton.Clipboard")
        ) { IsDisabled = true };

        panelNode.AppendChild(inputNode);
        panelNode.AppendChild(buttonsNode);
        buttonsNode.AppendChild(createBlankButton);
        buttonsNode.AppendChild(createCopyOfButton);
        buttonsNode.AppendChild(createFromClipboardButton);

        string newProfileName = "";

        inputNode.OnValueChanged += name => {
            name = name.Trim();
            bool valid = !string.IsNullOrEmpty(name) && name.Length <= 24 && !Manager.ProfileExists(name);

            createBlankButton.IsDisabled         = !valid;
            createCopyOfButton.IsDisabled        = !valid;
            createFromClipboardButton.IsDisabled = !valid;

            newProfileName = valid ? name : "";
        };

        createBlankButton.OnClick += _ => {
            if (string.IsNullOrEmpty(newProfileName)) return;
            Manager.CreateBlankProfile(newProfileName);
            inputNode.Value = "";
            newProfileName  = "";
        };

        createCopyOfButton.OnClick += _ => {
            if (string.IsNullOrEmpty(newProfileName)) return;
            Manager.CreateCopiedProfile(newProfileName);
            inputNode.Value = "";
            newProfileName  = "";
        };

        createFromClipboardButton.OnClick += _ => {
            if (string.IsNullOrEmpty(newProfileName)) return;
            Manager.CreateProfileFromClipboard(newProfileName);
            inputNode.Value = "";
            newProfileName  = "";
        };

        var player = Framework.Service<IPlayer>();

        Node jobAssociationMessage = new() {
            Id        = "JobAssociationWarning",
            ClassList = ["job-association-message"],
            NodeValue = new SeStringBuilder().AddIcon(BitmapFontIcon.Warning).AddText(" ").AddText(I18N.Translate("ToolbarProfilesWindow.JobAssociationWarning", player.GetJobInfo(player.JobId).Name)).Build(),
        };

        jobAssociationMessage.Style.IsVisible = WidgetManager.UseJobAssociatedProfiles;

        ProfileEditNode.AppendChild(jobAssociationMessage);
    }

    private void OnProfileCreated(string name)
    {
        var node = new ToolbarProfileButtonNode(name);

        node.Id = GetNodeIdForProfile(name);
        ProfileListNode.AppendChild(node);

        node.OnClick += _ => Manager.ActivateProfile(name);
    }

    private void OnProfileDeleted(string name)
    {
        ProfileListNode.QuerySelector($"#{GetNodeIdForProfile(name)}")?.Dispose();
    }

    private void CreateJobAssociationNodes()
    {
        JobAssociationNode.Clear();

        List<ClassJob> allJobs  = Framework.Service<IDataManager>().GetExcelSheet<ClassJob>().ToList();
        List<string>   profiles = Manager.GetProfileNames();

        Dictionary<ClassJobCategory, List<ClassJob>> categorizedJobs = [];
        foreach (var job in allJobs) {
            if (job.RowId == 0 || job.Abbreviation.ToString() == "ADV") continue;

            if (!categorizedJobs.TryGetValue(job.ClassJobCategory.Value, out var list)) {
                categorizedJobs.Add(job.ClassJobCategory.Value, list = []);
            }

            list.Add(job);
        }

        foreach (var jobs in categorizedJobs.Values) {
            jobs.Sort((a, b) => String.Compare(a.Name.ExtractText(), b.Name.ExtractText(), StringComparison.Ordinal));
        }

        CheckboxNode node = new(
            "EnableJobAssociations",
            WidgetManager.UseJobAssociatedProfiles,
            I18N.Translate("ToolbarProfilesWindow.UseJobAssociatedProfiles.Name"),
            I18N.Translate("ToolbarProfilesWindow.UseJobAssociatedProfiles.Description")
        );

        node.OnValueChanged += state => {
            ConfigManager.Set("Toolbar.UseJobAssociatedProfiles", state);
            ProfileEditNode.QuerySelector("#JobAssociationWarning")!.Style.IsVisible = state;
        };

        Node jobListNode = new() { ClassList = ["jobs-list"] };
        jobListNode.Style.IsVisible = WidgetManager.UseJobAssociatedProfiles;

        JobAssociationNode.AppendChild(node);
        JobAssociationNode.AppendChild(jobListNode);

        foreach (var (category, jobs) in categorizedJobs) {
            Node bodyNode = CreateCollapsiblePanel(jobListNode, category.Name.ExtractText());

            foreach (var job in jobs) {
                SelectNode jobNode = new(
                    $"JobAssociation_{job.RowId}",
                    Manager.GetProfileNameForJobId((byte)job.RowId),
                    profiles,
                    job.NameEnglish.ExtractText()
                );

                jobNode.OnValueChanged += profile => {
                    Manager.SetProfileNameForJob((byte)job.RowId, profile);
                };

                bodyNode.AppendChild(jobNode);
            }
        }

        node.OnValueChanged += state => {
            WidgetManager.UseJobAssociatedProfiles = state;
            jobListNode.Style.IsVisible            = state;
        };
    }

    private static string GetNodeIdForProfile(string name) => $"Profile_{Crc32.Get(name)}";
}
